// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EdFi.Db.Deploy.Extensions;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;
using log4net;

namespace EdFi.Db.Deploy.Specifications
{
    public class FeaturesSpecification : OptionsSpecification
    {
        private readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        public override bool IsSatisfiedBy(IOptions obj)
        {
            Preconditions.ThrowIfNull(obj, nameof(obj));

            if (!obj.Features.Any())
            {
                return true;
            }

            var featureExpression = new Regex(@"\d{4}.*?");

            if (obj.Features.Any(f => featureExpression.IsMatch(f)))
            {
                ErrorMessages.Add("Feature names must not start with four digits");
            }

            string engineTypeDirectory = obj.Engine.Directory();

            // to assist in debugging
            if (obj.FilePaths.Any())
            {
                foreach (string feature in obj.Features)
                {
                    foreach (string path in obj.FilePaths)
                    {
                        TestFeatureDirectory(path, DatabaseConventions.StructureDirectory, feature);

                        TestFeatureDirectory(path, DatabaseConventions.DataDirectory, feature);
                    }
                }
            }

            return !ErrorMessages.Any();

            void TestFeatureDirectory(string path, string structureOrDataDirectory, string feature)
            {
                string databaseTypeDirectory = obj.DatabaseType.Directory(ArtifactsFolderStructureType.NewVersion);

                string featurePath = Path.GetFullPath(
                    Path.Combine(
                        path,
                        DatabaseConventions.ArtifactsDirectory,
                        engineTypeDirectory,
                        structureOrDataDirectory,
                        databaseTypeDirectory,
                        feature));

                if (!Directory.Exists(featurePath))
                {
                    _logger.Debug($"Feature {structureOrDataDirectory} {feature} does not exist in path {path}");

                    databaseTypeDirectory = obj.DatabaseType.Directory(ArtifactsFolderStructureType.LegacyVersion);

                    featurePath = Path.GetFullPath(
                        Path.Combine(
                            path,
                            DatabaseConventions.DatabaseDirectory,
                            structureOrDataDirectory,
                            databaseTypeDirectory,
                            feature));

                    if (!Directory.Exists(featurePath))
                    {
                        _logger.Debug($"Feature {structureOrDataDirectory} {feature} does not exist in path {path}");
                    }

                    // If config.Path test fails for v3.3, engine should not be PostgreSql
                    // only if folder exists
                    if (!obj.AreFeaturesValidForLegacyDatabaseDirectoryStructure())
                    {
                        _logger.Debug("Currently only SqlServer is supported to deploy features for Ed-Fi ODS/API.");
                    }
                }
            }
        }
    }
}
