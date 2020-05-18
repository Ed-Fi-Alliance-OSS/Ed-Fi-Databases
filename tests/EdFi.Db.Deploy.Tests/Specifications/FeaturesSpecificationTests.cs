// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.IO;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Specifications;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests.Specifications
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class FeaturesSpecificationTests
    {
        private static readonly string _path =
            Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "SupportingArtifacts",
                "Database",
                "Structure",
                "EdFi",
                "Changes");

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

        public class When_validating_features_with_postgres_and_valid_feature : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.PostgreSql);

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_no_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }

        public class When_validating_features_with_postgres_and_invalid_feature : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.PostgreSql);

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"9875Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false() => _result.ShouldBeFalse();
        }

        public class When_validating_features_with_sqlserver_and_valid_feature : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.SqlServer);

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_zero_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }

        public class When_validating_features_with_sqlserver_and_invalid_feature : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.SqlServer);

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"9873Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false() => _result.ShouldBeFalse();
        }

        public class When_validating_features_with_extensions_path_with_postgres_and_the_folders_exist : TestFixtureBase
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                SetUpFixture();
            }

            [OneTimeTearDown]
            public void TearDown()
            {
                FeaturesSpecificationTests.TearDownFixture();
            }

            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.PostgreSql);

                A.CallTo(() => _options.FilePaths)
                    .Returns(
                        new List<string>
                        {
                            Path.Combine(TestContext.CurrentContext.TestDirectory, "SupportingArtifacts", "Database")
                        });

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }

        public class When_validating_features_with_extensions_path_with_postgres_and_the_folders_do_not_exist : TestFixtureBase
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                SetUpFixture();
            }

            [OneTimeTearDown]
            public void TearDown()
            {
                FeaturesSpecificationTests.TearDownFixture();
            }

            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.PostgreSql);

                A.CallTo(() => _options.FilePaths)
                    .Returns(
                        new List<string>
                        {
                            Path.Combine(TestContext.CurrentContext.TestDirectory, "SupportingArtifacts", "InvalidFolder")
                        });

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_no_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }

        public class When_validating_features_with_extensions_path_with_sqlserver_and_the_folders_exist : TestFixtureBase
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                SetUpFixture();
            }

            [OneTimeTearDown]
            public void TearDown()
            {
                FeaturesSpecificationTests.TearDownFixture();
            }

            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.SqlServer);

                A.CallTo(() => _options.FilePaths)
                    .Returns(
                        new List<string>
                        {
                            Path.Combine(TestContext.CurrentContext.TestDirectory, "SupportingArtifacts", "Database")
                        });

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_zero_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }

        public class When_validating_features_with_extensions_path_with_sqlserver_and_the_folders_do_not_exist : TestFixtureBase
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                SetUpFixture();
            }

            [OneTimeTearDown]
            public void TearDown()
            {
                FeaturesSpecificationTests.TearDownFixture();
            }

            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                A.CallTo(() => _options.Engine)
                    .Returns(EngineType.SqlServer);

                A.CallTo(() => _options.FilePaths)
                    .Returns(
                        new List<string>
                        {
                            Path.Combine(TestContext.CurrentContext.TestDirectory, "SupportingArtifacts", "InvalidFolder")
                        });

                A.CallTo(() => _options.Features)
                    .Returns(
                        new List<string> {"Changes"});

                _sut = new FeaturesSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_zero_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true() => _result.ShouldBeTrue();
        }
    }
}
