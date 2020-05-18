// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Reflection;
using EdFi.Db.Deploy.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace EdFi.Db.Deploy.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterAllTypes<T>(
            this IServiceCollection services,
            Assembly[] assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            Preconditions.ThrowIfNull(assemblies, nameof(assemblies));

            var typesFromAssemblies = assemblies
                .SelectMany(
                    a => a.DefinedTypes
                        .Where(
                            x => x.GetInterfaces()
                                .Contains(typeof(T))));

            foreach (var type in typesFromAssemblies)
            {
                if (!type.IsAbstract)
                {
                    services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
                }
            }
        }
    }
}
