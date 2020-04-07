﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Messerli.MetaGeneratorAbstractions;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.MetaGenerator
{
    public class DotnetToolInstaller : IDotnetToolInstaller
    {
        private const string Dotnet = "dotnet";

        private readonly ITools _tools;

        public DotnetToolInstaller(ITools tools)
        {
            _tools = tools;
            _tools.RegisterTool(Dotnet, "dotnet.exe");
            _tools.VerifyTools();
        }

        public void InstallTool(string path, string toolName, string? version)
        {
            const string dotnetToolsFileName = "dotnet-tools.json";
            var dotnet = _tools.GetTool("dotnet.exe");

            var hasDotnetToolManifest = Directory.GetFiles(path, dotnetToolsFileName, SearchOption.AllDirectories)
                .Any();

            if (!hasDotnetToolManifest)
            {
                dotnet.Execute(new[] { "new", "tool-manifest" }, path);
            }

            dotnet.Execute(GetToolInstallArguments(toolName, version), path);
        }

        private IEnumerable<string> GetToolInstallArguments(string toolName, string? version)
        {
            var toolArguments = new List<string>
            {
                "tool",
                "install",
                toolName,
            };
            toolArguments.AddRange(GetVersionArguments(version));
            return toolArguments;
        }

        private IEnumerable<string> GetVersionArguments(string? version)
            => version is { }
                ? new List<string>()
                : new List<string>
                {
                    "--version",
                    version!,
                };
    }
}