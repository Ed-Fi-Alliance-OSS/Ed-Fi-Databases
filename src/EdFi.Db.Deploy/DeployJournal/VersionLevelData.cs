// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;

namespace EdFi.Db.Deploy.DeployJournal
{
    public class VersionLevelData
    {
        private const string OdsScriptSource = "ED-FI-ODS";
        private const string OdsImplementationScriptSource = "ED-FI-ODS-IMPLEMENTATION";
        private const string StructureScriptType = "STRUCTURE";
        private const string DataScriptType = "DATA";
        private const string EdFiDatabaseType = "EDFI";
        private const string ChangesScriptSubType = "CHANGES";

        private const int EdFiOdsStructureLegacyMaxLevel = 1030;
        private const int EdFiOdsDataLegacyMaxLevel = 1030;
        private const int EdFiOdsImplementationDataLegacyMaxLevel = 1;
        private const int EdFiOdsMergedDataLegacyMaxLevel = 1040;

        private readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        public readonly IDictionary<string, DatabaseVersionLevel> ExtensionStructureVersionLevelByExtensionName = new Dictionary<string, DatabaseVersionLevel>();
        public readonly IDictionary<string, DatabaseVersionLevel> ExtensionDataVersionLevelByExtensionName = new Dictionary<string, DatabaseVersionLevel>();
        public readonly IDictionary<string, DatabaseVersionLevel> ExtensionChangeQueriesStructureVersionLevelByExtensionName = new Dictionary<string, DatabaseVersionLevel>();
        public readonly IDictionary<string, DatabaseVersionLevel> ExtensionChangeQueriesDataVersionLevelByExtensionName = new Dictionary<string, DatabaseVersionLevel>();

        public VersionLevelData(IReadOnlyCollection<DatabaseVersionLevel> databaseVersionLevels)
        {
            SetOdsStructureLevel();
            SetOdsDataLevel();
            SetOdsImplementationDataLevel();
            SetExtensionStructureVersionLevelsByExtensionName();
            SetExtensionDataVersionLevelsByExtensionName();
            SetExtensionChangeQueriesStructureVersionLevelsByExtensionName();
            SetExtensionChangeQueriesDataVersionLevelsByExtensionName();
            SetOdsStructureChangesLevel();
            ResetOdsDataLevelDueToScriptThatMovedFromImplementationToOds();

            void SetOdsStructureLevel()
            {
                OdsStructureLevel = databaseVersionLevels
                    .Where(
                        dvl => IsAnOdsRepositoryScript(dvl) &&
                               IsAStructureScript(dvl) &&
                               IsForTheODSDatabase(dvl) &&
                               IsNotAFeatureScript(dvl))
                    .Select(dvl => dvl.VersionLevel)
                    .SingleOrDefault();
            }

            void SetOdsDataLevel()
            {
                OdsDataLevel = databaseVersionLevels
                    .Where(
                        dvl => IsAnOdsRepositoryScript(dvl) &&
                               IsADataScript(dvl) &&
                               IsForTheODSDatabase(dvl) &&
                               IsNotAFeatureScript(dvl))
                    .Select(dvl => dvl.VersionLevel)
                    .SingleOrDefault();
            }

            void SetOdsImplementationDataLevel()
            {
                OdsImplementationDataLevel = databaseVersionLevels
                    .Where(
                        dvl => IsAnOdsImplementationRepositoryScript(dvl) &&
                               IsADataScript(dvl) &&
                               IsForTheODSDatabase(dvl) &&
                               IsNotAFeatureScript(dvl))
                    .Select(dvl => dvl.VersionLevel)
                    .SingleOrDefault();
            }

            void SetOdsStructureChangesLevel()
            {
                EdFiOdsStructureChangesLevel = databaseVersionLevels
                    .Where(
                        dvl => IsAnOdsRepositoryScript(dvl) &&
                               IsAStructureScript(dvl) &&
                               IsForTheODSDatabase(dvl) &&
                               IsAChangeQueriesScript(dvl))
                    .Select(dvl => dvl.VersionLevel)
                    .SingleOrDefault();
            }

            void SetExtensionStructureVersionLevelsByExtensionName()
            {
                databaseVersionLevels
                    .Where(IsNotACoreScript)
                    .Where(IsAStructureScript)
                    .Where(x => !IsAChangeQueriesScript(x))
                    .ToList()
                    .ForEach(x => { ExtensionStructureVersionLevelByExtensionName.Add(x.ScriptSource, x); });
            }

            void SetExtensionDataVersionLevelsByExtensionName()
            {
                databaseVersionLevels
                    .Where(IsNotACoreScript)
                    .Where(IsADataScript)
                    .Where(x => !IsAChangeQueriesScript(x))
                    .ToList()
                    .ForEach(x => { ExtensionDataVersionLevelByExtensionName.Add(x.ScriptSource, x); });
            }

            void SetExtensionChangeQueriesStructureVersionLevelsByExtensionName()
            {
                databaseVersionLevels
                    .Where(IsNotACoreScript)
                    .Where(IsAStructureScript)
                    .Where(IsAChangeQueriesScript)
                    .ToList()
                    .ForEach(x => { ExtensionChangeQueriesStructureVersionLevelByExtensionName.Add(x.ScriptSource, x); });
            }

            void SetExtensionChangeQueriesDataVersionLevelsByExtensionName()
            {
                databaseVersionLevels
                    .Where(IsNotACoreScript)
                    .Where(IsADataScript)
                    .Where(IsAChangeQueriesScript)
                    .ToList()
                    .ForEach(x => { ExtensionChangeQueriesDataVersionLevelByExtensionName.Add(x.ScriptSource, x); });
            }

            void ResetOdsDataLevelDueToScriptThatMovedFromImplementationToOds()
            {
                // v3.2.0 has 1030 version level for Ed-Fi-ODS data scripts
                // and 0001 version level for Ed-Fi-ODS-Implementation data scripts
                // v3.3 moves the single implementation data script to Standards data scripts with 1040 version level
                if (OdsImplementationDataLevel == EdFiOdsImplementationDataLegacyMaxLevel
                    && OdsDataLevel == EdFiOdsDataLegacyMaxLevel)
                {
                    OdsDataLevel = 1040;
                }

                foreach (var extension in ExtensionChangeQueriesDataVersionLevelByExtensionName)
                {
                    if (!ExtensionChangeQueriesStructureVersionLevelByExtensionName.ContainsKey(extension.Key))
                    {
                        _logger.Warn($"Change Query Extension {extension.Key} was not found as {OdsScriptSource} script source but there is a {DataScriptType} script type entry.");
                    }
                }
            }
        }

        public int? OdsStructureLevel { get; set; }

        public int? OdsDataLevel { get; set; }

        public int? OdsImplementationDataLevel { get; set; }

        public int? EdFiOdsStructureChangesLevel { get; set; }

        public static bool IsAnOdsImplementationRepositoryScript(DatabaseVersionLevel x) => x?.ScriptSource?.ToUpper() == OdsImplementationScriptSource;

        public static bool IsAnOdsRepositoryScript(DatabaseVersionLevel x) => x?.ScriptSource?.ToUpper() == OdsScriptSource;

        public static bool IsForTheODSDatabase(DatabaseVersionLevel x) => x?.DatabaseType?.ToUpper() == EdFiDatabaseType;

        public static bool IsNotACoreScript(DatabaseVersionLevel x) => !IsAnOdsRepositoryScript(x) && !IsAnOdsImplementationRepositoryScript(x);

        public static bool IsAChangeQueriesScript(DatabaseVersionLevel x) => x?.SubType?.ToUpper() == ChangesScriptSubType;

        public static bool IsNotAFeatureScript(DatabaseVersionLevel x) => string.IsNullOrWhiteSpace(x?.SubType);

        public static bool IsAStructureScript(DatabaseVersionLevel x) => x?.ScriptType?.ToUpper() == StructureScriptType;

        public static bool IsADataScript(DatabaseVersionLevel x) => x?.ScriptType?.ToUpper() == DataScriptType;
    }
}
