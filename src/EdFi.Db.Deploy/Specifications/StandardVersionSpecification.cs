// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using log4net;

namespace EdFi.Db.Deploy.Specifications
{
    public class StandardVersionSpecification : OptionsSpecification
    {
        private readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        public override bool IsSatisfiedBy(IOptions obj)
        {
            Preconditions.ThrowIfNull(obj, nameof(obj));

            var standardProjectPath = obj.FilePaths.FirstOrDefault(x =>
                x.Contains(DatabaseConventions.StandardProject, StringComparison.InvariantCultureIgnoreCase));

            if (standardProjectPath == null)
            {
                _logger.Debug($"No standard project path was found.");
                return true;
            }

            if (string.IsNullOrEmpty(obj.StandardVersion))
            {
                _logger.Debug($"Standard Version is required to run artifacts from the {DatabaseConventions.StandardProject} project.");
                ErrorMessages.Add($"Standard Version is required to run artifacts from the {DatabaseConventions.StandardProject} project.");
                return false;
            }

            var standardVersionPath = Path.GetFullPath(
                Path.Combine(
                    standardProjectPath,
                    DatabaseConventions.StandardFolder,
                    obj.StandardVersion));

            if (!Directory.Exists(standardVersionPath))
            {
                _logger.Debug($"Standard Version directory {standardVersionPath} does not exist.");
                ErrorMessages.Add($"Standard Version directory {standardVersionPath} does not exist.");
            }

            return !ErrorMessages.Any();
        }
    }
}
