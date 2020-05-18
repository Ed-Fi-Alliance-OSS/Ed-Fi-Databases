// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Db.Deploy.Extensions
{
    public static class DatabaseTypeExtensions
    {
        public static string Directory(this DatabaseType databaseType, ArtifactsFolderStructureType artifactsFolderStructureType)
        {
            switch (artifactsFolderStructureType)
            {
                case ArtifactsFolderStructureType.NewVersion:
                    return GetDirectoryNameForNewOdsApiVersion();

                case ArtifactsFolderStructureType.LegacyVersion:
                    return GetDirectoryNameForOdsApi32();

                default:
                    throw new ArgumentOutOfRangeException($"ArtifactsFolderStructureType \"{artifactsFolderStructureType}\" is not found.");
            }

            string GetDirectoryNameForNewOdsApiVersion()
            {
                switch (databaseType)
                {
                    case DatabaseType.Admin:
                        return "Admin";

                    case DatabaseType.Security:
                        return "Security";

                    case DatabaseType.ODS:
                        return "Ods";

                    default:
                        throw new ArgumentOutOfRangeException($"DatabaseType \"{databaseType}\" is not found.");
                }
            }

            string GetDirectoryNameForOdsApi32()
            {
                switch (databaseType)
                {
                    case DatabaseType.Admin:
                        return "EdFi_Admin";

                    case DatabaseType.Security:
                        return "EdFiSecurity";

                    case DatabaseType.ODS:
                        return "EdFi";

                    default:
                        throw new ArgumentOutOfRangeException($"DatabaseType \"{databaseType}\" is not found.");
                }
            }
        }
    }
}
