// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

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
    public abstract class DeployLegacyExtensionsFromPathCommand : IDatabaseCommand
    {
        private readonly ILog _logger;
        private readonly IUpgradeEngineFactory _upgradeEngineFactory;

        protected DeployLegacyExtensionsFromPathCommand(IUpgradeEngineFactory upgradeEngineFactory, ILog logger)
        {
            _upgradeEngineFactory = Preconditions.ThrowIfNull(upgradeEngineFactory, nameof(upgradeEngineFactory));
            _logger = Preconditions.ThrowIfNull(logger, nameof(logger));
        }

        public int Order
        {
            get => 20;
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

            // deploy legacy extensions structure scripts
            // /SupportingArtifacts/Database/Structure/[DatabaseTypeFolder]/{*.sql}
            foreach (string path in filePaths)
            {
                commandResults.Add(RunScripts(path, true));
            }

            // deploy legacy extensions data scripts
            // /SupportingArtifacts/Database/Data/[DatabaseTypeFolder]/{*.sql}
            foreach (string path in filePaths)
            {
                commandResults.Add(RunScripts(path, true));
            }

            return DatabaseCommandResult.Create(commandResults);

            DatabaseCommandResult RunScripts(string path, bool isStructureScripts)
            {
                // note we can only run one path at a time, so we must recreate the db up instance.
                config.ParentPath = path;
                config.ScriptPath = ScriptsPath(new LegacyExtensionsScriptPathResolver(path, config.DatabaseType));

                if (!Directory.Exists(config.ScriptPath))
                {
                    _logger.Debug($"No legacy extension scripts found in {config.ScriptPath}");

                    return new DatabaseCommandResult { IsSuccessful = true };
                }

                _logger.Info($"Deploying legacy extension from path {config.ScriptPath}");

                var upgradeEngine = _upgradeEngineFactory.Create(config);

                if (config.PerformWhatIf)
                {
                    return new DatabaseCommandResult
                    {
                        IsSuccessful = true,
                        RequiresUpgrade = upgradeEngine.IsUpgradeRequired()
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
