// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Db.Deploy.Adapters;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.ScriptPathResolvers;

namespace EdFi.Db.Deploy.DeployJournal
{
    public class ScriptPathInfoProvider : IScriptPathInfoProvider
    {
        private IOptions _options;
        private readonly IFileSystem _fileSystemAdapter;

        public ScriptPathInfoProvider(IFileSystem fileSystemAdapter)
        {
            _fileSystemAdapter = Preconditions.ThrowIfNull(fileSystemAdapter, nameof(fileSystemAdapter));
        }

        public IReadOnlyList<IScriptPathInfo> FindAllScriptsInFileSystem(IOptions options)
        {
            _options = Preconditions.ThrowIfNull(options, nameof(options));

            var scriptPaths = new List<ScriptPathInfo>();

            scriptPaths.AddRange(FindCoreScripts());
            scriptPaths.AddRange(FindLegacyExtensionScripts());
            scriptPaths.AddRange(FindFeatureScripts());
            scriptPaths.AddRange(FindLegacyExtensionFeatureScripts());

            return scriptPaths;
        }

        private IEnumerable<ScriptPathInfo> FindCoreScripts()
        {
            var filePaths = _options.FilePaths.ToList();
            var databaseType = _options.DatabaseType;
            var engineType = _options.Engine;

            var scriptPaths = (from path in filePaths
                               let scriptsPath = GetScriptsPathFromDatabaseDeployPath(path, true)
                               where _fileSystemAdapter.DirectoryExists(scriptsPath)
                               select new ScriptPathInfo(_fileSystemAdapter, path, scriptsPath)).ToList();

            scriptPaths.AddRange(
                from path in filePaths
                let scriptsPath = GetScriptsPathFromDatabaseDeployPath(path, false)
                where _fileSystemAdapter.DirectoryExists(scriptsPath)
                select new ScriptPathInfo(_fileSystemAdapter, path, scriptsPath));

            return scriptPaths;

            string GetScriptsPathFromDatabaseDeployPath(string path, bool isStructureScripts)
            {
                string scriptsPath = GetScriptsPath(new ScriptPathResolver(path, databaseType, engineType), isStructureScripts);

                if (!_fileSystemAdapter.DirectoryExists(scriptsPath) && engineType == EngineType.SqlServer)
                {
                    scriptsPath = GetScriptsPath(new LegacyScriptPathResolver(path, databaseType), isStructureScripts);
                }

                return scriptsPath;
            }
        }

        private IEnumerable<ScriptPathInfo> FindLegacyExtensionScripts()
        {
            if (_options.Engine != EngineType.SqlServer)
            {
                return new List<ScriptPathInfo>();
            }

            var filePaths = _options.FilePaths.ToList();
            var databaseType = _options.DatabaseType;

            var scriptPaths = (from path in filePaths
                               let scriptsPath = GetScriptPathsFromLegacyExtensionsPath(path, true)
                               where _fileSystemAdapter.DirectoryExists(scriptsPath)
                               select new ScriptPathInfo(_fileSystemAdapter,path, scriptsPath)).ToList();

            scriptPaths.AddRange(
                from path in filePaths
                let scriptsPath = GetScriptPathsFromLegacyExtensionsPath(path, false)
                where _fileSystemAdapter.DirectoryExists(scriptsPath)
                select new ScriptPathInfo(_fileSystemAdapter,path, scriptsPath));

            return scriptPaths;

            string GetScriptPathsFromLegacyExtensionsPath(string path, bool isStructureScripts)
            {
                return GetScriptsPath(new LegacyExtensionsScriptPathResolver(path, databaseType), isStructureScripts);
            }
        }

        private IEnumerable<ScriptPathInfo> FindFeatureScripts()
        {
            var filePaths = _options.FilePaths.ToList();
            var features = _options.Features.ToList();
            var databaseType = _options.DatabaseType;
            var engineType = _options.Engine;

            var scriptPaths = (from path in filePaths
                               from feature in features
                               let scriptPath = GetScriptsPathFromFeatureDeployPath(path, feature, true)
                               where _fileSystemAdapter.DirectoryExists(scriptPath)
                               select new ScriptPathInfo(_fileSystemAdapter,path, scriptPath)).ToList();

            scriptPaths.AddRange(
                from path in filePaths
                from feature in features
                let scriptPath = GetScriptsPathFromFeatureDeployPath(path, feature, false)
                where _fileSystemAdapter.DirectoryExists(scriptPath)
                select new ScriptPathInfo(_fileSystemAdapter,path, scriptPath));

            return scriptPaths;

            string GetScriptsPathFromFeatureDeployPath(string path, string feature, bool isStructureScripts)
            {
                string scriptsPath = GetScriptsPath(new ScriptPathResolver(path, databaseType, EngineType.SqlServer, feature), isStructureScripts);

                if (!_fileSystemAdapter.DirectoryExists(scriptsPath) && engineType == EngineType.SqlServer)
                {
                    scriptsPath = GetScriptsPath(new LegacyScriptPathResolver(path, databaseType, feature), isStructureScripts);
                }

                return scriptsPath;
            }
        }

        private IEnumerable<ScriptPathInfo> FindLegacyExtensionFeatureScripts()
        {
            if (_options.Engine != EngineType.SqlServer)
            {
                return new List<ScriptPathInfo>();
            }

            var filePaths = _options.FilePaths.ToList();
            var features = _options.Features.ToList();
            var databaseType = _options.DatabaseType;

            var scriptPaths = (from path in filePaths
                               from feature in features
                               let scriptsPath = GetScriptPathsFromLegacyExtensionFeaturesPath(path, feature, true)
                               where _fileSystemAdapter.DirectoryExists(scriptsPath)
                               select new ScriptPathInfo(_fileSystemAdapter,path, scriptsPath)).ToList();

            scriptPaths.AddRange(
                from path in filePaths
                from feature in features
                let scriptsPath = GetScriptPathsFromLegacyExtensionFeaturesPath(path, feature, false)
                where _fileSystemAdapter.DirectoryExists(scriptsPath)
                select new ScriptPathInfo(_fileSystemAdapter, path, scriptsPath));

            return scriptPaths;

            string GetScriptPathsFromLegacyExtensionFeaturesPath(string path, string feature, bool isStructureScripts)
            {
                return GetScriptsPath(new LegacyExtensionsScriptPathResolver(path, databaseType, feature), isStructureScripts);
            }
        }

        private static string GetScriptsPath(IScriptPathResolver fileInfoProvider, bool isStructureScripts)
            => isStructureScripts
                ? fileInfoProvider.StructureScriptPath()
                : fileInfoProvider.DataScriptPath();
    }
}