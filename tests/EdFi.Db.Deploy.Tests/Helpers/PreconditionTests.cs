// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Db.Deploy.Helpers;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests.Helpers
{
    [TestFixture]
    public class PreconditionTests
    {
        public class When_preconditions_are_null : TestFixtureBase
        {
            private Exception _ex;
            private string _text;

            protected override void Arrange()
            {
                _ex = null;
                _text = null;
            }

            protected override void Act()
            {
                try
                {
                    Preconditions.ThrowIfNull(_text, "NullText");
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
            public void Should_exception_be_ArgumentNullException()
            {
                _ex.ShouldBeOfType<ArgumentNullException>();
            }
        }

        public class When_preconditions_are_not_null : TestFixtureBase
        {
            private Exception _ex;
            private string _text;

            protected override void Arrange()
            {
                _ex = null;
                _text = "Instanced object";
            }

            protected override void Act()
            {
                try
                {
                    Preconditions.ThrowIfNull(_text, "NotNull");
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
        }
    }
}
