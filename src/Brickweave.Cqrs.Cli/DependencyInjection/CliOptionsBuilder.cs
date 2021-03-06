﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Brickweave.Cqrs.Cli.Factories;
using Brickweave.Cqrs.Cli.Factories.ParameterValues;
using Brickweave.Cqrs.Cli.Readers;
using Microsoft.Extensions.DependencyInjection;

namespace Brickweave.Cqrs.Cli.DependencyInjection
{
    public class CliOptionsBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IList<IExecutableRegistration> _executableRegistrations = new List<IExecutableRegistration>();

        private CultureInfo _culture;

        public CliOptionsBuilder(IServiceCollection services, params Assembly[] domainAssemblies)
        {
            var executables = domainAssemblies.SelectMany(a => a.ExportedTypes)
                .Where(t => typeof(IExecutable).IsAssignableFrom(t.GetTypeInfo()))
                .ToList();
            
            services
                .AddScoped<ICliDispatcher, CliDispatcher>()
                .AddScoped<IExecutableInfoFactory>(provider =>
                    new ExecutableInfoFactory(_executableRegistrations.ToArray()))
                .AddScoped<IParameterValueFactory, BasicParameterValueFactory>()
                .AddScoped<ISingleParameterValueFactory, BasicParameterValueFactory>()
                .AddScoped<IParameterValueFactory, WrappedBasicParameterValueFactory>()
                .AddScoped<ISingleParameterValueFactory, WrappedBasicParameterValueFactory>()
                .AddScoped<IParameterValueFactory, GuidParameterValueFactory>()
                .AddScoped<ISingleParameterValueFactory, GuidParameterValueFactory>()
                .AddScoped<IParameterValueFactory, WrappedGuidParameterValueFactory>()
                .AddScoped<ISingleParameterValueFactory, WrappedGuidParameterValueFactory>()
                .AddScoped<IParameterValueFactory>(s => new DateTimeParameterValueFactory(_culture))
                .AddScoped<ISingleParameterValueFactory>(s => new DateTimeParameterValueFactory(_culture))
                .AddScoped<IParameterValueFactory>(s => new EnumerableParameterValueFactory(s.GetServices<ISingleParameterValueFactory>()))
                .AddScoped<IParameterValueFactory>(s => new ListParameterValueFactory(s.GetServices<ISingleParameterValueFactory>()))
                .AddScoped<IExecutableFactory>(provider => new ExecutableFactory(
                    provider.GetServices<IParameterValueFactory>(),
                    executables))
                .AddScoped<IExecutableHelpReader>(s => new XmlDocumentationFileHelpReader(
                    _executableRegistrations,
                    domainAssemblies.Select(a => Path.Combine(Path.GetDirectoryName(a.Location), $"{a.GetName().Name}.xml")).ToArray()))
                .AddScoped<IHelpInfoFactory, HelpInfoFactory>();

            _services = services;
        }

        public CliOptionsBuilder OverrideCommandName<T>(string actionName, params string[] subjectNameParts) where T : class, ICommand
        {
            _executableRegistrations.Add(new ExecutableRegistration<T>(actionName, subjectNameParts));
            return this;
        }

        public CliOptionsBuilder OverrideQueryName<T>(string actionName, params string[] subjectNameParts) where T : class, IQuery
        {
            _executableRegistrations.Add(new ExecutableRegistration<T>(actionName, subjectNameParts));
            return this;
        }

        public CliOptionsBuilder OverrideExecutableName<T>(string actionName, params string[] subjectNameParts) where T : class, IExecutable
        {
            _executableRegistrations.Add(new ExecutableRegistration<T>(actionName, subjectNameParts));
            return this;
        }

        public CliOptionsBuilder AddCategoryHelpFile(string filePath)
        {
            _services.AddScoped<ICategoryHelpReader>(services => new JsonFileCategoryHelpReader(filePath));
            return this;
        }

        public CliOptionsBuilder AddDateParsingCulture(CultureInfo cultureInfo)
        {
            _culture = cultureInfo;
            return this;
        }
        
        public IServiceCollection Services()
        {
            return _services;
        }
    }
}
