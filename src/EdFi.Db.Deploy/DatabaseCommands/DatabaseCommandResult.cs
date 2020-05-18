// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Db.Deploy.UpgradeEngineStrategies
{
    public class DatabaseCommandResult
    {
        public bool RequiresUpgrade { get; set; }

        public Exception Exception { get; set; }

        public bool IsSuccessful { get; set; }

        public static DatabaseCommandResult Create(IList<DatabaseCommandResult> commandResults)
        {
            return new DatabaseCommandResult
            {
                IsSuccessful = commandResults.All(x => x.IsSuccessful),
                RequiresUpgrade = commandResults.Any(x => x.RequiresUpgrade),
                Exception = commandResults.Where(x => x.Exception != null)
                    .Select(x => x.Exception)
                    .FirstOrDefault()
            };
        }
    }
}
