// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using EdFi.Db.Deploy.DatabaseCommands;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Parameters.Verbs;
using EdFi.Db.Deploy.Specifications;
using EdFi.Db.Deploy.UpgradeEngineStrategies;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class ApplicationRunnerTests
    {
        public class When_deploying_the_ods_to_postgres_without_extensions_and_with_no_validation_issues : TestFixtureBase
        {
            private IDatabaseCommand _databaseCommand;
            private IDatabaseCommandFactory _databaseCommandFactory;
            private ICompositeSpecification _compositeSpecification;
            private ApplicationRunner _sut;
            private int _result;
            private DeployDatabase _options;

            protected override void Arrange()
            {
                _options = new DeployDatabase
                {
                    Engine = EngineType.PostgreSql,
                    DatabaseType = DatabaseType.ODS,
                    ConnectionString = "Valid Connection String"
                };

                _databaseCommand = Stub<IDatabaseCommand>();

                _databaseCommandFactory = Stub<IDatabaseCommandFactory>();

                A.CallTo(() => _databaseCommandFactory.CreateDatabaseCommands(A<EngineType>._))
                    .Returns(new List<IDatabaseCommand> {_databaseCommand});

                _compositeSpecification = Stub<ICompositeSpecification>();

                A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>._))
                    .Returns(true);

                A.CallTo(() => _databaseCommand.Execute(A<IOptions>._))
                    .Returns(new DatabaseCommandResult {IsSuccessful = true});

                var compositeSpecifications = new List<ICompositeSpecification> {_compositeSpecification};

                _result = -99;

                _sut = new ApplicationRunner(_options, _databaseCommandFactory, compositeSpecifications);
            }

            protected override void Act() => _result = _sut.Run();

            [Test]
            public void Should_check_if_there_is_any_validation_errors()
                => A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>.That.Matches(x => (IOptions) x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_create_the_database_commands()
                => A.CallTo(
                        () => _databaseCommandFactory.CreateDatabaseCommands(
                            A<EngineType>.That.Matches(x => x == EngineType.PostgreSql)))
                    .MustHaveHappened();

            [Test]
            public void Should_execute_the_database_commands()
                => A.CallTo(
                        () => _databaseCommand.Execute(
                            A<IOptions>.That.Matches(x => x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_return_success() => _result.ShouldBe(ApplicationStatus.Success);
        }

        public class When_deploying_the_ods_to_postgres_without_extensions_and_with_validation_issues : TestFixtureBase
        {
            private IDatabaseCommand _databaseCommand;
            private IDatabaseCommandFactory _databaseCommandFactory;
            private ICompositeSpecification _compositeSpecification;
            private ApplicationRunner _sut;
            private int _result;
            private DeployDatabase _options;

            protected override void Arrange()
            {
                _options = new DeployDatabase
                {
                    Engine = EngineType.PostgreSql,
                    DatabaseType = DatabaseType.ODS,
                    ConnectionString = "Valid Connection String"
                };

                _databaseCommand = Stub<IDatabaseCommand>();

                A.CallTo(() => _databaseCommand.Execute(A<IOptions>._));

                _databaseCommandFactory = Stub<IDatabaseCommandFactory>();

                A.CallTo(() => _databaseCommandFactory.CreateDatabaseCommands(A<EngineType>._))
                    .Returns(new List<IDatabaseCommand> {_databaseCommand});

                _compositeSpecification = Stub<ICompositeSpecification>();

                A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>._))
                    .Returns(false);

                var compositeSpecifications = new List<ICompositeSpecification> {_compositeSpecification};

                _result = -99;

                _sut = new ApplicationRunner(_options, _databaseCommandFactory, compositeSpecifications);
            }

            protected override void Act() => _result = _sut.Run();

            [Test]
            public void Should_check_if_there_is_any_validation_errors()
                => A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>.That.Matches(x => (IOptions) x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_not_create_the_database_commands()
                => A.CallTo(
                        () => _databaseCommandFactory.CreateDatabaseCommands(
                            A<EngineType>.That.Matches(x => x == EngineType.PostgreSql)))
                    .MustNotHaveHappened();

            [Test]
            public void Should_not_execute_the_database_commands()
                => A.CallTo(
                        () => _databaseCommand.Execute(
                            A<IOptions>.That.Matches(x => x == _options)))
                    .MustNotHaveHappened();

            [Test]
            public void Should_return_failure() => _result.ShouldBe(ApplicationStatus.Failure);
        }

        public class
            When_deploying_the_ods_to_postgres_without_extensions_and_with_no_validation_issues_and_database_throws_an_error
            : TestFixtureBase
        {
            private IDatabaseCommand _databaseCommand;
            private IDatabaseCommandFactory _databaseCommandFactory;
            private ICompositeSpecification _compositeSpecification;
            private ApplicationRunner _sut;
            private int _result;
            private DeployDatabase _options;

            protected override void Arrange()
            {
                _options = new DeployDatabase
                {
                    Engine = EngineType.PostgreSql,
                    DatabaseType = DatabaseType.ODS,
                    ConnectionString = "Valid Connection String"
                };

                _databaseCommand = Stub<IDatabaseCommand>();

                A.CallTo(() => _databaseCommand.Execute(A<IOptions>._))
                    .Throws(new Exception());

                _databaseCommandFactory = Stub<IDatabaseCommandFactory>();

                A.CallTo(() => _databaseCommandFactory.CreateDatabaseCommands(A<EngineType>._))
                    .Returns(new List<IDatabaseCommand> {_databaseCommand});

                _compositeSpecification = Stub<ICompositeSpecification>();

                A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>._))
                    .Returns(true);

                var compositeSpecifications = new List<ICompositeSpecification> {_compositeSpecification};

                _result = -99;

                _sut = new ApplicationRunner(_options, _databaseCommandFactory, compositeSpecifications);
            }

            protected override void Act() => _result = _sut.Run();

            [Test]
            public void Should_check_if_there_is_any_validation_errors()
                => A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>.That.Matches(x => (IOptions) x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_not_create_the_database_commands()
                => A.CallTo(
                        () => _databaseCommandFactory.CreateDatabaseCommands(
                            A<EngineType>.That.Matches(x => x == EngineType.PostgreSql)))
                    .MustHaveHappened();

            [Test]
            public void Should_not_execute_the_database_commands()
                => A.CallTo(
                        () => _databaseCommand.Execute(
                            A<IOptions>.That.Matches(x => x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_return_failure() => _result.ShouldBe(ApplicationStatus.Failure);
        }

        public class When_running_whatif_with_upgrade_required_to_sqlserver_with_features_and_no_validation_issues
            : TestFixtureBase
        {
            private IDatabaseCommand _databaseCommand;
            private IDatabaseCommandFactory _databaseCommandFactory;
            private ICompositeSpecification _compositeSpecification;
            private ApplicationRunner _sut;
            private int _result;
            private WhatIfExecution _options;

            protected override void Arrange()
            {
                _options = new WhatIfExecution
                {
                    Engine = EngineType.SqlServer,
                    DatabaseType = DatabaseType.ODS,
                    ConnectionString = "Valid Connection String",
                    Features = new List<string>
                    {
                        "Changes",
                        "Sample"
                    },
                };

                _databaseCommand = Stub<IDatabaseCommand>();

                _databaseCommandFactory = Stub<IDatabaseCommandFactory>();

                A.CallTo(() => _databaseCommandFactory.CreateDatabaseCommands(A<EngineType>._))
                    .Returns(new List<IDatabaseCommand> {_databaseCommand});

                _compositeSpecification = Stub<ICompositeSpecification>();

                A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>._))
                    .Returns(true);

                A.CallTo(() => _databaseCommand.Execute(A<IOptions>._))
                    .Returns(
                        new DatabaseCommandResult
                        {
                            IsSuccessful = true,
                            RequiresUpgrade = true
                        });

                var compositeSpecifications = new List<ICompositeSpecification> {_compositeSpecification};

                _result = -99;

                _sut = new ApplicationRunner(_options, _databaseCommandFactory, compositeSpecifications);
            }

            protected override void Act() => _result = _sut.Run();

            [Test]
            public void Should_check_if_there_is_any_validation_errors()
                => A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>.That.Matches(x => (IOptions) x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_create_the_database_commands()
                => A.CallTo(
                        () => _databaseCommandFactory.CreateDatabaseCommands(
                            A<EngineType>.That.Matches(x => x == EngineType.SqlServer)))
                    .MustHaveHappened();

            [Test]
            public void Should_execute_the_database_commands()
                => A.CallTo(
                        () => _databaseCommand.Execute(
                            A<IOptions>.That.Matches(x => x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_return_upgrade_is_required() => _result.ShouldBe(ApplicationStatus.UpgradeIsRequired);
        }

        public class When_running_whatif_with_no_upgrade_required_to_sqlserver_with_features_and_no_validation_issues
            : TestFixtureBase
        {
            private IDatabaseCommand _databaseCommand;
            private IDatabaseCommandFactory _databaseCommandFactory;
            private ICompositeSpecification _compositeSpecification;
            private ApplicationRunner _sut;
            private int _result;
            private WhatIfExecution _options;

            protected override void Arrange()
            {
                _options = new WhatIfExecution
                {
                    Engine = EngineType.SqlServer,
                    DatabaseType = DatabaseType.ODS,
                    ConnectionString = "Valid Connection String",
                    Features = new List<string>
                    {
                        "Changes",
                        "Sample"
                    }
                };

                _databaseCommand = Stub<IDatabaseCommand>();

                _databaseCommandFactory = Stub<IDatabaseCommandFactory>();

                A.CallTo(() => _databaseCommandFactory.CreateDatabaseCommands(A<EngineType>._))
                    .Returns(new List<IDatabaseCommand> {_databaseCommand});

                _compositeSpecification = Stub<ICompositeSpecification>();

                A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>._))
                    .Returns(true);

                A.CallTo(() => _databaseCommand.Execute(A<IOptions>._))
                    .Returns(
                        new DatabaseCommandResult
                        {
                            IsSuccessful = true,
                            RequiresUpgrade = false
                        });

                var compositeSpecifications = new List<ICompositeSpecification> {_compositeSpecification};

                _result = -99;

                _sut = new ApplicationRunner(_options, _databaseCommandFactory, compositeSpecifications);
            }

            protected override void Act() => _result = _sut.Run();

            [Test]
            public void Should_check_if_there_is_any_validation_errors()
                => A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>.That.Matches(x => (IOptions) x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_create_the_database_commands()
                => A.CallTo(
                        () => _databaseCommandFactory.CreateDatabaseCommands(
                            A<EngineType>.That.Matches(x => x == EngineType.SqlServer)))
                    .MustHaveHappened();

            [Test]
            public void Should_execute_the_database_commands()
                => A.CallTo(
                        () => _databaseCommand.Execute(
                            A<IOptions>.That.Matches(x => x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_return_no_upgrade_required() => _result.ShouldBe(ApplicationStatus.Success);
        }

        public class When_running_whatif_to_sqlserver_with_features_and_with_validation_issues : TestFixtureBase
        {
            private IDatabaseCommand _databaseCommand;
            private IDatabaseCommandFactory _databaseCommandFactory;
            private ICompositeSpecification _compositeSpecification;
            private ApplicationRunner _sut;
            private int _result;
            private WhatIfExecution _options;

            protected override void Arrange()
            {
                _options = new WhatIfExecution
                {
                    Engine = EngineType.SqlServer,
                    DatabaseType = DatabaseType.ODS,
                    ConnectionString = "Valid Connection String",
                    Features = new List<string>
                    {
                        "Changes",
                        "1234Valid"
                    }
                };

                _databaseCommand = Stub<IDatabaseCommand>();

                A.CallTo(() => _databaseCommand.Execute(A<IOptions>._));

                _databaseCommandFactory = Stub<IDatabaseCommandFactory>();

                A.CallTo(() => _databaseCommandFactory.CreateDatabaseCommands(A<EngineType>._))
                    .Returns(new List<IDatabaseCommand> {_databaseCommand});

                _compositeSpecification = Stub<ICompositeSpecification>();

                A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>._))
                    .Returns(false);

                var compositeSpecifications = new List<ICompositeSpecification> {_compositeSpecification};

                _result = -99;

                _sut = new ApplicationRunner(_options, _databaseCommandFactory, compositeSpecifications);
            }

            protected override void Act() => _result = _sut.Run();

            [Test]
            public void Should_check_if_there_is_any_validation_errors()
                => A.CallTo(() => _compositeSpecification.IsSatisfiedBy(A<object>.That.Matches(x => (IOptions) x == _options)))
                    .MustHaveHappened();

            [Test]
            public void Should_not_create_the_database_commands()
                => A.CallTo(
                        () => _databaseCommandFactory.CreateDatabaseCommands(
                            A<EngineType>.That.Matches(x => x == EngineType.SqlServer)))
                    .MustNotHaveHappened();

            [Test]
            public void Should_not_execute_the_database_commands()
                => A.CallTo(
                        () => _databaseCommand.Execute(
                            A<IOptions>.That.Matches(x => x == _options)))
                    .MustNotHaveHappened();

            [Test]
            public void Should_return_failure() => _result.ShouldBe(ApplicationStatus.Failure);
        }
    }
}
