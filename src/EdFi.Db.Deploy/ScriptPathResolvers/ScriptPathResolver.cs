// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using EdFi.Db.Deploy.Extensions;
using EdFi.Db.Deploy.Helpers;

namespace EdFi.Db.Deploy.ScriptPathResolvers {
    public class ScriptPathResolver : ScriptPathResolverBase
    {
        private readonly EngineType _engineType;

        public ScriptPathResolver(string path, DatabaseType databaseType, EngineType engineType, string feature = null)
            : base(path, databaseType, feature)
        {
            _engineType = engineType;
        }

        public override string StructureScriptPath()
            => Path.GetFullPath(
                !string.IsNullOrEmpty(_feature)
                    ? Path.Combine(
                        _path,
                        DatabaseConventions.ArtifactsDirectory,
                        _engineType.Directory(),
                        DatabaseConventions.StructureDirectory,
                        DatabaseTypeDirectory(),
                        _feature)
                    : Path.Combine(
                        _path,
                        DatabaseConventions.ArtifactsDirectory,
                        _engineType.Directory(),
                        DatabaseConventions.StructureDirectory,
                        DatabaseTypeDirectory()));

        public override string DataScriptPath()
            => Path.GetFullPath(
                !string.IsNullOrEmpty(_feature)
                    ? Path.Combine(
                        _path,
                        DatabaseConventions.ArtifactsDirectory,
                        _engineType.Directory(),
                        DatabaseConventions.DataDirectory,
                        DatabaseTypeDirectory(),
                        _feature)
                    : Path.Combine(
                        _path,
                        DatabaseConventions.ArtifactsDirectory,
                        _engineType.Directory(),
                        DatabaseConventions.DataDirectory,
                        DatabaseTypeDirectory()));

        protected override string DatabaseTypeDirectory()
        {
            switch (_databaseType)
            {
                case DatabaseType.Admin:
                    return "Admin";

                case DatabaseType.Security:
                    return "Security";

                case DatabaseType.ODS:
                    return "Ods";

                default:
                    throw new ArgumentOutOfRangeException($"DatabaseType \"{_databaseType}\" is not found.");
            }
        }
    }
}
