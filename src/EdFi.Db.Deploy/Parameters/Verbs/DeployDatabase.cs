// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace EdFi.Db.Deploy.Parameters.Verbs
{
    [Verb("deploy", HelpText = "Deploys the named database type")]
    public class DeployDatabase : AbstractOptionsVerb
    {
        // DbUp uses this implicitly - thus will show "0 references" in Code Lens
        [Usage(ApplicationAlias = "EdFi.Db.Deploy.exe")]

        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Deploy ODS database on SQL Server",
                    new DeployDatabase
                    {
                        DatabaseType = DatabaseType.ODS,
                        Engine = EngineType.SqlServer,
                        ConnectionString = SampleConnectionString,
                        FilePaths = SampleFilePath
                    });

                yield return new Example(
                    "Deploy Admin database on PostgreSQL",
                    new DeployDatabase
                    {
                        DatabaseType = DatabaseType.Admin,
                        Engine = EngineType.PostgreSql,
                        ConnectionString = SampleConnectionString,
                        FilePaths = SampleFilePath
                    });

                yield return new Example(
                    "Deploy Security database on PostgreSQL with 42 second command timeout",
                    new DeployDatabase
                    {
                        DatabaseType = DatabaseType.Security,
                        Engine = EngineType.PostgreSql,
                        ConnectionString = SampleConnectionString,
                        TimeoutInSeconds = 42,
                        FilePaths = SampleFilePath
                    });

                yield return new Example(
                    "Deploy ODS database on SqlServer with extensions",
                    new DeployDatabase
                    {
                        DatabaseType = DatabaseType.ODS,
                        Engine = EngineType.SqlServer,
                        ConnectionString = SampleConnectionString,
                        FilePaths = SampleFilePathsWithExtensions
                    });

                yield return new Example(
                    "Deploy ODS database on SqlServer with features",
                    new DeployDatabase
                    {
                        DatabaseType = DatabaseType.ODS,
                        Engine = EngineType.SqlServer,
                        ConnectionString = SampleConnectionString,
                        FilePaths = SampleFilePath,
                        Features = SampleFeatures
                    });

                yield return new Example(
                    "Deploy ODS database on SqlServer with extensions and features",
                    new DeployDatabase
                    {
                        DatabaseType = DatabaseType.ODS,
                        Engine = EngineType.SqlServer,
                        ConnectionString = SampleConnectionString,
                        FilePaths = SampleFilePathsWithExtensions,
                        Features = SampleFeatures
                    });
            }
        }
    }
}
