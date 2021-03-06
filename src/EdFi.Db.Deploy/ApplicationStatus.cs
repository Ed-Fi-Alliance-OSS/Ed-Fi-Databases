﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Db.Deploy
{
    public static class ApplicationStatus
    {
        public const int Failure = -1;
        public const int Success = 0;
        public const int UpgradeIsRequired = 1;
    }
}
