// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Db.Deploy.Parameters;

namespace EdFi.Db.Deploy.Specifications
{
    public abstract class OptionsSpecification : ISpecification<IOptions>
    {
        protected OptionsSpecification()
        {
            ErrorMessages = new List<string>();
        }

        public abstract bool IsSatisfiedBy(IOptions obj);

        public List<string> ErrorMessages { get; }
    }
}
