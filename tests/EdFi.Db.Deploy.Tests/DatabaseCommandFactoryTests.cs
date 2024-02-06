// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Db.Deploy.DatabaseCommands;
using EdFi.Db.Deploy.DeployJournal;
using EdFi.Db.Deploy.UpgradeEngineFactories;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Db.Deploy.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class DatabaseCommandFactoryTests
    {
        private static readonly IDatabaseCommand[] _databaseCommands =
        {
            // Include new commands in a non-ordered, non-grouped way to ensure grouping and reordering are executed correctly.
            new SqlServerUpdateLegacyDatabaseCommand(A.Dummy<IScriptPathInfoProvider>(), A.Dummy<IEdFiLegacyDatabaseRepository>(),
                A.Dummy<ISqlServerUpgradeEngineFactory>()),
            new SqlServerDeployFeaturesFromPathCommand(A.Dummy<SqlServerUpgradeEngineFactory>()),
            new PostgresDeployFeaturesFromPathCommand(A.Dummy<PostgresUpgradeEngineFactory>()),
            new PostgresDeployLegacyExtensionFeaturesFromPathCommand(A.Dummy<PostgresUpgradeEngineFactory>()),
            new SqlServerDeployLegacyExtensionsFromPathCommand(A.Dummy<SqlServerUpgradeEngineFactory>()),
            new PostgresDeployLegacyExtensionsFromPathCommand(A.Dummy<PostgresUpgradeEngineFactory>()),
            new SqlServerDeployLegacyExtensionFeaturesFromPathCommand(A.Dummy<SqlServerUpgradeEngineFactory>()),
            new SqlServerCreateDatabaseCommand(),
            new PostgresCreateDatabaseCommand(),
            new SqlServerDeployDatabaseFromPathCommand(A.Dummy<SqlServerUpgradeEngineFactory>()),
            new PostgresDeployDatabaseFromPathCommand(A.Dummy<PostgresUpgradeEngineFactory>())
        };

        public class When_creating_commands_for_postgres : TestFixtureBase
        {
            private IList<IDatabaseCommand> _results;
            private DatabaseCommandFactory _sut;

            protected override void Arrange() => _sut = new DatabaseCommandFactory(_databaseCommands);

            protected override void Act()
                => _results = _sut.CreateDatabaseCommands(EngineType.PostgreSql)
                    .ToList();

            [Test]
            public void Should_have_the_first_result_of_type_PostgresCreateDatabaseCommand()
                => _results[0]
                    .ShouldBeOfType<PostgresCreateDatabaseCommand>();

            [Test]
            public void Should_have_the_second_result_of_type_PostgresDeployFeaturesCommand()
                => _results[1]
                    .ShouldBeOfType<PostgresDeployDatabaseFromPathCommand>();

            [Test]
            public void Should_have_the_third_result_of_type_PostgresDeployLegacyExtensionsFromPathCommand()
                => _results[2]
                    .ShouldBeOfType<PostgresDeployLegacyExtensionsFromPathCommand>();

            [Test]
            public void Should_have_the_forth_result_of_type_PostgresDeployFeaturesFromPathCommand()
                => _results[3]
                    .ShouldBeOfType<PostgresDeployFeaturesFromPathCommand>();

            [Test]
            public void Should_have_the_fifth_result_of_type_PostgresDeployLegacyExtensionFeaturesFromPathCommand()
                => _results[4]
                    .ShouldBeOfType<PostgresDeployLegacyExtensionFeaturesFromPathCommand>();
        }

        public class When_creating_commands_for_sqlserver : TestFixtureBase
        {
            private IList<IDatabaseCommand> _results;
            private DatabaseCommandFactory _sut;

            protected override void Arrange() => _sut = new DatabaseCommandFactory(_databaseCommands);

            protected override void Act()
                => _results = _sut.CreateDatabaseCommands(EngineType.SqlServer)
                    .ToList();

            [Test]
            public void Should_have_the_first_result_of_type_SqlServerCreateDatabaseCommand()
                => _results[0]
                    .ShouldBeOfType<SqlServerCreateDatabaseCommand>();

            [Test]
            public void Should_have_the_second_result_of_type_SqlServerLegacyDatabaseCheckCommand()
                => _results[1]
                    .ShouldBeOfType<SqlServerUpdateLegacyDatabaseCommand>();

            [Test]
            public void Should_have_the_third_result_of_type_SqlServerDeployDatabaseFromPathCommand()
                => _results[2]
                    .ShouldBeOfType<SqlServerDeployDatabaseFromPathCommand>();

            [Test]
            public void Should_have_the_forth_result_of_type_SqlServerDeployLegacyExtensionsFromPathCommand()
                => _results[3]
                    .ShouldBeOfType<SqlServerDeployLegacyExtensionsFromPathCommand>();

            [Test]
            public void Should_have_the_fifth_result_of_type_SqlServerDeployFeaturesFromPathCommand()
                => _results[4]
                    .ShouldBeOfType<SqlServerDeployFeaturesFromPathCommand>();

            [Test]
            public void Should_have_the_sixth_result_of_type_SqlServerDeployLegacyExtensionFeaturesFromPathCommand()
                => _results[5]
                    .ShouldBeOfType<SqlServerDeployLegacyExtensionFeaturesFromPathCommand>();
        }
    }
}
