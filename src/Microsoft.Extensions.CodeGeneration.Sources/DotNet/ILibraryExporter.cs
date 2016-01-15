﻿using System;
using System.Collections.Generic;
using Microsoft.DotNet.ProjectModel.Compilation;

namespace Microsoft.Extensions.CodeGeneration.Sources.DotNet
{
    public interface ILibraryExporter
    {
        IEnumerable<LibraryExport> GetAllExports();
        LibraryExport GetExport(string name);
    }
}
