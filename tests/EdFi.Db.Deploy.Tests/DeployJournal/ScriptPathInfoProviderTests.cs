// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Db.Deploy.Adapters;
using EdFi.Db.Deploy.Parameters;
using FakeItEasy;
using NUnit.Framework;
using EdFi.Db.Deploy.DeployJournal;
using Shouldly;

// ReSharper disable InconsistentNaming
namespace EdFi.Db.Deploy.Tests.DeployJournal
{
    [TestFixture]
    public class ScriptPathInfoProviderTests : TestFixtureBase
    {
        protected IOptions MockOptions;
        protected IFileSystem MockFileSystem;
        protected IScriptPathInfoProvider Provider;

        protected override void Arrange()
        {
            MockOptions = A.Fake<IOptions>();
            MockFileSystem = A.Fake<IFileSystem>();

            Provider = new ScriptPathInfoProvider(MockFileSystem);
        }

        [TestFixture]
        public class WhenRetrievingAllScriptPathsInfo : ScriptPathInfoProviderTests
        {
            protected IReadOnlyList<IScriptPathInfo> Result;

            protected override void Act()
            {
                Result = Provider.FindAllScriptsInFileSystem(MockOptions);
            }

            [TestFixture]
            public class Given_there_are_core_scripts : WhenRetrievingAllScriptPathsInfo
            {
                private const string BasePath = "c:\\ed-fi-ods-implementation";
                private const string StructureScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Structure\\Ods";
                private const string DataScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Data\\Ods";
                private const string FeatureScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Structure\\Ods\\Changes";
                private const string AdminStructureScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Structure\\Admin";

                protected override void Arrange()
                {
                    base.Arrange();

                    MockOptions.FilePaths = new[] { BasePath };
                    MockOptions.Engine = EngineType.SqlServer;
                    MockOptions.DatabaseType = DatabaseType.ODS;

                    A.CallTo(() => MockFileSystem.DirectoryExists(StructureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(DataScripts))
                        .Returns(true);

                    // These next two are set to make sure they _don't_ turn up in the results.
                    A.CallTo(() => MockFileSystem.DirectoryExists(FeatureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(AdminStructureScripts))
                        .Returns(true);
                }

                [TestFixture]
                public class And_no_features : Given_there_are_core_scripts
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        MockOptions.Features = new string[0];
                    }


                    [Test]
                    public void Then_two_script_paths_should_be_found()
                    {
                        Result.Count.ShouldBe(2);
                    }

                    [Test]
                    public void Then_results_include_core_structure_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == StructureScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_core_data_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == DataScripts).ShouldNotBeNull();
                    }
                }

                [TestFixture]
                public class And_user_requested_changes_feature : Given_there_are_core_scripts
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        MockOptions.Features = new[] { "Changes" };
                    }

                    [Test]
                    public void Then_three_script_paths_should_be_found()
                    {
                        Result.Count.ShouldBe(3);
                    }

                    [Test]
                    public void Then_results_include_core_structure_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == StructureScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_core_data_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == DataScripts).ShouldNotBeNull();
                    }


                    [Test]
                    public void Then_results_include_structure_changes_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == FeatureScripts).ShouldNotBeNull();
                    }

                }
            }

            [TestFixture]
            public class Given_there_are_no_core_scripts_for_the_database : WhenRetrievingAllScriptPathsInfo
            {
                private const string BasePath = "c:\\ed-fi-ods-implementation";
                private const string StructureScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Structure\\Ods";
                private const string DataScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Data\\Ods";
                private const string FeatureScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Structure\\Ods\\Changes";
                private const string AdminStructureScripts = "c:\\ed-fi-ods-implementation\\Artifacts\\MsSql\\Structure\\Admin";

                protected override void Arrange()
                {
                    base.Arrange();

                    MockOptions.FilePaths = new[] { BasePath };
                    MockOptions.Engine = EngineType.SqlServer;
                    MockOptions.DatabaseType = DatabaseType.ODS;

                    A.CallTo(() => MockFileSystem.DirectoryExists(StructureScripts))
                        .Returns(false);

                    A.CallTo(() => MockFileSystem.DirectoryExists(DataScripts))
                        .Returns(false);

                    // Still true - make sure this doesn't show up
                    A.CallTo(() => MockFileSystem.DirectoryExists(FeatureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(AdminStructureScripts))
                        .Returns(true);
                }


                [Test]
                public void Then_no_script_paths_should_be_found()
                {
                    Result.Count.ShouldBe(0);
                }
            }

            [TestFixture]
            public class Given_there_are_extension_scripts : WhenRetrievingAllScriptPathsInfo
            {
                private const string BasePath = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension";
                private const string StructureScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Structure\\Ods";
                private const string DataScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Data\\Ods";
                private const string FeatureScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Structure\\Ods\\Changes";
                private const string AdminStructureScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Structure\\Admin";

                protected override void Arrange()
                {
                    base.Arrange();

                    MockOptions.FilePaths = new[] { BasePath };
                    MockOptions.Engine = EngineType.SqlServer;
                    MockOptions.DatabaseType = DatabaseType.ODS;

                    A.CallTo(() => MockFileSystem.DirectoryExists(StructureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(DataScripts))
                        .Returns(true);

                    // These next set is to make sure they _don't_ turn up in the results.
                    A.CallTo(() => MockFileSystem.DirectoryExists(FeatureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(AdminStructureScripts))
                        .Returns(true);
                }

                [TestFixture]
                public class And_no_features : Given_there_are_extension_scripts
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        MockOptions.Features = new string[0];
                    }

                    [Test]
                    public void Then_two_script_paths_should_be_found()
                    {
                        Result.Count.ShouldBe(2);
                    }

                    [Test]
                    public void Then_results_include_extension_structure_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == StructureScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_extension_data_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == DataScripts).ShouldNotBeNull();
                    }
                }

                [TestFixture]
                public class And_user_requested_changes_feature : Given_there_are_extension_scripts
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        MockOptions.Features = new[] { "Changes" };
                    }

                    [Test]
                    public void Then_three_script_paths_should_be_found()
                    {
                        Result.Count.ShouldBe(3);
                    }

                    [Test]
                    public void Then_results_include_extension_structure_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == StructureScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_extension_data_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == DataScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_extension_structure_changes_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == FeatureScripts).ShouldNotBeNull();
                    }
                }
            }

            [TestFixture]
            public class Given_there_are_no_extension_scripts : WhenRetrievingAllScriptPathsInfo
            {
                private const string BasePath = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension";
                private const string StructureScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Structure\\Ods";
                private const string DataScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Data\\Ods";
                private const string FeatureScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Structure\\Ods\\Changes";
                private const string AdminStructureScripts = "c:\\ed-fi-ods-implementation\\edfi.glendale.extension\\Artifacts\\MsSql\\Structure\\Admin";

                protected override void Arrange()
                {
                    base.Arrange();

                    MockOptions.FilePaths = new[] { BasePath };
                    MockOptions.Engine = EngineType.SqlServer;
                    MockOptions.DatabaseType = DatabaseType.ODS;

                    A.CallTo(() => MockFileSystem.DirectoryExists(StructureScripts))
                        .Returns(false);

                    A.CallTo(() => MockFileSystem.DirectoryExists(DataScripts))
                        .Returns(false);

                    // These next set is to make sure they _don't_ turn up in the results.
                    A.CallTo(() => MockFileSystem.DirectoryExists(FeatureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(AdminStructureScripts))
                        .Returns(true);
                }

                [Test]
                public void Then_no_script_paths_should_be_found()
                {
                    Result.Count.ShouldBe(0);
                }
            }

            [TestFixture]
            public class Given_there_are_legacy_extension_scripts : WhenRetrievingAllScriptPathsInfo
            {
                private const string BasePath = "c:\\ed-fi-ods-implementation\\edfi.sample.extension";
                private const string StructureScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Structure\\EdFi";
                private const string DataScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Data\\EdFi";
                private const string FeatureScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Structure\\EdFi\\Changes";
                private const string AdminStructureScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Structure\\EdFiAdmin";

                protected override void Arrange()
                {
                    base.Arrange();

                    MockOptions.FilePaths = new[] { BasePath };
                    MockOptions.Engine = EngineType.SqlServer;
                    MockOptions.DatabaseType = DatabaseType.ODS;

                    A.CallTo(() => MockFileSystem.DirectoryExists(StructureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(DataScripts))
                        .Returns(true);

                    // These next set is to make sure they _don't_ turn up in the results.
                    A.CallTo(() => MockFileSystem.DirectoryExists(FeatureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(AdminStructureScripts))
                        .Returns(true);
                }

                [TestFixture]
                public class And_no_features : Given_there_are_legacy_extension_scripts
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        MockOptions.Features = new string[0];
                    }

                    [Test]
                    public void Then_two_script_paths_should_be_found()
                    {
                        Result.Count.ShouldBe(2);
                    }

                    [Test]
                    public void Then_results_include_legacy_extension_structure_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == StructureScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_legacy_extension_data_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == DataScripts).ShouldNotBeNull();
                    }
                }

                [TestFixture]
                public class And_user_requested_changes_feature : Given_there_are_legacy_extension_scripts
                {
                    protected override void Arrange()
                    {
                        base.Arrange();

                        MockOptions.Features = new[] { "Changes" };
                    }

                    [Test]
                    public void Then_three_script_paths_should_be_found()
                    {
                        Result.Count.ShouldBe(3);
                    }

                    [Test]
                    public void Then_results_include_legacy_extension_structure_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == StructureScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_legacy_extension_data_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == DataScripts).ShouldNotBeNull();
                    }

                    [Test]
                    public void Then_results_include_legacy_extension_structure_changes_script_path()
                    {
                        Result.FirstOrDefault(x => x.ScriptPath == FeatureScripts).ShouldNotBeNull();
                    }
                }
            }

            [TestFixture]
            public class Given_there_are_no_legacy_extension_scripts : WhenRetrievingAllScriptPathsInfo
            {
                private const string BasePath = "c:\\ed-fi-ods-implementation\\edfi.sample.extension";
                private const string StructureScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Structure\\EdFi";
                private const string DataScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Data\\EdFi";
                private const string FeatureScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Structure\\EdFi\\Changes";
                private const string AdminStructureScripts = "c:\\ed-fi-ods-implementation\\edfi.sample.extension\\Database\\Structure\\EdFiAdmin";

                protected override void Arrange()
                {
                    base.Arrange();

                    MockOptions.FilePaths = new[] { BasePath };
                    MockOptions.Engine = EngineType.SqlServer;
                    MockOptions.DatabaseType = DatabaseType.ODS;

                    A.CallTo(() => MockFileSystem.DirectoryExists(StructureScripts))
                        .Returns(false);

                    A.CallTo(() => MockFileSystem.DirectoryExists(DataScripts))
                        .Returns(false);

                    // These next set is to make sure they _don't_ turn up in the results.
                    A.CallTo(() => MockFileSystem.DirectoryExists(FeatureScripts))
                        .Returns(true);

                    A.CallTo(() => MockFileSystem.DirectoryExists(AdminStructureScripts))
                        .Returns(true);
                }

                [Test]
                public void Then_no_script_paths_should_be_found()
                {
                    Result.Count.ShouldBe(0);
                }
            }
        }
    }
}
