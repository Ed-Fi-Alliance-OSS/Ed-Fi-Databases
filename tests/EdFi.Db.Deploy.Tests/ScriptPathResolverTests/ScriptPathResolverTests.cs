﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using EdFi.Db.Deploy.Extensions;
using EdFi.Db.Deploy.ScriptPathResolvers;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests.ScriptPathResolverTests
{
    [TestFixture]
    public class ScriptPathResolverTests
    {
        private static string StandardVersion = "4.0.0";
        private static string ExtensionVersion = "1.1.0";
        private static string OdsFolder = Path.Combine(TestFixtureBase.RootPath, "edfi", "ed-fi-ods");
        private static string StandardFolder = Path.Combine(TestFixtureBase.RootPath, "edfi", "ed-fi-ods", DatabaseConventions.StandardProject);
        private static string ExtensionFolder = Path.Combine(TestFixtureBase.RootPath, "edfi", "ed-fi-extensions", $"{DatabaseConventions.ExtensionPrefix}Homograph");

        public class When_resolving_the_path_using_the_new_artifacts_folder_structure_in_the_ods : TestFixtureBase
        {
            private ScriptPathResolver _sut;
            private string _dataScriptPath;
            private string _structureScriptPath;

            protected override void Arrange()
            {
                _sut = new ScriptPathResolver(OdsFolder, DatabaseType.Admin, EngineType.SqlServer);
            }

            protected override void Act()
            {
                _dataScriptPath = _sut.DataScriptPath();
                _structureScriptPath = _sut.StructureScriptPath();
            }

            [Test]
            public void Should_have_a_not_null_data_script_path() => _dataScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_data_script_path()
                => _dataScriptPath.ShouldBe(
                    Path.Combine(
                        OdsFolder,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.DataDirectory,
                        "Admin"));

            [Test]
            public void Should_have_a_not_null_structure_script_path() => _structureScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_structure_script_path()
                => _structureScriptPath.ShouldBe(
                    Path.Combine(
                        OdsFolder,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.StructureDirectory,
                        "Admin"));
        }

        public class When_resolving_the_path_of_a_feature_using_the_new_artifacts_folder_structure_in_the_ods : TestFixtureBase
        {
            private ScriptPathResolver _sut;
            private string _dataScriptPath;
            private string _structureScriptPath;

            protected override void Arrange()
            {
                _sut = new ScriptPathResolver(OdsFolder, DatabaseType.Admin, EngineType.SqlServer, "Feature");
            }

            protected override void Act()
            {
                _dataScriptPath = _sut.DataScriptPath();
                _structureScriptPath = _sut.StructureScriptPath();
            }

            [Test]
            public void Should_have_a_not_null_data_script_path() => _dataScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_data_script_path()
                => _dataScriptPath.ShouldBe(
                    Path.Combine(
                        OdsFolder,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.DataDirectory,
                        "Admin",
                        "Feature"));

            [Test]
            public void Should_have_a_not_null_structure_script_path() => _structureScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_structure_script_path()
                => _structureScriptPath.ShouldBe(
                    Path.Combine(
                        OdsFolder,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.StructureDirectory,
                        "Admin",
                        "Feature"));
        }

        // note the standard scripts (e.g. data model) for the 3.3 release will be treated like an extension
        public class When_resolving_the_path_of_an_extension_using_the_new_artifacts_folder_structure : TestFixtureBase
        {
            private ScriptPathResolver _sut;
            private string _dataScriptPath;
            private string _structureScriptPath;

            protected override void Arrange()
            {
                _sut = new ScriptPathResolver(StandardFolder, DatabaseType.ODS, EngineType.PostgreSql, standardVersion: StandardVersion);
            }

            protected override void Act()
            {
                _dataScriptPath = _sut.DataScriptPath();
                _structureScriptPath = _sut.StructureScriptPath();
            }

            [Test]
            public void Should_have_a_not_null_data_script_path() => _dataScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_data_script_path()
                => _dataScriptPath.ShouldBe(
                    Path.Combine(
                        StandardFolder,
                        DatabaseConventions.StandardFolder,
                        StandardVersion,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.PostgreSql.Directory(),
                        DatabaseConventions.DataDirectory,
                        "Ods"));

            [Test]
            public void Should_have_a_not_null_structure_script_path() => _structureScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_structure_script_path()
                => _structureScriptPath.ShouldBe(
                    Path.Combine(
                        StandardFolder,
                        DatabaseConventions.StandardFolder,
                        StandardVersion,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.PostgreSql.Directory(),
                        DatabaseConventions.StructureDirectory,
                        "Ods"));
        }

        public class When_resolving_the_path_of_feature_within_an_extension_using_the_new_artifacts_folder_structure : TestFixtureBase
        {
            private ScriptPathResolver _sut;
            private string _dataScriptPath;
            private string _structureScriptPath;

            protected override void Arrange()
            {
                _sut = new ScriptPathResolver(StandardFolder, DatabaseType.ODS, EngineType.SqlServer, "Feature", standardVersion: StandardVersion);
            }

            protected override void Act()
            {
                _dataScriptPath = _sut.DataScriptPath();
                _structureScriptPath = _sut.StructureScriptPath();
            }

            [Test]
            public void Should_have_a_not_null_data_script_path() => _dataScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_data_script_path()
                => _dataScriptPath.ShouldBe(
                    Path.Combine(
                        StandardFolder,
                        DatabaseConventions.StandardFolder,
                        StandardVersion,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.DataDirectory,
                        "Ods",
                        "Feature"));

            [Test]
            public void Should_have_a_not_null_structure_script_path() => _structureScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_structure_script_path()
                => _structureScriptPath.ShouldBe(
                    Path.Combine(
                        StandardFolder,
                        DatabaseConventions.StandardFolder,
                        StandardVersion,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.StructureDirectory,
                        "Ods",
                        "Feature"));
        }

        public class When_resolving_the_path_an_extension_using_standard_and_extension_versions : TestFixtureBase
        {
            private ScriptPathResolver _sut;
            private string _dataScriptPath;
            private string _structureScriptPath;

            protected override void Arrange()
            {
                _sut = new ScriptPathResolver(ExtensionFolder, DatabaseType.ODS, EngineType.SqlServer, standardVersion: StandardVersion, extensionVersion: ExtensionVersion);
            }

            protected override void Act()
            {
                _dataScriptPath = _sut.DataScriptPath();
                _structureScriptPath = _sut.StructureScriptPath();
            }

            [Test]
            public void Should_have_a_not_null_data_script_path() => _dataScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_data_script_path()
                => _dataScriptPath.ShouldBe(
                    Path.Combine(
                        ExtensionFolder,
                        DatabaseConventions.VersionsFolder,
                        "1.0.0",
                        DatabaseConventions.StandardFolder,
                        StandardVersion,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.DataDirectory,
                        "Ods"));

            [Test]
            public void Should_have_a_not_null_structure_script_path() => _structureScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_structure_script_path()
                => _structureScriptPath.ShouldBe(
                    Path.Combine(
                        ExtensionFolder,
                        DatabaseConventions.VersionsFolder,
                        "1.0.0",
                        DatabaseConventions.StandardFolder,
                        StandardVersion,
                        DatabaseConventions.ArtifactsDirectory,
                        EngineType.SqlServer.Directory(),
                        DatabaseConventions.StructureDirectory,
                        "Ods"));
        }
    }
}
