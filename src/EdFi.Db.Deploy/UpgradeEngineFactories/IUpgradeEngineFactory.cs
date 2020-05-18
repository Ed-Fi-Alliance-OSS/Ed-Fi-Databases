// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using DbUp.Engine;
using EdFi.Db.Deploy.Adapters;

namespace EdFi.Db.Deploy.UpgradeEngineFactories
{
    public interface IUpgradeEngineFactory
    {
        UpgradeEngine Create(UpgradeEngineConfig config);

        IUpgradeEngineWrapper Create(UpgradeEngineConfig config, params SqlScript[] scripts);
    }

    public interface ISqlServerUpgradeEngineFactory : IUpgradeEngineFactory
    {
        // This interface exists only to support dependency injection - no additional methods are needed.
    }
}
