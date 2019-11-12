﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Messerli.CommandLine;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;
using Messerli.ProjectGenerator.UserInput;
using Messerli.TfsClient;
using Stubble.Core.Builders;
using static Messerli.ProjectGenerator.ExecutableInformation;

namespace Messerli.ProjectGenerator
{
    internal class CompositionRoot
    {
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<SystemConsoleWriter>().As<IConsoleWriter>();
            builder.RegisterType<SystemConsoleReader>().As<IConsoleReader>();
            builder.RegisterType<UserInputProvider>().As<IUserInputProvider>().SingleInstance();
            builder.RegisterType<UserInputDescriptionBuilder>().AsSelf();
            builder.RegisterType<TemplateLoader>().As<ITemplateLoader>();

            builder.RegisterType<ConsoleClient>().As<IClient>();
            builder.RegisterType<FileGenerator>().As<IFileGenerator>();
            builder.RegisterType<StubbleBuilder>().AsSelf();

            builder.RegisterType<ExecutingPluginAssemblyProvider>().As<IExecutingPluginAssemblyProvider>().SingleInstance();

            builder.RegisterType<StringRequester>();
            builder.RegisterType<BoolRequester>();
            builder.RegisterType<IntegerRequester>();
            builder.RegisterType<DoubleRequester>();
            builder.RegisterType<SelectionRequester>();
            builder.RegisterType<PathRequester>();
            builder.RegisterType<ExistingPathRequester>();
            builder.RegisterType<DateRequester>();
            builder.RegisterType<DateTimeRequester>();
            builder.RegisterType<TimeRequester>();

            builder.Register(VariableRequesterFactory).As<IVariableRequester>();

            RegisterPlugins(builder);

            return builder.Build();
        }

        private void RegisterPlugins(ContainerBuilder builder)
        {
            GetExecutableDirectory().Match(
                () => throw new Exception("Failed to get directory of executable."),
                executablePath =>
                {
                    foreach (var path in Directory.GetFiles(Path.Combine(executablePath, "plugins"), "*Projects.dll"))
                    {
                        var registrar = builder.RegisterAssemblyModules(Assembly.LoadFile(path));
                    }
                });
        }

        private static IVariableRequester VariableRequesterFactory(IComponentContext context, IEnumerable<Parameter> paremeter)
        {
            var variableType = paremeter.TypedAs<VariableType>();

            return variableType switch
            {
                VariableType.String => context.Resolve<StringRequester>(),
                VariableType.Bool => context.Resolve<BoolRequester>(),
                VariableType.Integer => context.Resolve<IntegerRequester>(),
                VariableType.Double => context.Resolve<DoubleRequester>(),
                VariableType.Selection => context.Resolve<SelectionRequester>(),
                VariableType.Path => context.Resolve<PathRequester>(),
                VariableType.ExistingPath => context.Resolve<ExistingPathRequester>(),
                VariableType.Date => context.Resolve<DateRequester>(),
                VariableType.DateTime => context.Resolve<DateTimeRequester>(),
                VariableType.Time => context.Resolve<TimeRequester>(),

                _ => throw new NotImplementedException($"The VariableType '{variableType}' is not supported at the moment..."),
            };
        }
    }
}