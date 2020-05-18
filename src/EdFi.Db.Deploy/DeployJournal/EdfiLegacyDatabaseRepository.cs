// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using EdFi.Db.Deploy.Helpers;

namespace EdFi.Db.Deploy.DeployJournal
{
    [ExcludeFromCodeCoverage]
    public class EdFiLegacyDatabaseRepository : IEdFiLegacyDatabaseRepository
    {
        // Note: cannot use Dapper.Contrib's GetAll where there is no primary key, e.g. on information_schema.tables or on dbo.versionlevel

        public IReadOnlyList<DatabaseTable> FindAllTables(string connectionString)
        {
            Preconditions.ThrowIfNull(connectionString, nameof(connectionString));

            const string Query =
                "SELECT [TABLE_SCHEMA] AS [TableSchema], [TABLE_NAME] AS [TableName] FROM [INFORMATION_SCHEMA].[TABLES]";

            using (var connection = CreateConnection(connectionString))
            {
                return connection.Query<DatabaseTable>(Query).ToList();
            }
        }

        public IReadOnlyList<DatabaseVersionLevel> FindAllVersionLevels(string connectionString)
        {
            Preconditions.ThrowIfNull(connectionString, nameof(connectionString));

            const string Query = "SELECT ScriptSource, ScriptType, DatabaseType, SubType, VersionLevel FROM dbo.VersionLevel";

            using (var connection = CreateConnection(connectionString))
            {
                return connection.Query<DatabaseVersionLevel>(Query).ToList();
            }
        }

        public void DropVersionLevelTable(string connectionString)
        {
            const string SqlStatement = "DROP TABLE [dbo].[VersionLevel]";

            using (var connection = CreateConnection(connectionString))
            {
                connection.Execute(SqlStatement);
            }
        }

        public void InsertIntoDeployJournal(string connectionString, IEnumerable<DeployJournalRecord> insertList)
        {
            using (var connection = CreateConnection(connectionString))
            {
                connection.Insert(insertList);
            }
        }

        private static IDbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);
    }
}
