// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using DbUp.Engine;
using EdFi.Db.Deploy.Helpers;

namespace EdFi.Db.Deploy.Adapters
{
    /// <summary>
    /// The thin adapter on DbUp functionality to facilitate mock-creation in unit tests.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UpgradeEngineWrapper : IUpgradeEngineWrapper
    {
        private readonly UpgradeEngine _upgradeEngine;

        public UpgradeEngineWrapper(UpgradeEngine upgradeEngine)
        {
            _upgradeEngine = Preconditions.ThrowIfNull(upgradeEngine, nameof(upgradeEngine));
        }

        public DatabaseUpgradeResult PerformUpgrade() => _upgradeEngine.PerformUpgrade();
    
        public bool IsUpgradeRequired() => _upgradeEngine.IsUpgradeRequired();
    }
}
