// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using EdFi.Db.Deploy.Extensions;

namespace EdFi.Db.Deploy.ScriptPathResolvers
{
    public class ScriptPathResolver : ScriptPathResolverBase
    {
        private readonly EngineType _engineType;
        protected readonly string _standardVersion;
        protected readonly string _extensionVersion;

        public ScriptPathResolver(string path, DatabaseType databaseType, EngineType engineType, 
            string feature = null, string standardVersion = null, string extensionVersion = null)
            : base(path, databaseType, feature)
        {
            _engineType = engineType;
            _standardVersion = standardVersion;
            _extensionVersion = extensionVersion;
        }

        public override string StructureScriptPath()
            => Path.GetFullPath(
                !string.IsNullOrEmpty(_feature)
                    ? Path.Combine(
                        GetVersionFolderPath(),
                        DatabaseConventions.ArtifactsDirectory,
                        _engineType.Directory(),
                        DatabaseConventions.StructureDirectory,
                        DatabaseTypeDirectory(),
                        _feature)
                    : Path.Combine(
                        GetVersionFolderPath(),
                        DatabaseConventions.ArtifactsDirectory,
                        _engineType.Directory(),
                        DatabaseConventions.StructureDirectory,
                        DatabaseTypeDirectory()));

        public override string DataScriptPath()
            => Path.GetFullPath(
                !string.IsNullOrEmpty(_feature)
                    ? Path.Combine(
                        GetVersionFolderPath(),
                        DatabaseConventions.ArtifactsDirectory,
                        _engineType.Directory(),
                        DatabaseConventions.DataDirectory,
                        DatabaseTypeDirectory(),
                        _feature)
                    : Path.Combine(
                        GetVersionFolderPath(),
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

        private string GetVersionFolderPath()
        {
            if (_path.Contains(DatabaseConventions.StandardProject, StringComparison.InvariantCultureIgnoreCase))
            {
                return Path.Combine(_path, DatabaseConventions.StandardFolder, _standardVersion);
            }

            if (_path.Contains(DatabaseConventions.ExtensionPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                var extensionPath = Path.Combine(_path, DatabaseConventions.VersionsFolder, _extensionVersion, DatabaseConventions.StandardFolder, _standardVersion);

                if(Directory.Exists(extensionPath))
                {
                    return extensionPath;
                }

                return Path.Combine(_path, DatabaseConventions.VersionsFolder, DatabaseConventions.DefaultExtensionVersion, DatabaseConventions.StandardFolder, _standardVersion);
            }

            return _path;
        }
    }
}
