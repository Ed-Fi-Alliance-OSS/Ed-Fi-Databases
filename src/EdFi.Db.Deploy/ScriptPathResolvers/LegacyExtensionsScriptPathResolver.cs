// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;

namespace EdFi.Db.Deploy.ScriptPathResolvers
{
    public class LegacyExtensionsScriptPathResolver : LegacyScriptPathResolver
    {
        public LegacyExtensionsScriptPathResolver(string path, DatabaseType databaseType, string feature = null)
            : base(path, databaseType, feature) { }

        public override string StructureScriptPath()
            => Path.GetFullPath(
                !string.IsNullOrEmpty(_feature)
                    ? Path.Combine(
                        _path,
                        DatabaseConventions.SupportingArtifactsDirectory,
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.StructureDirectory,
                        DatabaseTypeDirectory(),
                        _feature)
                    : Path.Combine(
                        _path,
                        DatabaseConventions.SupportingArtifactsDirectory,
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.StructureDirectory,
                        DatabaseTypeDirectory()));

        public override string DataScriptPath()
            => Path.GetFullPath(
                !string.IsNullOrEmpty(_feature)
                    ? Path.Combine(
                        _path,
                        DatabaseConventions.SupportingArtifactsDirectory,
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.DataDirectory,
                        DatabaseTypeDirectory(),
                        _feature)
                    : Path.Combine(
                        _path,
                        DatabaseConventions.SupportingArtifactsDirectory,
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.DataDirectory,
                        DatabaseTypeDirectory()));
    }
}
