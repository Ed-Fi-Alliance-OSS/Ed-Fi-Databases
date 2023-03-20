// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Specifications;
using FakeItEasy;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Shouldly;

namespace EdFi.Db.Deploy.Tests.Specifications
{
    [TestFixture]
    public class StandardVersionSpecificationTests
    {
        private static readonly string _path =
            Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                DatabaseConventions.StandardProject,
                DatabaseConventions.StandardFolder,
                "4.0.0");

        private static void SetUpFixture()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        private static void TearDownFixture()
        {
            if (Directory.Exists(_path))
            {
                Directory.Delete(_path);
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
                _sut = new StandardVersionSpecification();
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

        public class When_validating_non_standard_project_path : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.FilePaths)
                    .Returns(new[] { "../../../../../../Ed-Fi-ODS-Implementation/" });

                _sut = new StandardVersionSpecification();
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

        public class When_validating_standard_project_path_with_null_standardVersion : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.FilePaths)
                    .Returns(new[] { "../../../../../../Ed-Fi-ODS/Application/EdFi.Ods.Standard/" });

                _sut = new StandardVersionSpecification();
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

        public class When_validating_standard_project_path_with_valid_standardVersion : TestFixtureBase
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
                            DatabaseConventions.StandardProject
                            )
                    });

                A.CallTo(() => _options.StandardVersion)
                    .Returns("4.0.0");

                _sut = new StandardVersionSpecification();
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

        public class When_validating_standard_project_path_with_invalid_standardVersion : TestFixtureBase
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
                        Path.Combine(TestContext.CurrentContext.TestDirectory,
                        DatabaseConventions.StandardProject
                        )
                    });

                A.CallTo(() => _options.StandardVersion)
                    .Returns("5.0.0");

                _sut = new StandardVersionSpecification();
            }

            protected override void Act()
                => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_message()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false() => _result.ShouldBeFalse();
        }
    }
}
