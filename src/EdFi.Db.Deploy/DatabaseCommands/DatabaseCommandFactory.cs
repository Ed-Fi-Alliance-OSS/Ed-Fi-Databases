// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Db.Deploy.Helpers;

namespace EdFi.Db.Deploy.DatabaseCommands
{
    public class DatabaseCommandFactory : IDatabaseCommandFactory
    {
        private readonly List<IDatabaseCommand> _postgresCommands;
        private readonly List<IDatabaseCommand> _sqlCommands;

        public DatabaseCommandFactory(IEnumerable<IDatabaseCommand> databaseCommands)
        {
            var commands = Preconditions.ThrowIfNull(databaseCommands, "databaseCommands")
                .ToList();

            _postgresCommands = commands.Where(
                    x => x.GetType()
                        .Name.StartsWith("Postgres", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            _sqlCommands = commands.Where(
                    x => x.GetType()
                        .Name.StartsWith("SqlServer", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        public IEnumerable<IDatabaseCommand> CreateDatabaseCommands(EngineType engineType)
        {
            // engineType will never be null since its an enum

            return engineType == EngineType.PostgreSql
                ? _postgresCommands.OrderBy(x => x.Order)
                : _sqlCommands.OrderBy(x => x.Order);
        }
    }
}
