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
    [TestFixture]
    public class EngineTypeExtensionTests
    {
        public class When_engine_type_has_extension_directory : TestFixtureBase
        {
            private Exception _ex;
            private string _msSqlDirectory;
            private string _pgSqlDirectory;

            protected override void Arrange()
            {
                _ex = null;
            }

            protected override void Act()
            {
                try
                {
                    _msSqlDirectory = EngineType.SqlServer.Directory();
                    _pgSqlDirectory = EngineType.PostgreSql.Directory();
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
            public void Should_SqlServer_be_MsSql_directory()
            {
                _msSqlDirectory.ShouldBe("MsSql");
            }

            [Test]
            public void Should_PostgreSql_be_PgSql_directory()
            {
                _pgSqlDirectory.ShouldBe("PgSql");
            }
        }

        public class When_engine_type_is_not_specified : TestFixtureBase
        {
            private Exception _ex;
            private EngineType _databaseType;
            private string _engineTypeDirectory;

            protected override void Arrange()
            {
                _ex = null;
                _databaseType = A.Dummy<EngineType>();
            }

            protected override void Act()
            {
                try
                {
                    _engineTypeDirectory = _databaseType.Directory();
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
