// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Db.Deploy.Parameters.Verbs
{
    public abstract class AbstractOptionsVerb : IOptions
    {
        protected const string SampleConnectionString = "<connection string>";

        protected static readonly List<string> SampleFilePath = new List<string>
        {
            @"c:/Ed-Fi/Ed-Fi-ODS",
            @"c:/Ed-Fi/Ed-Fi-ODS-Implementation"
        };

        protected static readonly List<string> SampleFilePathsWithExtensions = new List<string>
        {
            @"c:/Ed-Fi/Ed-Fi-ODS",
            @"c:/Ed-Fi/Ed-Fi-ODS-Implementation",
            @"c:/Ed-Fi/Ed-Fi-ODS-Implementation/Application/EdFi.Ods.Extensions.GrandBend",
            @"c:/Ed-Fi/Ed-Fi-ODS-Implementation/Application/EdFi.Ods.Extensions.Homograph",
            @"c:/Ed-Fi/Ed-Fi-ODS-Implementation/Application/EdFi.Ods.Extensions.Sample"
        };

        protected static readonly List<string> SampleFeatures = new List<string>
        {
            "Changes",
            "Sample"
        };

        public DatabaseType DatabaseType { get; set; }

        public EngineType Engine { get; set; }

        public string ConnectionString { get; set; }

        public int TimeoutInSeconds { get; set; }

        public IEnumerable<string> FilePaths { get; set; }

        public IEnumerable<string> Features { get; set; }

        public bool AreFeaturesValidForLegacyDatabaseDirectoryStructure() => Engine == EngineType.SqlServer;
    }
}
