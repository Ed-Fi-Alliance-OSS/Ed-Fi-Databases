// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using CommandLine;

namespace EdFi.Db.Deploy.Parameters
{
    public interface IOptions
    {
        [Option(
            'd',
            "database",
            Required = false,
            Default = DatabaseType.ODS,
            HelpText = "Database type: Admin, ODS, or Security")]
        DatabaseType DatabaseType { get; set; }

        [Option('e', "engine", Required = true, HelpText = "Database engine type: SqlServer or PostgreSql")]
        EngineType Engine { get; set; }

        [Option('c', "connectionString", Required = true, HelpText = "Connection string")]
        string ConnectionString { get; set; }

        [Option('t', "timeOut", Default = 60, HelpText = "Command timeout in seconds")]
        int TimeoutInSeconds { get; set; }

        [Option('p', "filePaths", Separator = ',', HelpText = "Path to Files to be installed")]
        IEnumerable<string> FilePaths { get; set; }

        [Option('f', "features", Separator = ',', HelpText = "Feature: Changes,Sample")]
        IEnumerable<string> Features { get; set; }

        bool AreFeaturesValidForLegacyDatabaseDirectoryStructure();
    }
}
