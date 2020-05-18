// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Db.Deploy.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Db.Deploy.DeployJournal
{
    public class DeployJournalDataProvider
    {
        private const string OdsStructure = "EdFi.Ods.Standard.Artifacts.MsSql.Structure.Ods";
        private const string OdsStructureChangeQueries = "EdFi.Ods.Standard.Artifacts.MsSql.Structure.Ods.Changes";
        private const string OdsData = "EdFi.Ods.Standard.Artifacts.MsSql.Data.Ods";

        private readonly VersionLevelData _versionLevelData;
        private readonly IReadOnlyList<ScriptFileVersionLevel> _scriptFileVersionLevels;

        public DeployJournalDataProvider(VersionLevelData versionLevelData, IReadOnlyList<ScriptFileVersionLevel> scriptFileVersionLevels)
        {
            _versionLevelData = Preconditions.ThrowIfNull(versionLevelData, nameof(versionLevelData));
            _scriptFileVersionLevels = Preconditions.ThrowIfNull(scriptFileVersionLevels, nameof(scriptFileVersionLevels));
        }

        public IReadOnlyList<string> LookupOdsStructureScripts()
        {
            return LookupScriptsByScriptType(OdsStructure, _versionLevelData.OdsStructureLevel ?? 0);
        }

        public IReadOnlyList<string> LookupOdsStructureChangeQueryScripts()
        {
            return LookupScriptsByScriptType(OdsStructureChangeQueries, _versionLevelData.EdFiOdsStructureChangesLevel ?? 0);
        }

        public IReadOnlyList<string> LookupOdsDataScripts()
        {
            return LookupScriptsByScriptType(OdsData, _versionLevelData.OdsDataLevel ?? 0);
        }

        public IReadOnlyList<string> LookupExtensionStructureScripts(string extensionName)
        {
            _versionLevelData.ExtensionStructureVersionLevelByExtensionName.TryGetValue(extensionName, out DatabaseVersionLevel extensionVersionLevel);
            var scripts = LookupScriptsByScriptType($"EdFi.Ods.Extensions.{extensionName}.Artifacts.MsSql.Structure.Ods", extensionVersionLevel?.VersionLevel ?? 0);

            _versionLevelData.ExtensionChangeQueriesStructureVersionLevelByExtensionName.TryGetValue(extensionName, out DatabaseVersionLevel extensionVersionLevelForChangeQueries);
            var changeScripts = LookupScriptsByScriptType($"EdFi.Ods.Extensions.{extensionName}.Artifacts.MsSql.Structure.Ods.Changes", extensionVersionLevelForChangeQueries?.VersionLevel ?? 0);

            return scripts.Concat(changeScripts).ToList();
        }

        public IReadOnlyList<string> LookupExtensionDataScripts(string extensionName)
        {
            _versionLevelData.ExtensionDataVersionLevelByExtensionName.TryGetValue(extensionName, out DatabaseVersionLevel extensionVersionLevel);
            var scripts = LookupScriptsByScriptType($"EdFi.Ods.Extensions.{extensionName}.Artifacts.MsSql.Data.Ods", extensionVersionLevel?.VersionLevel ?? 0);

            _versionLevelData.ExtensionChangeQueriesDataVersionLevelByExtensionName.TryGetValue(extensionName, out DatabaseVersionLevel extensionVersionLevelForChangeQueries);
            var changeScripts = LookupScriptsByScriptType($"EdFi.Ods.Extensions.{extensionName}.Artifacts.MsSql.Data.Ods.Changes", extensionVersionLevelForChangeQueries?.VersionLevel ?? 0);

            return scripts.Concat(changeScripts).ToList();
        }

        private IReadOnlyList<string> LookupScriptsByScriptType(string scriptSource, int versionLevel)
        {
            const string Changes = ".Changes";

            if (versionLevel < 1)
            {
                return new List<string>();
            }

            var scriptFiles = _scriptFileVersionLevels
                .Where(script => script.ScriptName.StartsWith(scriptSource, StringComparison.InvariantCultureIgnoreCase))
                .Where(script => script.VersionLevel <= versionLevel);

            if (!scriptSource.Contains(Changes))
            {
                scriptFiles = scriptFiles
                    .Where(script => !script.ScriptName.Contains(Changes));
            }

            return scriptFiles
                .OrderBy(script => script.VersionLevel)
                .Select(script => script.ScriptName)
                .ToList();
        }
    }
}
