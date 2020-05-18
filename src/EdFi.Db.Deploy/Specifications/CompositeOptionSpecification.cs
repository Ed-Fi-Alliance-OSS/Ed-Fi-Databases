// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Db.Deploy.Helpers;
using EdFi.Db.Deploy.Parameters;

namespace EdFi.Db.Deploy.Specifications
{
    public abstract class CompositeOptionSpecification : ICompositeSpecification
    {
        protected CompositeOptionSpecification(IEnumerable<ISpecification<IOptions>> specifications)
        {
            Specifications = Preconditions.ThrowIfNull(specifications?.ToList(), nameof(specifications));

            ErrorMessages = new List<string>();
        }

        protected IList<ISpecification<IOptions>> Specifications { get; }

        public List<string> ErrorMessages { get; }

        public bool IsSatisfiedBy(object obj)
        {
            Preconditions.ThrowIfNull(obj, nameof(obj));

            foreach (ISpecification<IOptions> specification in FilteredSpecifications())
            {
                specification.IsSatisfiedBy((IOptions) obj);
                ErrorMessages.AddRange(specification.ErrorMessages);
            }

            return !ErrorMessages.Any();
        }

        protected abstract IEnumerable<ISpecification<IOptions>> FilteredSpecifications();
    }
}
