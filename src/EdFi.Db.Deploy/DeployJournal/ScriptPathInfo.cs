// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EdFi.Db.Deploy.Adapters;
using EdFi.Db.Deploy.Helpers;

namespace EdFi.Db.Deploy.DeployJournal
{
    public interface IScriptPathInfo
    {
        string ParentPath { get; }
        string ScriptPath { get; }
        IReadOnlyList<ScriptFileVersionLevel> GetAllScriptFiles();
    }

    public class ScriptPathInfo : IScriptPathInfo
    {
        private readonly IFileSystem _fileSystemAdapter;

        public string ParentPath { get; }

        public string ScriptPath { get; }

        public ScriptPathInfo(IFileSystem fileSystemAdapter, string parentPath, string scriptPath)
        {
            _fileSystemAdapter = Preconditions.ThrowIfNull(fileSystemAdapter, nameof(fileSystemAdapter));

            ParentPath = Path.GetFullPath(parentPath);
            ScriptPath = Path.GetFullPath(scriptPath);
        }

        public IReadOnlyList<ScriptFileVersionLevel> GetAllScriptFiles()
        {
            string scriptNamePrefix = ScriptNamePrefix();

            var allScriptNames = _fileSystemAdapter.GetFiles(ScriptPath, "*.sql", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName);

            return allScriptNames.Select(scriptName => new ScriptFileVersionLevel(scriptNamePrefix, scriptName)).ToList();

            string ScriptNamePrefix()
            {
                string parentFolder = ParentFolder();

                string scriptFileNamePrefix = ScriptPath.Remove(0, parentFolder.Length);

                scriptFileNamePrefix = scriptFileNamePrefix.Replace("\\", ".");

                if (scriptFileNamePrefix.StartsWith("."))
                    scriptFileNamePrefix = scriptFileNamePrefix.Remove(0, 1);

                return scriptFileNamePrefix;
            }

            string ParentFolder()
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
        }
    }
}
