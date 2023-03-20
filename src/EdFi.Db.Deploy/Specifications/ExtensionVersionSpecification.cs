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
    public class ExtensionVersionSpecification : OptionsSpecification
    {
        private readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        public override bool IsSatisfiedBy(IOptions obj)
        {
            Preconditions.ThrowIfNull(obj, nameof(obj));

            var extensionPaths = obj.FilePaths.Where(x =>
                x.Contains(DatabaseConventions.ExtensionPrefix, StringComparison.InvariantCultureIgnoreCase));

            if (extensionPaths.Count() == 0)
            {
                _logger.Debug($"No extension paths were found.");
                return true;
            }

            if (string.IsNullOrEmpty(obj.StandardVersion) || string.IsNullOrEmpty(obj.ExtensionVersion))
            {
                _logger.Debug($"StandardVersion and ExtensionVersion parameters are required to run artifacts from the extension projects.");
                ErrorMessages.Add($"StandardVersion and ExtensionVersion parameters are required to run artifacts from the extension projects.");
                return false;
            }

            foreach (var extensionPath in extensionPaths)
            {
                var extensionVersionPath = Path.GetFullPath(
                                Path.Combine(
                                    extensionPath,
                                    DatabaseConventions.VersionsFolder,
                                    obj.ExtensionVersion,
                                    DatabaseConventions.StandardFolder,
                                    obj.StandardVersion));

                if (!Directory.Exists(extensionVersionPath))
                {
                    _logger.Debug($"Extension {extensionPath} has no extension version {obj.ExtensionVersion} folder.");
                    var extensionDefaultVersionPath = Path.GetFullPath(
                                Path.Combine(
                                    extensionPath,
                                    DatabaseConventions.VersionsFolder,
                                    DatabaseConventions.DefaultExtensionVersion,
                                    DatabaseConventions.StandardFolder,
                                    obj.StandardVersion));

                    if (!Directory.Exists(extensionDefaultVersionPath))
                    {
                        _logger.Debug($"Extension {extensionPath} has no default extension version {DatabaseConventions.DefaultExtensionVersion} folder.");
                        ErrorMessages.Add($"Extension Version directory {extensionVersionPath} does not exist");
                        ErrorMessages.Add($"Default Extension Version directory {extensionDefaultVersionPath} does not exist");
                    }
                }
            }

            return !ErrorMessages.Any();
        }
    }
}
