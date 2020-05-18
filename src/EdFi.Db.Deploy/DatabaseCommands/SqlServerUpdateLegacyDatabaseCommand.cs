// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using EdFi.Db.Deploy.DeployJournal;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Parameters.Verbs;
using EdFi.Db.Deploy.UpgradeEngineFactories;
using EdFi.Db.Deploy.UpgradeEngineStrategies;
using log4net;

namespace EdFi.Db.Deploy.DatabaseCommands
{
    public class SqlServerUpdateLegacyDatabaseCommand : IDatabaseCommand
    {
        public const string ExtensionErrorMessage =
            "At this time we cannot update a 3.2 database that has custom extensions in it, due to the possibility of breaking changes in the data standard update scripts. The VersionNumber table has been replaced with a DeployJournal table. You will need to fix extensions manually and then re-run the upgrade. See ODS/API 3.3.0 release notes for more information on how to continue with the upgrade.";
        private readonly IEdFiLegacyDatabaseRepository _legacyDatabaseRepository;
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IScriptPathInfoProvider _scriptPathInfoProvider;
        private readonly IUpgradeEngineFactory _upgradeEngineFactory;

        public SqlServerUpdateLegacyDatabaseCommand(IScriptPathInfoProvider scriptPathInfoProvider,
            IEdFiLegacyDatabaseRepository legacyDatabaseRepository, ISqlServerUpgradeEngineFactory upgradeEngineFactory)
        {
            _scriptPathInfoProvider = Preconditions.ThrowIfNull(scriptPathInfoProvider, nameof(scriptPathInfoProvider));
            _legacyDatabaseRepository = Preconditions.ThrowIfNull(legacyDatabaseRepository, nameof(legacyDatabaseRepository));
            _upgradeEngineFactory = Preconditions.ThrowIfNull(upgradeEngineFactory, nameof(upgradeEngineFactory));
        }

        public int Order
        {
            get => 5;
        }

        public DatabaseCommandResult Execute(IOptions options)
        {
            Preconditions.ThrowIfNull(options, nameof(options));

            if (options.DatabaseType != DatabaseType.ODS)
            {
                _logger.Debug("Legacy database check only applies for ODS database");

                return new DatabaseCommandResult {IsSuccessful = true};
            }

            if (!IsLegacyDatabase(options))
            {
                return new DatabaseCommandResult {IsSuccessful = true};
            }

            _logger.Info("Legacy database is found.");

            if (options.GetType() == typeof(WhatIfExecution))
            {
                // Do not perform upgrade - just indicate that an upgrade needs to occur.
                return new DatabaseCommandResult
                {
                    RequiresUpgrade = true,
                    IsSuccessful = true
                };
            }

            CreateDeployJournalTable(options);

            var versionLevelData = GetDatabaseVersionLevelData(options);
            var scriptFileVersionLevels = GetAllScriptFileVersionLevels(options);
            var deployJournalScripts = GetDeployJournalScriptsFromVersionLevelData(scriptFileVersionLevels, versionLevelData);
            LoadLegacyFileNamesIntoDeployJournal(deployJournalScripts, options);
            RemoveVersionLevelTable(options);

            if (HasExtensions(versionLevelData))
            {
                _logger.Error(ExtensionErrorMessage);
                return new DatabaseCommandResult {IsSuccessful = false};
            }

            return new DatabaseCommandResult {IsSuccessful = true};

            bool IsLegacyDatabase(IOptions opt)
            {
                _logger.Debug("Legacy database check: Query if [dbo].[VersionLevel] table exists in database");

                var tables = _legacyDatabaseRepository.FindAllTables(opt.ConnectionString) ?? new List<DatabaseTable>();

                return tables.Any(
                    table =>
                        table.TableSchema.Equals("dbo") &&
                        table.TableName.Equals("VersionLevel")
                );
            }

            VersionLevelData GetDatabaseVersionLevelData(IOptions opt)
            {
                return new VersionLevelData(_legacyDatabaseRepository.FindAllVersionLevels(opt.ConnectionString));
            }

            void CreateDeployJournalTable(IOptions opt)
            {
                _logger.Debug("Create [dbo].[DeployJournal] table");

                var config = UpgradeEngineConfig.Create(opt);

                var fakeScript = new SqlScript(
                    "create-deploy-journal.sql",
                    "/* this script does nothing except give DbUp an opportunity to create the DeployJournal*/");

                _upgradeEngineFactory.Create(config, fakeScript)
                    .PerformUpgrade();
            }

            void LoadLegacyFileNamesIntoDeployJournal(IReadOnlyList<string> scripts, IOptions opt)
            {
                _logger.Debug("Add [dbo].[DeployJournal] data");

                var insertList = scripts.Select(
                    x => new DeployJournalRecord
                    {
                        Applied = DateTime.Now,
                        ScriptName = x
                    });

                _legacyDatabaseRepository.InsertIntoDeployJournal(opt.ConnectionString, insertList);
            }

            void RemoveVersionLevelTable(IOptions opt)
            {
                _logger.Debug("Remove [dbo].[VersionLevel] table");
                _legacyDatabaseRepository.DropVersionLevelTable(opt.ConnectionString);
            }

            IReadOnlyList<ScriptFileVersionLevel> GetAllScriptFileVersionLevels(IOptions opts)
            {
                var allScriptPaths = _scriptPathInfoProvider.FindAllScriptsInFileSystem(opts);

                var allScriptFileVersionLevels = new List<ScriptFileVersionLevel>();

                foreach (var scriptPath in allScriptPaths)
                {
                    allScriptFileVersionLevels.AddRange(scriptPath.GetAllScriptFiles());
                }

                return allScriptFileVersionLevels;
            }

            IReadOnlyList<string> GetDeployJournalScriptsFromVersionLevelData(
                IEnumerable<ScriptFileVersionLevel> scriptFileVersionLevelsArg, VersionLevelData versionLevelDataArg)
            {
                var deployJournalDataProvider = new DeployJournalDataProvider(
                    versionLevelDataArg, scriptFileVersionLevelsArg.ToList());

                var scripts = new List<string>();

                scripts.AddRange(deployJournalDataProvider.LookupOdsStructureScripts());
                scripts.AddRange(deployJournalDataProvider.LookupOdsDataScripts());

                foreach (var extension in versionLevelData.ExtensionStructureVersionLevelByExtensionName.Keys)
                {
                    scripts.AddRange(deployJournalDataProvider.LookupExtensionStructureScripts(extension));
                }

                foreach (var extension in versionLevelData.ExtensionDataVersionLevelByExtensionName.Keys)
                {
                    scripts.AddRange(deployJournalDataProvider.LookupExtensionDataScripts(extension));
                }

                scripts.AddRange(deployJournalDataProvider.LookupOdsStructureChangeQueryScripts());

                return scripts;
            }

            bool HasExtensions(VersionLevelData versionLevels)
            {
                return versionLevels.ExtensionStructureVersionLevelByExtensionName.Any();
            }
        }
    }
}
