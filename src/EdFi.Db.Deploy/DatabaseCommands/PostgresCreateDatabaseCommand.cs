// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using DbUp;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.UpgradeEngineStrategies;
using log4net;

namespace EdFi.Db.Deploy.DatabaseCommands
{
    public class PostgresCreateDatabaseCommand : IDatabaseCommand
    {
        private readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        public int Order
        {
            get => 1;
        }

        public DatabaseCommandResult Execute(IOptions options)
        {
            Preconditions.ThrowIfNull(options, nameof(options));

            _logger.Debug("Creating postgres database if it does not exist");

            EnsureDatabase.For.PostgresqlDatabase(options.ConnectionString, new DbUpLogger(_logger));
            return new DatabaseCommandResult {IsSuccessful = true};
        }
    }
}
