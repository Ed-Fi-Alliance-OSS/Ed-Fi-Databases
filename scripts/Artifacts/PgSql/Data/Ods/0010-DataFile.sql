-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

BEGIN; 

INSERT INTO edfi.SampleTable (SampleTableId, TextColumn) VALUES (1, 'First');
INSERT INTO edfi.SampleTable (SampleTableId, TextColumn) VALUES (2, 'Second');
INSERT INTO edfi.SampleTable (SampleTableId, TextColumn) VALUES (3, 'Third');

COMMIT;
