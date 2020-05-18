// SPDX-License-Identifier: Apache-2.0
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
    public class LegacyScriptPathResolverTests
    {
        private const string OdsFolder = @"c:\edfi\ed-fi-ods";

        public class When_resolving_the_path_using_the_legacy_folder_structure_in_the_ods : TestFixtureBase
        {
            private LegacyScriptPathResolver _sut;
            private string _dataScriptPath;
            private string _structureScriptPath;

            protected override void Arrange()
            {
                _sut = new LegacyScriptPathResolver(OdsFolder, DatabaseType.ODS);
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
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.DataDirectory,
                        "EdFi"));

            [Test]
            public void Should_have_a_not_null_structure_script_path() => _structureScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_structure_script_path()
                => _structureScriptPath.ShouldBe(
                    Path.Combine(
                        OdsFolder,
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.StructureDirectory,
                        "EdFi"));
        }

        public class When_resolving_the_path_having_a_feature_using_the_legacy_folder_structure_in_the_ods : TestFixtureBase
        {
            private LegacyScriptPathResolver _sut;
            private string _dataScriptPath;
            private string _structureScriptPath;

            protected override void Arrange()
            {
                _sut = new LegacyScriptPathResolver(OdsFolder, DatabaseType.ODS, "Feature");
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
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.DataDirectory,
                        "EdFi",
                        "Feature"));

            [Test]
            public void Should_have_a_not_null_structure_script_path() => _structureScriptPath.ShouldNotBeNull();

            [Test]
            public void Should_have_a_valid_structure_script_path()
                => _structureScriptPath.ShouldBe(
                    Path.Combine(
                        OdsFolder,
                        DatabaseConventions.DatabaseDirectory,
                        DatabaseConventions.StructureDirectory,
                        "EdFi",
                        "Feature"));
        }
    }
}
