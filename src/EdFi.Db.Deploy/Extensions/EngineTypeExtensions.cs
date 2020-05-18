// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Db.Deploy.Extensions
{
    public static class EngineTypeExtensions
    {
        public static string Directory(this EngineType engineType)
        {
            switch (engineType)
            {
                case EngineType.SqlServer:
                    return "MsSql";

                case EngineType.PostgreSql:
                    return "PgSql";

                default:
                    throw new ArgumentOutOfRangeException($"EngineType \"{engineType}\" is not found.");
            }
        }
    }
}
