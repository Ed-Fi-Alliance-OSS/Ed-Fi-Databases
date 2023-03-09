// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;

namespace EdFi.Db.Deploy.Specifications
{
    public class ExtensionVersionSpecification : OptionsSpecification
    {
        public override bool IsSatisfiedBy(IOptions obj)
        {
            Preconditions.ThrowIfNull(obj, nameof(obj));

            var extensionPaths = obj.FilePaths.Where(x =>
                x.Contains(DatabaseConventions.ExtensionPrefix, StringComparison.InvariantCultureIgnoreCase));

            if (extensionPaths.Count() == 0)
            {
                return true;
            }

            if (string.IsNullOrEmpty(obj.StandardVersion) || string.IsNullOrEmpty(obj.ExtensionVersion))
            {
                ErrorMessages.Add($"Standard and Extension Versions are required to run artifacts from the extension projects.");
                return false;
            }

            foreach(var extensionPath in extensionPaths)
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
                    var extensionDefaultVersionPath = Path.GetFullPath(
                                Path.Combine(
                                    extensionPath,
                                    DatabaseConventions.VersionsFolder,
                                    DatabaseConventions.DefaultExtensionVersion,
                                    DatabaseConventions.StandardFolder,
                                    obj.StandardVersion));

                    if(!Directory.Exists(extensionDefaultVersionPath))
                    {
                        ErrorMessages.Add($"Extension Version directory {extensionVersionPath} does not exist");
                        ErrorMessages.Add($"Default Extension Version directory {extensionDefaultVersionPath} does not exist");
                    }
                }
            }

            return !ErrorMessages.Any();
        }
    }
}
