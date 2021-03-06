// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.DotNet.ProjectModel;
using Microsoft.DotNet.ProjectModel.Graph;
using Microsoft.Extensions.CodeGeneration.DotNet;

namespace Microsoft.Extensions.CodeGeneration
{
    public class DefaultCodeGeneratorAssemblyProvider : ICodeGeneratorAssemblyProvider
    {
        private static readonly HashSet<string> _codeGenerationFrameworkAssemblies =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "Microsoft.Extensions.CodeGeneration",
            };

        private readonly ILibraryManager _libraryManager;
        private readonly ICodeGenAssemblyLoadContext _assemblyLoadContext;
        private readonly ILibraryExporter _libraryExporter;
        
        public DefaultCodeGeneratorAssemblyProvider(ILibraryManager libraryManager, ICodeGenAssemblyLoadContext loadContext, ILibraryExporter libraryExporter)
        {
            if (libraryManager == null)
            {
                throw new ArgumentNullException(nameof(libraryManager));
            }
            if(loadContext == null)
            {
                throw new ArgumentNullException(nameof(loadContext));
            }
            if(libraryExporter == null) 
            {
                throw new ArgumentNullException(nameof(libraryExporter));
            }
            _libraryManager = libraryManager;
            _assemblyLoadContext = loadContext;
            _libraryExporter = libraryExporter;
            
        }

        public IEnumerable<Assembly> CandidateAssemblies
        {
            get
            {

                var list = _codeGenerationFrameworkAssemblies
                    .SelectMany(_libraryManager.GetReferencingLibraries)
                    .Distinct()
                    .Where(IsCandidateLibrary);
                
                return list.Select(lib => _assemblyLoadContext.LoadFromPath(_libraryExporter.GetResolvedPathForDependency(lib)));
            }
        }

        private bool IsCandidateLibrary(LibraryDescription library)
        {
            return !_codeGenerationFrameworkAssemblies.Contains(library.Identity.Name) && (!"Project".Equals(library.Identity.Type.Value, StringComparison.OrdinalIgnoreCase));
        }
    }
}