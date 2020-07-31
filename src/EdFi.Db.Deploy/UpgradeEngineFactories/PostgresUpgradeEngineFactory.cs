﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using DbUp;
using DbUp.Builder;
using log4net;

namespace EdFi.Db.Deploy.UpgradeEngineFactories
{
    public class PostgresUpgradeEngineFactory : UpgradeEngineFactory
    {
        private readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        protected override UpgradeEngineBuilder UpgradeEngineBuilder(string connectionString)
        {
            var builder = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .JournalToPostgresqlTable(DatabaseConventions.Postgres.DefaultSchema, DatabaseConventions.JournalTable)
                .WithVariablesDisabled()
                .LogTo(new DbUpLogger(_logger));

            return builder;
        }
    }
}
