// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Dapper.Contrib.Extensions;

namespace EdFi.Db.Deploy.DeployJournal
{
    public class DatabaseVersionLevel
    {
        public string ScriptSource { get; set; }
        public string ScriptType { get; set; }
        public string DatabaseType { get; set; }
        public string SubType { get; set; }
        public int VersionLevel { get; set; }
    }
}
