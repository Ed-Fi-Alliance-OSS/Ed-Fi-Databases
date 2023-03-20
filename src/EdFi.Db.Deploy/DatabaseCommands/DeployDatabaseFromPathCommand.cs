// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.ScriptPathResolvers;
using EdFi.Db.Deploy.UpgradeEngineFactories;
using EdFi.Db.Deploy.UpgradeEngineStrategies;
using log4net;

namespace EdFi.Db.Deploy.DatabaseCommands
{
    public abstract class DeployDatabaseFromPathCommand : IDatabaseCommand
    {
        private const string EdFiStandardIdentifier = "EdFi.Ods.Standard";

        private readonly ILog _logger;
        private readonly IUpgradeEngineFactory _upgradeEngineFactory;

        protected DeployDatabaseFromPathCommand(IUpgradeEngineFactory upgradeEngineFactory, ILog logger)
        {
            _upgradeEngineFactory = Preconditions.ThrowIfNull(upgradeEngineFactory, nameof(upgradeEngineFactory));
            _logger = Preconditions.ThrowIfNull(logger, nameof(logger));
        }

        public int Order
        {
            get => 10;
        }

        public DatabaseCommandResult Execute(IOptions options)
        {
            Preconditions.ThrowIfNull(options, nameof(options));

            var filePaths = options.FilePaths.ToList();

            if (!filePaths.Any())
            {
                _logger.Debug("No file paths are found.");
                return new DatabaseCommandResult { IsSuccessful = false };
            }

            var commandResults = new List<DatabaseCommandResult>();

            var config = UpgradeEngineConfig.Create(options);

            // deploy structure scripts
            // NewVersion:    /Artifacts/[EngineTypeFolder]/Structure/[DatabaseTypeFolder]/{*.sql}
            // LegacyVersion: /Database/Structure/[DatabaseTypeFolder]/{*.sql}
            foreach (string path in filePaths)
            {
                var results = RunScripts(path, true, options.StandardVersion, options.ExtensionVersion);
                commandResults.Add(results);

                // If any filePath fails to upgrade, abort immediately to avoid further changing the database
                if (!results.IsSuccessful)
                {
                    DatabaseCommandResult.Create(commandResults);
                }
            }

            // deploy data scripts
            // NewVersion:    /Artifacts/[EngineTypeFolder]/Data/[DatabaseTypeFolder]/{*.sql}
            // LegacyVersion: /Database/Data/[DatabaseTypeFolder]/{*.sql}
            foreach (string path in filePaths)
            {
                var results = RunScripts(path, false, options.StandardVersion, options.ExtensionVersion);
                commandResults.Add(results);

                // If any filePath fails to upgrade, abort immediately to avoid further changing the database
                if (!results.IsSuccessful)
                {
                    DatabaseCommandResult.Create(commandResults);
                }
            }

            return DatabaseCommandResult.Create(commandResults);

            DatabaseCommandResult RunScripts(string path, bool isStructureScripts, string standardVersion, string extensionVersion)
            {
                config.ParentPath = path;
                config.ScriptPath = ScriptsPath(new ScriptPathResolver(path, options.DatabaseType, options.Engine, standardVersion: standardVersion, extensionVersion: extensionVersion));

                if (!Directory.Exists(config.ScriptPath))
                {
                    _logger.Debug($"No database scripts found in {config.ScriptPath}");

                    config.ScriptPath = ScriptsPath(new LegacyScriptPathResolver(path, options.DatabaseType));

                    if (!Directory.Exists(config.ScriptPath))
                    {
                        _logger.Debug($"No database scripts found in {config.ScriptPath}");

                        return new DatabaseCommandResult { IsSuccessful = true };
                    }
                }

                _logger.Info($"Deploying database from path {config.ScriptPath}");

                var upgradeEngine = _upgradeEngineFactory.Create(config);

                if (config.PerformWhatIf)
                {
                    return new DatabaseCommandResult
                    {
                        IsSuccessful = true,
                        RequiresUpgrade = upgradeEngine.IsUpgradeRequired()
                    };
                }

                // Explicitly block database structure upgrades for the Ed-Fi Standard project on ODS if an upgrade is required and scripts have previously been executed for this path
                if (isStructureScripts && config.DatabaseType == DatabaseType.ODS && IsEdFiStandardScriptsPath(path) &&
                    upgradeEngine.IsUpgradeRequired() && upgradeEngine.GetExecutedScripts().Any())
                {
                    return new DatabaseCommandResult
                    {
                        IsSuccessful = false,
                        RequiresUpgrade = true,
                        Exception = new Exception(
                            $"Upgrades are not supported at this time for database type {DatabaseType.ODS} using the {EdFiStandardIdentifier} data model. This tool only supports feature scripts for this type.")
                    };
                }

                var result = upgradeEngine.PerformUpgrade();

                return new DatabaseCommandResult
                {
                    IsSuccessful = result.Successful,
                    Exception = result.Error
                };

                string ScriptsPath(IScriptPathResolver fileInfoProvider)
                    => isStructureScripts
                        ? fileInfoProvider.StructureScriptPath()
                        : fileInfoProvider.DataScriptPath();

                bool IsEdFiStandardScriptsPath(string scriptsPath) => scriptsPath.Contains(EdFiStandardIdentifier);
            }
        }
    }
}
