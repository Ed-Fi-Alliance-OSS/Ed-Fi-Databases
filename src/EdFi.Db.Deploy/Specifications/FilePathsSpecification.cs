// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using System.Linq;
using System.Reflection;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using log4net;

namespace EdFi.Db.Deploy.Specifications
{
    public class FilePathsSpecification : OptionsSpecification
    {
        private readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        public override bool IsSatisfiedBy(IOptions obj)
        {
            Preconditions.ThrowIfNull(obj, nameof(obj));

            if (!obj.FilePaths.Any())
            {
                ErrorMessages.Add("No file paths are found");
            }

            foreach (string path in obj.FilePaths)
            {
                if (!Directory.Exists(path))
                {
                    ErrorMessages.Add($"Base directory {path} does not exist");
                }
            }

            return !ErrorMessages.Any();
        }
    }
}
