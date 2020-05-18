// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.ScriptProviders;
using EdFi.Db.Deploy.Adapters;
using EdFi.Db.Deploy.Helpers;

namespace EdFi.Db.Deploy.UpgradeEngineFactories
{
    [ExcludeFromCodeCoverage]
    public abstract class UpgradeEngineFactory : IUpgradeEngineFactory
    {
        public UpgradeEngine Create(UpgradeEngineConfig config)
        {
            Preconditions.ThrowIfNull(config, nameof(config));

            return UpgradeEngineBuilder(config.ConnectionString)
                .WithScriptsFromFileSystem(
                    config.ParentFolder(),
                    new FileSystemScriptOptions
                    {
                        Filter = s => new FileInfo(s).DirectoryName.Equals(
                            config.ScriptPath,
                            StringComparison.InvariantCultureIgnoreCase),
                        IncludeSubDirectories = true
                    })
                .WithExecutionTimeout(TimeSpan.FromSeconds(config.TimeoutInSeconds))
                .Build();
        }

        public IUpgradeEngineWrapper Create(UpgradeEngineConfig config, params SqlScript[] scripts)
        {
            Preconditions.ThrowIfNull(config, nameof(config));
            Preconditions.ThrowIfNull(scripts, nameof(scripts));

            return new UpgradeEngineWrapper(
                UpgradeEngineBuilder(config.ConnectionString)
                    .WithScripts(scripts)
                    .WithExecutionTimeout(TimeSpan.FromSeconds(config.TimeoutInSeconds))
                    .Build());
        }

        protected abstract UpgradeEngineBuilder UpgradeEngineBuilder(string connectionString);
    }
}
