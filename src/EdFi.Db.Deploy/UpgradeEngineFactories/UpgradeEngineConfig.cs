// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Parameters.Verbs;

namespace EdFi.Db.Deploy.UpgradeEngineFactories
{
    public class UpgradeEngineConfig
    {
        public string ConnectionString { get; set; }

        public int TimeoutInSeconds { get; set; }

        public DatabaseType DatabaseType { get; set; }

        public string ParentPath { get; set; }

        public string ScriptPath { get; set; }

        public bool PerformWhatIf { get; set; }

        public string ParentFolder()
        {
            if (ParentPath.TrimEnd('\\')
                    .EndsWith("Ed-Fi-ODS", StringComparison.InvariantCultureIgnoreCase)
                || ParentPath.TrimEnd('\\')
                    .EndsWith("Ed-Fi-ODS-Implementation", StringComparison.InvariantCultureIgnoreCase))
                return ParentPath;

            var parent = new DirectoryInfo(ParentPath).Parent;

            return parent == null
                ? ParentPath
                : parent.FullName;
        }

        public static UpgradeEngineConfig Create(IOptions options)
        {
            Preconditions.ThrowIfNull(options, nameof(options));

            return new UpgradeEngineConfig
            {
                ConnectionString = options.ConnectionString,
                DatabaseType = options.DatabaseType,
                TimeoutInSeconds = options.TimeoutInSeconds,
                PerformWhatIf = options.GetType() == typeof(WhatIfExecution)
            };
        }
    }
}
