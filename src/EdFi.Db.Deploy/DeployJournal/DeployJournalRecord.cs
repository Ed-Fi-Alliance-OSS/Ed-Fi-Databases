// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Dapper.Contrib.Extensions;

namespace EdFi.Db.Deploy.DeployJournal
{
    [Table("DeployJournal")]
    public class DeployJournalRecord
    {
        public string ScriptName { get; set; }

        public DateTime Applied { get; set; }

        public override bool Equals(object obj)
        {
            return obj != null && (obj is DeployJournalRecord other) && other.ScriptName == ScriptName;
        }

        public override int GetHashCode()
        {
            return ScriptName.GetHashCode();
        }
    }
}
