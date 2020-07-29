// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using CommandLine;
using EdFi.Db.Deploy.Adapters;
using EdFi.Db.Deploy.DatabaseCommands;
using EdFi.Db.Deploy.DeployJournal;
using EdFi.Db.Deploy.Extensions;
using EdFi.Db.Deploy.Parameters;
using EdFi.Db.Deploy.Parameters.Verbs;
using EdFi.Db.Deploy.Specifications;
using EdFi.Db.Deploy.UpgradeEngineFactories;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace EdFi.Db.Deploy
{
    internal static class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);
        private static IServiceProvider _serviceProvider;

        private static void Main(string[] args)
        {
            ConfigureLogging();

            _logger.Debug("Entered Main, starts parsing");

            var result = new Parser(
                    config =>
                    {
                        config.CaseInsensitiveEnumValues = true;
                        config.HelpWriter = Console.Error;
                        config.AutoVersion = true;
                        config.CaseSensitive = false;
                    })
                .ParseArguments<DeployDatabase, WhatIfExecution>(args);

            if (args != null &&
               (args[0].Equals("help", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("version", StringComparison.InvariantCultureIgnoreCase)))
            {
                Environment.Exit(0);
            }

            int exitCode = result.MapResult(
                (IOptions opts) => RunDatabaseDeployTool(opts),
                ParseErrorOccurred
            );

            _logger.Debug("Exited Main");

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            }

            Environment.Exit(exitCode);

            void ConfigureLogging()
            {
                var assembly = typeof(Program).GetTypeInfo()
                    .Assembly;

                string configPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "log4net.config");

                XmlConfigurator.Configure(LogManager.GetRepository(assembly), new FileInfo(configPath));
            }

            int RunDatabaseDeployTool(IOptions options)
            {
                _serviceProvider = ConfigureServices(new ServiceCollection(), options);

                exitCode = _serviceProvider.GetService<ApplicationRunner>()
                    .Run();

                return exitCode;
            }

            int ParseErrorOccurred(IEnumerable<Error> errors)
            {
                _logger.Error($"Errors occurred while parsing arguments: {string.Join(' ', args)}");
                return -1;
            }
        }

        private static IServiceProvider ConfigureServices(IServiceCollection serviceCollection, IOptions options)
        {
            var assemblies = new[] { Assembly.GetEntryAssembly() };

            serviceCollection.RegisterAllTypes<IDatabaseCommand>(assemblies, ServiceLifetime.Singleton);
            serviceCollection.RegisterAllTypes<ISpecification<IOptions>>(assemblies, ServiceLifetime.Singleton);
            serviceCollection.RegisterAllTypes<IUpgradeEngineFactory>(assemblies, ServiceLifetime.Singleton);
            serviceCollection.RegisterAllTypes<ISqlServerUpgradeEngineFactory>(assemblies, ServiceLifetime.Singleton);
            serviceCollection.RegisterAllTypes<ICompositeSpecification>(assemblies, ServiceLifetime.Singleton);

            serviceCollection.AddSingleton(options)
                .AddSingleton<ApplicationRunner>()
                .AddSingleton<IDatabaseCommandFactory, DatabaseCommandFactory>()
                .AddSingleton<SqlServerUpgradeEngineFactory>()
                .AddSingleton<PostgresUpgradeEngineFactory>()
                .AddSingleton<IFileSystem, FileSystemAdapter>()
                .AddTransient<IUpgradeEngineWrapper, UpgradeEngineWrapper>()
                .AddTransient<IScriptPathInfoProvider, ScriptPathInfoProvider>()
                .AddTransient<IEdFiLegacyDatabaseRepository, EdFiLegacyDatabaseRepository>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
