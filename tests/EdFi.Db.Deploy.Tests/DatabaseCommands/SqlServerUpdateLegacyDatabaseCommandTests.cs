// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using DbUp.Engine;
using EdFi.Db.Deploy.Adapters;
using EdFi.Db.Deploy.DatabaseCommands;
using EdFi.Db.Deploy.DeployJournal;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Parameters.Verbs;
using EdFi.Db.Deploy.UpgradeEngineFactories;
using EdFi.Db.Deploy.UpgradeEngineStrategies;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming
namespace EdFi.Db.Deploy.Tests.DatabaseCommands
{
    [TestFixture]
    public class SqlServerUpdateLegacyDatabaseCommandTests : TestFixtureBase
    {
        protected IScriptPathInfoProvider ScriptPathInfoProvider;
        protected IEdFiLegacyDatabaseRepository EdFiLegacyDatabaseRepository;
        protected SqlServerUpdateLegacyDatabaseCommand System;
        protected IUpgradeEngineWrapper UpgradeEngineWrapper;
        protected ISqlServerUpgradeEngineFactory UpgradeEngineFactory;

        protected override void Arrange()
        {
            ScriptPathInfoProvider = A.Fake<IScriptPathInfoProvider>();
            EdFiLegacyDatabaseRepository = A.Fake<IEdFiLegacyDatabaseRepository>();
            UpgradeEngineWrapper = A.Fake<IUpgradeEngineWrapper>();
            UpgradeEngineFactory = A.Fake<ISqlServerUpgradeEngineFactory>();

            A.CallTo(() => UpgradeEngineFactory.Create(A<UpgradeEngineConfig>._, A<SqlScript[]>._))
                .Returns(UpgradeEngineWrapper);

            System = new SqlServerUpdateLegacyDatabaseCommand(
                ScriptPathInfoProvider, EdFiLegacyDatabaseRepository, UpgradeEngineFactory);
        }

        [TestFixture]
        public class When_executing_the_command : SqlServerUpdateLegacyDatabaseCommandTests
        {
            protected DatabaseCommandResult Result;
            protected IOptions Options;

            protected override void Arrange()
            {
                base.Arrange();
                Options = A.Dummy<IOptions>();
            }

            protected override void Act()
            {
                Result = System.Execute(Options);
            }

            [TestFixture]
            public class Given_null_options : SqlServerUpdateLegacyDatabaseCommandTests
            {
                // Should not inherit from When_executing_the_command because we need to handle the exception.

                [Test]
                public void Then_throw_argument_null_exception()
                {
                    new Action(() => System.Execute(null))
                        .ShouldThrow<ArgumentNullException>();
                }
            }

            [TestFixture]
            public class Given_admin_database : When_executing_the_command
            {
                protected override void Arrange()
                {
                    base.Arrange();
                    Options.DatabaseType = DatabaseType.Admin;
                }

                [Test]
                public void Then_command_should_be_successful() => Result.IsSuccessful.ShouldBeTrue();

                [Test]
                public void Then_script_path_info_provider_should_not_have_been_called()
                    => A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options)).MustNotHaveHappened();

                [Test]
                public void Then_upgrade_should_not_be_required() => Result.RequiresUpgrade.ShouldBeFalse();

                [Test]
                public void Then_upgrade_should_not_have_occurred()
                    => A.CallTo(() => UpgradeEngineWrapper.PerformUpgrade())
                        .MustNotHaveHappened();

                [Test]
                public void Then_whatif_analysis_should_not_have_occurred()
                    => A.CallTo(() => UpgradeEngineWrapper.IsUpgradeRequired())
                        .MustNotHaveHappened();
            }

            [TestFixture]
            public class Given_security_database : When_executing_the_command
            {
                protected override void Arrange()
                {
                    base.Arrange();
                    Options.DatabaseType = DatabaseType.Security;
                }

                [Test]
                public void Then_command_should_be_successful() => Result.IsSuccessful.ShouldBeTrue();

                [Test]
                public void Then_script_path_info_provider_should_not_have_been_called()
                    => A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options)).MustNotHaveHappened();

                [Test]
                public void Then_upgrade_should_not_be_required() => Result.RequiresUpgrade.ShouldBeFalse();

                [Test]
                public void Then_upgrade_should_not_have_occurred()
                    => A.CallTo(() => UpgradeEngineWrapper.PerformUpgrade())
                        .MustNotHaveHappened();

                [Test]
                public void Then_whatif_analysis_should_not_have_occurred()
                    => A.CallTo(() => UpgradeEngineWrapper.IsUpgradeRequired())
                        .MustNotHaveHappened();
            }

            [TestFixture]
            public class Given_ods_database : When_executing_the_command
            {
                protected const string ConnectionString = "hello world";

                protected override void Arrange()
                {
                    base.Arrange();
                    Options.DatabaseType = DatabaseType.ODS;
                    Options.ConnectionString = ConnectionString;
                }

                [TestFixture]
                public class And_it_is_an_empty_database : Given_ods_database
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        A.CallTo(() => EdFiLegacyDatabaseRepository.FindAllTables(A<string>._))
                            .Returns(null);
                    }

                    [Test]
                    public void Then_command_should_be_successful() => Result.IsSuccessful.ShouldBeTrue();

                    [Test]
                    public void Then_script_path_info_provider_should_not_have_been_called()
                        => A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options)).MustNotHaveHappened();

                    [Test]
                    public void Then_upgrade_should_not_be_required() => Result.RequiresUpgrade.ShouldBeFalse();

                    [Test]
                    public void Then_upgrade_should_not_have_occurred()
                        => A.CallTo(() => UpgradeEngineWrapper.PerformUpgrade())
                            .MustNotHaveHappened();

                    [Test]
                    public void Then_whatif_analysis_should_not_have_occurred()
                        => A.CallTo(() => UpgradeEngineWrapper.IsUpgradeRequired())
                            .MustNotHaveHappened();
                }

                [TestFixture]
                public class And_it_does_not_have_version_level_table : Given_ods_database
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        A.CallTo(() => EdFiLegacyDatabaseRepository.FindAllTables(A<string>._))
                            .Returns(
                                new[]
                                {
                                    new DatabaseTable
                                    {
                                        TableName = "random",
                                        TableSchema = "dbo"
                                    }
                                });
                    }

                    [Test]
                    public void Then_command_should_be_successful() => Result.IsSuccessful.ShouldBeTrue();

                    [Test]
                    public void Then_script_path_info_provider_should_not_have_been_called()
                        => A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options)).MustNotHaveHappened();

                    [Test]
                    public void Then_upgrade_should_not_be_required() => Result.RequiresUpgrade.ShouldBeFalse();
                }

                [TestFixture]
                public class And_it_has_a_version_level_table : Given_ods_database
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        A.CallTo(() => EdFiLegacyDatabaseRepository.FindAllTables(A<string>._))
                            .Returns(
                                new[]
                                {
                                    new DatabaseTable
                                    {
                                        TableName = "VersionLevel",
                                        TableSchema = "dbo"
                                    }
                                });
                    }

                    [TestFixture]
                    public class And_using_whatif_verb : And_it_has_a_version_level_table
                    {
                        protected override void Arrange()
                        {
                            base.Arrange();
                            Options = new WhatIfExecution { DatabaseType = DatabaseType.ODS };
                        }

                        [Test]
                        public void Then_command_should_be_successful() => Result.IsSuccessful.ShouldBeTrue();

                        [Test]
                        public void Then_script_path_info_provider_should_not_have_been_called()
                            => A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options)).MustNotHaveHappened();

                        [Test]
                        public void Then_upgrade_should_be_required() => Result.RequiresUpgrade.ShouldBeTrue();

                        [Test]
                        public void Then_upgrade_should_not_have_occurred()
                            => A.CallTo(() => UpgradeEngineWrapper.PerformUpgrade())
                                .MustNotHaveHappened();

                        [Test]

                        // Note: it is not necessary to call DbUp - we know implicitly that an upgrade is required, since the VersionLevel table exists
                        public void Then_whatif_analysis_should_not_have_occurred()
                            => A.CallTo(() => UpgradeEngineWrapper.IsUpgradeRequired())
                                .MustNotHaveHappened();
                    }

                    [TestFixture]
                    public class And_using_deploy_verb : And_it_has_a_version_level_table
                    {
                        [TestFixture]
                        public class And_extensions_are_installed : And_it_has_a_version_level_table
                        {
                            protected override void Arrange()
                            {
                                base.Arrange();

                                A.CallTo(() => EdFiLegacyDatabaseRepository.FindAllVersionLevels(A<string>._))
                                    .Returns(
                                        new[]
                                        {
                                            new DatabaseVersionLevel
                                            {
                                                DatabaseType = "EDFI",
                                                ScriptSource = "Ed-Fi-ODS",
                                                VersionLevel = 30,
                                                ScriptType = "Structure"
                                            },
                                            new DatabaseVersionLevel
                                            {
                                                DatabaseType = "EDFI",
                                                ScriptSource = "GrandBend",
                                                VersionLevel = 30,
                                                ScriptType = "Structure"
                                            }
                                        });
                            }

                            [Test]
                            public void Then_command_should_be_not_successful() => Result.IsSuccessful.ShouldBeFalse();

                            [Test]
                            public void Then_script_files_should_be_written_to_the_deploy_journal()
                                => A.CallTo(
                                    () => EdFiLegacyDatabaseRepository.InsertIntoDeployJournal(
                                        ConnectionString, A<IEnumerable<DeployJournalRecord>>.That.IsEmpty())).MustHaveHappened();

                            [Test]
                            public void Then_script_path_info_provider_should_have_been_called()
                                => A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options)).MustHaveHappened();

                            [Test]
                            public void Then_should_create_the_deploy_journal_table()
                                => A.CallTo(() => UpgradeEngineWrapper.PerformUpgrade())
                                    .MustHaveHappened();

                            [Test]
                            public void Then_upgrade_should_not_be_required() => Result.RequiresUpgrade.ShouldBeFalse();

                            [Test]
                            public void Then_version_level_table_should_have_been_dropped()
                                => A.CallTo(() => EdFiLegacyDatabaseRepository.DropVersionLevelTable(ConnectionString))
                                    .MustHaveHappened();

                            [Test]
                            public void Then_whatif_analysis_should_not_have_occurred()
                                => A.CallTo(() => UpgradeEngineWrapper.IsUpgradeRequired())
                                    .MustNotHaveHappened();
                        }

                        [TestFixture]
                        public class And_change_queries_is_installed : And_it_has_a_version_level_table
                        {
                            protected const string FileName1010 = "1010-whatever.sql";
                            protected const string OdsStructureChangesPath = "EdFi.Ods.Standard.Artifacts.MsSql.Structure.Ods.Changes";
                            protected static readonly string ExpectedScriptName = $"{OdsStructureChangesPath}.{FileName1010}";

                            protected override void Arrange()
                            {
                                base.Arrange();

                                A.CallTo(() => EdFiLegacyDatabaseRepository.FindAllVersionLevels(A<string>._))
                                    .Returns(
                                        new[]
                                        {
                                            new DatabaseVersionLevel
                                            {
                                                DatabaseType = "EDFI",
                                                ScriptSource = "Ed-Fi-ODS",
                                                VersionLevel = 1010,
                                                ScriptType = "Structure",
                                                SubType = "Changes"
                                            }
                                        });

                                var scriptPathInfo = A.Fake<IScriptPathInfo>();
                                A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options))
                                    .Returns(new[] { scriptPathInfo }.ToList());


                                var scriptFileVersionLevelList = new[]
                                {
                                    new ScriptFileVersionLevel(OdsStructureChangesPath,FileName1010)
                                };

                                A.CallTo(() => scriptPathInfo.GetAllScriptFiles())
                                    .Returns(scriptFileVersionLevelList);
                            }

                            [Test]
                            public void Then_command_should_be_successful() => Result.IsSuccessful.ShouldBeTrue();

                            [Test]
                            public void Then_script_files_should_be_written_to_the_deploy_journal()
                                => A.CallTo(
                                    () => EdFiLegacyDatabaseRepository.InsertIntoDeployJournal(
                                        ConnectionString, A<IEnumerable<DeployJournalRecord>>.That.Contains(new DeployJournalRecord { ScriptName = ExpectedScriptName })))
                                    .MustHaveHappened();

                            [Test]
                            public void Then_script_path_info_provider_should_have_been_called()
                                => A.CallTo(() => ScriptPathInfoProvider.FindAllScriptsInFileSystem(Options)).MustHaveHappened();

                            [Test]
                            public void Then_should_create_the_deploy_journal_table()
                                => A.CallTo(() => UpgradeEngineWrapper.PerformUpgrade())
                                    .MustHaveHappened();

                            [Test]
                            public void Then_upgrade_should_not_be_required() => Result.RequiresUpgrade.ShouldBeFalse();

                            [Test]
                            public void Then_version_level_table_should_have_been_dropped()
                                => A.CallTo(() => EdFiLegacyDatabaseRepository.DropVersionLevelTable(ConnectionString))
                                    .MustHaveHappened();

                            [Test]
                            public void Then_whatif_analysis_should_not_have_occurred()
                                => A.CallTo(() => UpgradeEngineWrapper.IsUpgradeRequired())
                                    .MustNotHaveHappened();
                        }
                    }
                }
            }
        }
    }
}
