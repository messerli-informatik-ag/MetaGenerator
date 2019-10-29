﻿using Autofac;
using Messerli.ProjectAbstractions;

namespace Messerli.OneCoreProjects
{
    public class OneCoreProjectsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OneCoreLibraryGenerator>().As<IProjectGenerator>();
            builder.RegisterType<ProjectInformationProvider>().As<IProjectInformationProvider>();
        }
    }
}
