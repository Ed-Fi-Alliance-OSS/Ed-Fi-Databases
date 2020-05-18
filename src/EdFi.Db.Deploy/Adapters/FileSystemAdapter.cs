// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace EdFi.Db.Deploy.Adapters
{
    [ExcludeFromCodeCoverage]
    public class FileSystemAdapter : IFileSystem
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public bool FileExists(string path) => File.Exists(path);

        public string ParentDirectory(string path) => new FileInfo(path).DirectoryName;

        public string[] GetFiles(string path, string searchPattern, SearchOption enumerationOption) => Directory.GetFiles(path, searchPattern, enumerationOption);
    }
}
