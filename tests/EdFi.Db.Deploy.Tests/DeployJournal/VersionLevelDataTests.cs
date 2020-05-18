// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Db.Deploy.DeployJournal;
using NUnit.Framework;
using Shouldly;
// ReSharper disable InconsistentNaming

namespace EdFi.Db.Deploy.Tests.DeployJournal
{
    [TestFixture]
    public class VersionLevelDataTests : TestFixtureBase
    {
        [TestFixture]
        public class When_testing_if_script_source_is_ods_implementation
        {
            [Test]
            public void Given_lowercase_ods_implementation_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-Fi-Ods-Implementation"
                };

                VersionLevelData.IsAnOdsImplementationRepositoryScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_mixed_case_ods_implementation_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-FI-ODS-IMPLEmentation"
                };

                VersionLevelData.IsAnOdsImplementationRepositoryScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_uppercase_ods_implementation_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "ED-FI-ODS-IMPLEMENTATION"
                };

                VersionLevelData.IsAnOdsImplementationRepositoryScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_null_ods_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = null
                };

                VersionLevelData.IsAnOdsImplementationRepositoryScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_something_else_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-Fi-ODS"
                };

                VersionLevelData.IsAnOdsImplementationRepositoryScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_input_then_response_is_false()
            {
                VersionLevelData.IsAnOdsImplementationRepositoryScript(null)
                    .ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_testing_if_script_source_is_ods
        {
            [Test]
            public void Given_lowercase_ods_implementation_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-Fi-Ods"
                };

                VersionLevelData.IsAnOdsRepositoryScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_mixed_case_ods_implementation_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-FI-ODS"
                };

                VersionLevelData.IsAnOdsRepositoryScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_uppercase_ods_implementation_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "ED-FI-ODS"
                };

                VersionLevelData.IsAnOdsRepositoryScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_null_ods_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = null
                };

                VersionLevelData.IsAnOdsRepositoryScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_something_else_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-Fi-ODS-Implementation"
                };

                VersionLevelData.IsAnOdsRepositoryScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_input_then_response_is_false()
            {
                VersionLevelData.IsAnOdsRepositoryScript(null)
                    .ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_testing_if_version_level_record_is_for_the_ods_database
        {
            [Test]
            public void Given_lowercase_edfi_then_result_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    DatabaseType = "edfi"
                };

                VersionLevelData.IsForTheODSDatabase(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_mixedcase_edfi_then_result_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    DatabaseType = "edFI"
                };

                VersionLevelData.IsForTheODSDatabase(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_uppercase_edfi_then_result_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    DatabaseType = "EDFI"
                };

                VersionLevelData.IsForTheODSDatabase(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_null_then_result_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    DatabaseType = null
                };

                VersionLevelData.IsForTheODSDatabase(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_edfisecurity_then_result_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    DatabaseType = "EDFISECURITY"
                };

                VersionLevelData.IsForTheODSDatabase(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_input_then_response_is_false()
            {
                VersionLevelData.IsForTheODSDatabase(null)
                    .ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_testing_if_script_source_is_not_a_core_script
        {
            [Test]
            public void Given_ods_then_result_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-Fi-ODS"
                };

                VersionLevelData.IsNotACoreScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_ods_implementation_then_result_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "Ed-Fi-ODS"
                };

                VersionLevelData.IsNotACoreScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_then_result_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = null
                };

                VersionLevelData.IsNotACoreScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_something_else_then_result_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptSource = "GrandBend"
                };

                VersionLevelData.IsNotACoreScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_null_input_then_result_is_true()
            {
                VersionLevelData.IsNotACoreScript(null)
                    .ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_testing_if_sub_type_is_for_change_queries
        {
            [Test]
            public void Given_lowercase_changes_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = "changes"
                };

                VersionLevelData.IsAChangeQueriesScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_uppercase_changes_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = "CHANGES"
                };

                VersionLevelData.IsAChangeQueriesScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_mixed_changes_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = "cHANges"
                };

                VersionLevelData.IsAChangeQueriesScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_null_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = null
                };

                VersionLevelData.IsAChangeQueriesScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_input_then_response_is_false()
            {
                VersionLevelData.IsAChangeQueriesScript(null)
                    .ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_testing_if_not_a_feature_script
        {
            [Test]
            public void Given_null_input_then_response_is_true()
            {
                VersionLevelData.IsNotAFeatureScript(null)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_null_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = null
                };

                VersionLevelData.IsNotAFeatureScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_whitespace_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = "   "
                };

                VersionLevelData.IsNotAFeatureScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_changes_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = "changes"
                };

                VersionLevelData.IsNotAFeatureScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_something_else_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    SubType = "something else"
                };

                VersionLevelData.IsNotAFeatureScript(input)
                    .ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_testing_if_a_structure_script
        {
            [Test]
            public void Given_lowercase_structure_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "structure"
                };

                VersionLevelData.IsAStructureScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_mixedcase_structure_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "strUCTure"
                };

                VersionLevelData.IsAStructureScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_upper_structure_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "STRUCTURE"
                };

                VersionLevelData.IsAStructureScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_data_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "data"
                };

                VersionLevelData.IsAStructureScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = null
                };

                VersionLevelData.IsAStructureScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_input_then_response_is_false()
            {
                VersionLevelData.IsAStructureScript(null)
                    .ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_testing_if_a_data_script
        {
            [Test]
            public void Given_lowercase_data_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "data"
                };

                VersionLevelData.IsADataScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_mixedcase_data_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "data"
                };

                VersionLevelData.IsADataScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_upper_data_then_response_is_true()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "DATA"
                };

                VersionLevelData.IsADataScript(input)
                    .ShouldBeTrue();
            }

            [Test]
            public void Given_data_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = "structure"
                };

                VersionLevelData.IsADataScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_then_response_is_false()
            {
                var input = new DatabaseVersionLevel
                {
                    ScriptType = null
                };

                VersionLevelData.IsADataScript(input)
                    .ShouldBeFalse();
            }

            [Test]
            public void Given_null_input_then_response_is_false()
            {
                VersionLevelData.IsADataScript(null)
                    .ShouldBeFalse();
            }
        }

    }
}
