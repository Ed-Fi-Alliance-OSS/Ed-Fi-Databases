// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Specifications;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests.Specifications
{
    [TestFixture]
    public class ExtensionVersionSpecificationTests
    {
        private static readonly string[] _paths = {
            Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                DatabaseConventions.TPDMExtensionPath,
                DatabaseConventions.VersionsFolder,
                "1.1.0",
                DatabaseConventions.StandardFolder,
                "4.0.0"),
            Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                $"{DatabaseConventions.ExtensionPrefix}Homograph",
                DatabaseConventions.VersionsFolder,
                "1.1.0",
                DatabaseConventions.StandardFolder,
                "4.0.0"),
            Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                DatabaseConventions.TPDMExtensionPath,
                DatabaseConventions.VersionsFolder,
                "1.1.0",
                DatabaseConventions.StandardFolder,
                "5.0.0"),
            Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                $"{DatabaseConventions.ExtensionPrefix}Homograph",
                DatabaseConventions.VersionsFolder,
                "1.1.0",
                DatabaseConventions.StandardFolder,
                "5.0.0"),
        };

        private static void SetUpFixture()
        {
            foreach (var path in _paths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        private static void TearDownFixture()
        {
            foreach (var path in _paths)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path);
                }
            }
        }

        public class When_validating_with_null_options : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = null;
                _sut = new ExtensionVersionSpecification();
            }

            protected override void Act()
                => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_expected_exception()
            {
                ActualException.ShouldNotBeNull();
                ActualException.Message.ShouldBe("Value cannot be null. (Parameter 'obj')");
            }
        }

        public class When_validating_non_extension_project_paths : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.FilePaths)
                    .Returns(new[] {
                        "../../../../../../Ed-Fi-ODS-Implementation/",
                        "../../../../../../Ed-Fi-ODS/"
                    });

                _sut = new ExtensionVersionSpecification();
            }

            protected override void Act()
                => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_no_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }

        public class When_validating_extension_project_paths_with_null_standard_and_extension_versions : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.FilePaths)
                    .Returns(new[] {
                        "../../../../../../Ed-Fi-Extensions/Extensions/EdFi.Ods.Extensions.TPDM/",
                        "../../../../../../Ed-Fi-Extensions/Extensions/EdFi.Ods.Extensions.Homograph/"
                    });

                _sut = new ExtensionVersionSpecification();
            }

            protected override void Act()
                => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false() => _result.ShouldBeFalse();
        }

        public class When_validating_extension_project_paths_with_valid_4_0_0_extensionVersion : TestFixtureBase
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                SetUpFixture();
            }

            [OneTimeTearDown]
            public void TearDown()
            {
                TearDownFixture();
            }

            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.FilePaths)
                    .Returns(new[] {
                        Path.Combine(
                            TestContext.CurrentContext.TestDirectory,
                            DatabaseConventions.TPDMExtensionPath
                            ),
                        Path.Combine(
                            TestContext.CurrentContext.TestDirectory,
                            $"{DatabaseConventions.ExtensionPrefix}Homograph"
                            )
                    });

                A.CallTo(() => _options.StandardVersion)
                    .Returns("4.0.0");

                A.CallTo(() => _options.ExtensionVersion)
                    .Returns("1.1.0");

                _sut = new ExtensionVersionSpecification();
            }

            protected override void Act()
                => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_no_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }

        public class When_validating_extension_project_paths_with_valid_5_0_0_extensionVersion : TestFixtureBase
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                SetUpFixture();
            }

            [OneTimeTearDown]
            public void TearDown()
            {
                TearDownFixture();
            }

            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.FilePaths)
                    .Returns(new[] {
                        Path.Combine(
                            TestContext.CurrentContext.TestDirectory,
                            DatabaseConventions.TPDMExtensionPath
                        ),
                        Path.Combine(
                            TestContext.CurrentContext.TestDirectory,
                            $"{DatabaseConventions.ExtensionPrefix}Homograph"
                        )
                    });

                A.CallTo(() => _options.StandardVersion)
                    .Returns("5.0.0");

                A.CallTo(() => _options.ExtensionVersion)
                    .Returns("1.1.0");

                _sut = new ExtensionVersionSpecification();
            }

            protected override void Act()
                => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_no_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }
        
        public class When_validating_extension_project_paths_with_invalid_standardVersion : TestFixtureBase
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                SetUpFixture();
            }

            [OneTimeTearDown]
            public void TearDown()
            {
                TearDownFixture();
            }

            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.FilePaths)
                    .Returns(new[] {
                        Path.Combine(
                            TestContext.CurrentContext.TestDirectory,
                            DatabaseConventions.TPDMExtensionPath
                            ),
                        Path.Combine(
                            TestContext.CurrentContext.TestDirectory,
                            $"{DatabaseConventions.ExtensionPrefix}Homograph"
                            )
                    });

                A.CallTo(() => _options.StandardVersion)
                    .Returns("0.0.0");

                A.CallTo(() => _options.ExtensionVersion)
                    .Returns("1.1.0");

                _sut = new ExtensionVersionSpecification();
            }

            protected override void Act()
                => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_four_error_message()
                => _sut.ErrorMessages
                    .Count.ShouldBe(4);

            [Test]
            public void Should_return_false() => _result.ShouldBeFalse();
        }
    }
}
