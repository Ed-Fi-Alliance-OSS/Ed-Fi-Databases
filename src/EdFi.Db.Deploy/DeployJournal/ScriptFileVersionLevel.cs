// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Text.RegularExpressions;

namespace EdFi.Db.Deploy.DeployJournal
{
    public class ScriptFileVersionLevel
    {
        public string ScriptName { get; }

        public int VersionLevel { get; }

        public ScriptFileVersionLevel(string prefixName, string fileName)
        {
            ScriptName = $"{prefixName}.{fileName}";

            VersionLevel = ExtractScriptNumberFromFileName();

            int ExtractScriptNumberFromFileName()
            {
                var regexMatch = Regex.Match(fileName, @"^(\d{4})-.+\.sql");
                if (!regexMatch.Success)
                {
                    throw new InvalidOperationException($"Script name {fileName} is not named in the expected format: ####-*.sql.  Ensure that the correct scripts have been referenced, and that the Ed-Fi-ODS build conventions have not changed");
                }
                return int.Parse(regexMatch.Groups[1].Value);
            }
        }
    }
}
