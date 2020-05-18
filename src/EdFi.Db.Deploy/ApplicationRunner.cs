// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Db.Deploy.DatabaseCommands;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Specifications;
using log4net;

namespace EdFi.Db.Deploy
{
    public class ApplicationRunner
    {
        private readonly IDatabaseCommandFactory _databaseCommandFactory;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ApplicationRunner));
        private readonly IOptions _options;
        private readonly List<ICompositeSpecification> _specifications;

        public ApplicationRunner(
            IOptions options,
            IDatabaseCommandFactory databaseCommandFactory,
            IEnumerable<ICompositeSpecification> specifications)
        {
            _options = Preconditions.ThrowIfNull(options, nameof(options));
            _databaseCommandFactory = Preconditions.ThrowIfNull(databaseCommandFactory, nameof(databaseCommandFactory));

            _specifications = Preconditions.ThrowIfNull(specifications, nameof(specifications))
                .ToList();
        }

        public int Run()
        {
            try
            {
                // need to validate we have a valid setup of options
                if (!_specifications.Any(x => x.IsSatisfiedBy(_options)))
                {
                    _logger.Error("Unable to process options passed into the application");
                    _logger.Error("Errors found:");

                    foreach (var specification in _specifications)
                    {
                        _logger.Error(string.Join(Environment.NewLine, specification.ErrorMessages));
                    }

                    return ApplicationStatus.Failure;
                }

                var commands = _databaseCommandFactory.CreateDatabaseCommands(_options.Engine)
                    .ToList();

                foreach (IDatabaseCommand command in commands)
                {
                    var commandName = command.GetType()
                        .Name;

                    _logger.Debug($"Executing command {commandName}");
                    var commandResult = command.Execute(_options);

                    if (!commandResult.IsSuccessful)
                    {
                        if (commandResult.Exception == null)
                        {
                            return ApplicationStatus.Failure;
                        }

                        throw commandResult.Exception;
                    }

                    if (!commandResult.RequiresUpgrade)
                    {
                        continue;
                    }

                    _logger.Debug($"Upgrade is required for command {commandName}");
                    return ApplicationStatus.UpgradeIsRequired;
                }

                return ApplicationStatus.Success;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return ApplicationStatus.Failure;
            }
        }
    }
}
