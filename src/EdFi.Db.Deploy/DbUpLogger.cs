// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using DbUp.Engine.Output;
using EdFi.Db.Deploy.Helpers;
using log4net;

namespace EdFi.Db.Deploy
{
    public class DbUpLogger : IUpgradeLog
    {
        private readonly ILog _logger;

        public DbUpLogger(ILog logger)
        {
            _logger = Preconditions.ThrowIfNull(logger, nameof(logger));
        }

        public void WriteInformation(string format, params object[] args)
        {
            Preconditions.ThrowIfNull(format, nameof(format));
            Preconditions.ThrowIfNull(args, nameof(args));

            _logger.Info(string.Format(format, args));
        }

        public void WriteError(string format, params object[] args)
        {
            Preconditions.ThrowIfNull(format, nameof(format));
            Preconditions.ThrowIfNull(args, nameof(args));

            _logger.Error(string.Format(format, args));
        }

        public void WriteWarning(string format, params object[] args)
        {
            Preconditions.ThrowIfNull(format, nameof(format));
            Preconditions.ThrowIfNull(args, nameof(args));

            _logger.Warn(string.Format(format, args));
        }
    }
}
