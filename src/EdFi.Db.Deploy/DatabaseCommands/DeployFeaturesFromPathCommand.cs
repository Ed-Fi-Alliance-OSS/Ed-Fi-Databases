// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using EdFi.Db.Deploy.Extensions;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.ScriptPathResolvers;
using EdFi.Db.Deploy.UpgradeEngineFactories;
using EdFi.Db.Deploy.UpgradeEngineStrategies;
using log4net;

namespace EdFi.Db.Deploy.DatabaseCommands
{
    public abstract class DeployFeaturesFromPathCommand : IDatabaseCommand
    {
        private readonly ILog _logger;
        private readonly IUpgradeEngineFactory _upgradeEngineFactory;

        protected DeployFeaturesFromPathCommand(IUpgradeEngineFactory upgradeEngineFactory, ILog logger)
        {
            _upgradeEngineFactory = Preconditions.ThrowIfNull(upgradeEngineFactory, nameof(upgradeEngineFactory));
            _logger = Preconditions.ThrowIfNull(logger, nameof(logger));
        }

        public int Order
        {
            get => 30;
        }

        public DatabaseCommandResult Execute(IOptions options)
        {
            Preconditions.ThrowIfNull(options, nameof(options));

            var config = UpgradeEngineConfig.Create(options);

            return DeployFeaturesFromExtensions(options, config);
        }

        private DatabaseCommandResult DeployFeaturesFromExtensions(IOptions options, UpgradeEngineConfig config)
        {
            var features = options.Features.ToList();
            var filePaths = options.FilePaths.ToList();

            // if no features or extensions then we just exit with success
            if (!features.Any() || !filePaths.Any())
            {
                _logger.Debug("No features or file paths are found.");
                return new DatabaseCommandResult { IsSuccessful = true };
            }

            string engineTypeDirectory = options.Engine.Directory();

            var commandResults = new List<DatabaseCommandResult>();

            // deploy structure scripts
            // NewVersion:    /Artifacts/[EngineTypeFolder]/Structure/[DatabaseTypeFolder]/[Feature]/{*.sql}
            // LegacyVersion: /Database/Structure/[DatabaseTypeFolder]/[Feature]/{*.sql}
            foreach (string path in filePaths)
            {
                foreach (string feature in features)
                {
                    commandResults.Add(RunScripts(path, feature, true));
                }
            }

            // deploy data scripts
            // NewVersion:    /Artifacts/[EngineTypeFolder]/Data/[DatabaseTypeFolder]/[Feature]/{*.sql}
            // LegacyVersion: /Database/Data/[DatabaseTypeFolder]/[Feature]/{*.sql}
            foreach (string path in filePaths)
            {
                foreach (string feature in features)
                {
                    commandResults.Add(RunScripts(path, feature, false));
                }
            }

            return DatabaseCommandResult.Create(commandResults);

            DatabaseCommandResult RunScripts(string path, string feature, bool isStructureScripts)
            {
                // note we can only run one path at a time, so we must recreate the db up instance.
                config.ParentPath = path;
                config.ScriptPath = ScriptsPath(new ScriptPathResolver(path, options.DatabaseType, options.Engine, feature));

                if (!Directory.Exists(config.ScriptPath))
                {
                    _logger.Debug($"Feature {feature} is not found in path {config.ScriptPath}");

                    config.ScriptPath = ScriptsPath(new LegacyScriptPathResolver(path, options.DatabaseType, feature));

                    if (!Directory.Exists(config.ScriptPath))
                    {
                        _logger.Debug($"Feature {feature} is not found in path {config.ScriptPath}");
                        return new DatabaseCommandResult { IsSuccessful = true };
                    }

                    // If config.Path test fails for NewVersion, engine should not be PostgreSql
                    // only if folder exists
                    if (!options.AreFeaturesValidForLegacyDatabaseDirectoryStructure())
                    {
                        _logger.Warn("Currently only SqlServer is supported to deploy features for Legacy versions of Ed-Fi ODS Model");

                        return new DatabaseCommandResult { IsSuccessful = true };
                    }
                }

                _logger.Info($"Deploying feature {feature} from path {config.ScriptPath}");

                var upgradeEngine = _upgradeEngineFactory.Create(config);

                if (config.PerformWhatIf)
                {
                    return new DatabaseCommandResult
                    {
                        RequiresUpgrade = upgradeEngine.IsUpgradeRequired(),
                        IsSuccessful = true
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
            }
        }
    }
}
