// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Db.Deploy.Extensions;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests.Extensions
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class DatabaseTypeExtensionTests
    {
        public class Given_artifacts_folder_structure_is_NewVersion
        {
            private const ArtifactsFolderStructureType ArtifactsFolderStructureType =
                Deploy.ArtifactsFolderStructureType.NewVersion;

            public class When_database_type_has_extension_directory : TestFixtureBase
            {
                private Exception _ex;
                private string _odsExtensionDirectory;
                private string _adminExtensionDirectory;
                private string _securityExtensionDirectory;

                protected override void Arrange()
                {
                    _ex = null;
                }

                protected override void Act()
                {
                    try
                    {
                        _odsExtensionDirectory = DatabaseType.ODS.Directory(ArtifactsFolderStructureType);
                        _adminExtensionDirectory = DatabaseType.Admin.Directory(ArtifactsFolderStructureType);
                        _securityExtensionDirectory = DatabaseType.Security.Directory(ArtifactsFolderStructureType);
                    }
                    catch (Exception e)
                    {
                        _ex = e;
                    }
                }

                [Test]
                public void Should_not_throw_exception()
                {
                    _ex.ShouldBeNull();
                }

                [Test]
                public void Should_Ods_be_Ods_directory()
                {
                    _odsExtensionDirectory.ShouldBe("Ods");
                }

                [Test]
                public void Should_Admin_be_Admin_directory()
                {
                    _adminExtensionDirectory.ShouldBe("Admin");
                }

                [Test]
                public void Should_Security_be_Security_directory()
                {
                    _securityExtensionDirectory.ShouldBe("Security");
                }
            }
        }

        public class Given_artifacts_folder_structure_is_LegacyVersion
        {
            private const ArtifactsFolderStructureType ArtifactsFolderStructureType =
                Deploy.ArtifactsFolderStructureType.LegacyVersion;

            public class When_database_type_has_extension_directory : TestFixtureBase
            {
                private Exception _ex;
                private string _odsExtensionDirectory;
                private string _adminExtensionDirectory;
                private string _securityExtensionDirectory;

                protected override void Arrange()
                {
                    _ex = null;
                }

                protected override void Act()
                {
                    try
                    {
                        _odsExtensionDirectory = DatabaseType.ODS.Directory(ArtifactsFolderStructureType);
                        _adminExtensionDirectory = DatabaseType.Admin.Directory(ArtifactsFolderStructureType);
                        _securityExtensionDirectory = DatabaseType.Security.Directory(ArtifactsFolderStructureType);
                    }
                    catch (Exception e)
                    {
                        _ex = e;
                    }
                }

                [Test]
                public void Should_not_throw_exception()
                {
                    _ex.ShouldBeNull();
                }

                [Test]
                public void Should_Ods_be_EdFi_directory()
                {
                    _odsExtensionDirectory.ShouldBe("EdFi");
                }

                [Test]
                public void Should_Admin_be_EdFi_Admin_directory()
                {
                    _adminExtensionDirectory.ShouldBe("EdFi_Admin");
                }

                [Test]
                public void Should_Security_be_EdFiSecurity_directory()
                {
                    _securityExtensionDirectory.ShouldBe("EdFiSecurity");
                }
            }
        }

        public class Given_database_type_is_not_specified
        {
            public class When_artifacts_folder_structure_is_NewVersion : TestFixtureBase
            {
                private Exception _ex;
                private DatabaseType _databaseType;

                protected override void Arrange()
                {
                    _ex = null;
                    _databaseType = A.Dummy<DatabaseType>();
                }

                protected override void Act()
                {
                    try
                    {
                        _databaseType.Directory(ArtifactsFolderStructureType.NewVersion);
                    }
                    catch (Exception e)
                    {
                        _ex = e;
                    }
                }

                [Test]
                public void Should_throw_exception()
                {
                    _ex.ShouldNotBeNull();
                }

                [Test]
                public void Should_exception_be_ArgumentOutOfRange()
                {
                    _ex.ShouldBeOfType<ArgumentOutOfRangeException>();
                }
            }

            public class When_artifacts_folder_structure_is_LegacyVersion : TestFixtureBase
            {
                private Exception _ex;
                private DatabaseType _databaseType;

                protected override void Arrange()
                {
                    _ex = null;
                    _databaseType = A.Dummy<DatabaseType>();
                }

                protected override void Act()
                {
                    try
                    {
                        _databaseType.Directory(ArtifactsFolderStructureType.LegacyVersion);
                    }
                    catch (Exception e)
                    {
                        _ex = e;
                    }
                }

                [Test]
                public void Should_throw_exception()
                {
                    _ex.ShouldNotBeNull();
                }

                [Test]
                public void Should_exception_be_ArgumentOutOfRange()
                {
                    _ex.ShouldBeOfType<ArgumentOutOfRangeException>();
                }
            }
        }
    }
}
