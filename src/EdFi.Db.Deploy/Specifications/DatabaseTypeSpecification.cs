// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;

namespace EdFi.Db.Deploy.Specifications
{
    public class DatabaseTypeSpecification : OptionsSpecification
    {
        private readonly List<DatabaseType> _validOptions = new List<DatabaseType>
        {
            DatabaseType.Admin,
            DatabaseType.Security,
            DatabaseType.ODS
        };

        public override bool IsSatisfiedBy(IOptions obj)
        {
            Preconditions.ThrowIfNull(obj, nameof(obj));

            if (!_validOptions.Contains(obj.DatabaseType))
            {
                ErrorMessages.Add($"DatabaseType must be one of {string.Join(',', _validOptions.Select(x => x.ToString()))}");
            }

            return !ErrorMessages.Any();
        }
    }
}
