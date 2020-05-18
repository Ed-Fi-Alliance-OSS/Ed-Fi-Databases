// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Specifications;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests.Specifications
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class DatabaseTypeSpecificationTests
    {
        public class When_validating_ODS_database_type : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.ODS);

                _sut = new DatabaseTypeSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_not_have_any_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true_if_the_database_type_is_valid() => _result.ShouldBeTrue();
        }

        public class When_validating_Admin_database_type : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.Admin);

                _sut = new DatabaseTypeSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_not_have_any_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true_if_the_database_type_is_valid() => _result.ShouldBeTrue();
        }

        public class When_validating_Security_database_type : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(DatabaseType.Security);

                _sut = new DatabaseTypeSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_not_have_any_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true_if_the_database_type_is_valid() => _result.ShouldBeTrue();
        }

        public class When_validating_invalid_database_type : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.DatabaseType)
                    .Returns(A.Dummy<DatabaseType>());

                _sut = new DatabaseTypeSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_an_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false_if_the_database_type_is_invalid() => _result.ShouldBeFalse();
        }
    }
}
