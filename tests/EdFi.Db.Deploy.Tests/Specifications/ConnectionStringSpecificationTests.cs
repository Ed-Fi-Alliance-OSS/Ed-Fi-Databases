// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Specifications;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

// ReSharper disable InconsistentNaming

namespace EdFi.Db.Deploy.Tests.Specifications
{
    [TestFixture]
    public class ConnectionStringSpecificationTests
    {
        public class When_parsing_command_options_with_valid_connection_string : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.ConnectionString)
                    .Returns("A valid connection string");

                _sut = new ConnectionStringSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_no_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true_if_connection_string_is_not_null_or_empty() => _result.ShouldBeTrue();
        }

        public class When_parsing_command_options_with_null_connection_string : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.ConnectionString)
                    .Returns(null);

                _sut = new ConnectionStringSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_message()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false_if_connection_string_is_null() => _result.ShouldBeFalse();
        }

        public class When_parsing_command_options_with_empty_connection_string : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.ConnectionString)
                    .Returns(string.Empty);

                _sut = new ConnectionStringSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_message()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false_if_connection_string_is_empty() => _result.ShouldBeFalse();
        }
    }
}
