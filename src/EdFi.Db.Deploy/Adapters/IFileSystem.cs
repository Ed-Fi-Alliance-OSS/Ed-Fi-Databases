// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;

namespace EdFi.Db.Deploy.Adapters
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);

        bool FileExists(string path);

        string ParentDirectory(string path);

        string[] GetFiles(string path, string searchPattern, SearchOption enumerationOption);
    }
}
