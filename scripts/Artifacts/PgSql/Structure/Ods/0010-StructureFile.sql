-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE SCHEMA edfi AUTHORIZATION postgres;

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Table edfi.SampleTable --
CREATE TABLE edfi.SampleTable (
    SampleTableId INT NOT NULL,
    TextColumn VARCHAR(20) NOT NULL,
    CONSTRAINT SampleTable_PK PRIMARY KEY (SampleTableId)
);
