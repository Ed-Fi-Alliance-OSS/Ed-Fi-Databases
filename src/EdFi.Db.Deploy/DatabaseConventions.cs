// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Db.Deploy
{
    public static class DatabaseConventions
    {
        public const string DataDirectory = "Data";
        public const string StructureDirectory = "Structure";
        public const string ArtifactsDirectory = "Artifacts";
        public const string SupportingArtifactsDirectory = "SupportingArtifacts";
        public const string DatabaseDirectory = "Database";
        public static string JournalTable = "DeployJournal";
        public const string StandardProject = "EdFi.Ods.Standard";
        public const string StandardFolder = "Standard";
        public const string VersionsFolder = "Versions";
        public const string DefaultExtensionVersion = "1.0.0";
        public const string ExtensionPrefix = "EdFi.Ods.Extensions.";
        public const string TPDMExtensionPath = $"{ExtensionPrefix}TPDM";

        public static class SqlServer
        {
            public const string DefaultSchema = "dbo";
        }

        public static class Postgres
        {
            public const string DefaultSchema = "public";
        }
    }
}
