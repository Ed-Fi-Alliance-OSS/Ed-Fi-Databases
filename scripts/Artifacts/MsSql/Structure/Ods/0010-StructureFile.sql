-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'edfi')
    EXEC sys.sp_executesql N'CREATE SCHEMA [edfi]'
GO

-- Table [edfi].[SampleTable] --
CREATE TABLE [edfi].[SampleTable] (
    [SampleTableId] [INT] NOT NULL,
    [TextColumn] [NVARCHAR] (20) NOT NULL,
    CONSTRAINT [SampleTable_PK] PRIMARY KEY CLUSTERED (
        [SampleTableId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
