// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Db.Deploy.ScriptPathResolvers {
    public abstract class ScriptPathResolverBase : IScriptPathResolver
    {
        protected readonly DatabaseType _databaseType;
        protected readonly string _feature;
        protected readonly string _path;

        protected ScriptPathResolverBase(string path, DatabaseType databaseType, string feature = null)
        {
            _databaseType = databaseType;
            _feature = feature;
            _path = path;
        }

        public abstract string StructureScriptPath();

        public abstract string DataScriptPath();
        protected abstract string DatabaseTypeDirectory();
    }
}
