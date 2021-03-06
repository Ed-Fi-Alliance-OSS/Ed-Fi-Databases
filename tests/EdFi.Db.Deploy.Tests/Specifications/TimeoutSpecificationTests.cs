﻿// SPDX-License-Identifier: Apache-2.0
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
    public class TimeoutSpecificationTests
    {
        public class When_parsing_command_options_with_valid_timeout : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.TimeoutInSeconds)
                    .Returns(60);

                _sut = new TimeoutSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_return_true_when_timeout_is_greater_than_zero() => _result.ShouldBeTrue();
        }

        public class When_parsing_command_options_with_zero_timeout : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.TimeoutInSeconds)
                    .Returns(0);

                _sut = new TimeoutSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_no_error_messages()
                => _sut.ErrorMessages
                    .Count.ShouldBe(0);

            [Test]
            public void Should_return_true_when_timeout_is_equal_to_zero() => _result.ShouldBeTrue();
        }

        public class When_parsing_command_options_with_invaild_timeout : TestFixtureBase
        {
            private ISpecification<IOptions> _sut;
            private bool _result;
            private IOptions _options;

            protected override void Arrange()
            {
                _options = A.Fake<IOptions>();

                A.CallTo(() => _options.TimeoutInSeconds)
                    .Returns(-1);

                _sut = new TimeoutSpecification();
            }

            protected override void Act() => _result = _sut.IsSatisfiedBy(_options);

            [Test]
            public void Should_have_one_error_message()
                => _sut.ErrorMessages
                    .Count.ShouldBe(1);

            [Test]
            public void Should_return_false_when_timeout_is_less_than_zero() => _result.ShouldBeFalse();
        }
    }
}
