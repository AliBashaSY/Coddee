﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Threading;
using System.Windows;
using Coddee.AppBuilder;
using Coddee.ModuleDefinitions;
using Coddee.Mvvm;
using Coddee.Services;

namespace Coddee.WPF
{
    /// <summary>
    /// WPF application builder.
    /// </summary>
    public interface IWPFApplicationBuilder : IApplicationBuilder
    {

    }

    /// <inheritdoc cref="IWPFApplicationBuilder"/>
    public class WPFApplicationBuilder : WindowsApplicationBuilder, IWPFApplicationBuilder
    {
        private Application _systemApplication => ((WPFApplication)_app).GetSystemApplication();

        /// <inheritdoc />
        public WPFApplicationBuilder(WPFApplication app, IContainer container)
            : base(app, container)
        {
            _systemApplication.Startup += delegate { Start(); };
        }

        /// <inheritdoc />
        public override void Start()
        {
            _systemApplication.Dispatcher.Invoke(() =>
            {
                UISynchronizationContext.SetContext(SynchronizationContext.Current);
            });
            base.Start();
        }


        /// <inheritdoc />
        protected override void SetupDefaultBuildActions()
        {
            base.SetupDefaultBuildActions();
            if (BuildActionsCoordinator.GetAction(BuildActionsKeys.ApplicationTheme) == null)
                this.UseTheme(ApplicationColors.Default);
        }

        protected override IApplicationModulesManager CreateApplicationModuleManager(IContainer container)
        {
            var moduleManager = container.RegisterInstance<IWindowsApplicationModulesManager, WindowsApplicationModulesManager>(); 
            container.RegisterInstance<IWindowsApplicationModulesManager>(moduleManager);
            container.RegisterInstance<IApplicationModulesManager>(moduleManager);
            return moduleManager;
        }

        /// <inheritdoc />
        protected override Type[] GetDefaultModules()
        {
            return base.GetDefaultModules().Concat(WPFModuleDefinitions.Modules).ToArray();
        }

        /// <summary>
        /// Set the ViewModelBase dependencies.
        /// </summary>
        protected override void SetupViewModelBase()
        {
            base.SetupViewModelBase();
            Log($"Setting up Wpf ViewModelBase.");
            ViewModelBase.SetContainer(_container);
            ViewModelEvent.SetViewModelManager(_container.Resolve<IViewModelsManager>());
        }

        private StartupEventArgs _startupEventArgs;

        internal void SetStartupArgs(StartupEventArgs startupEventArgs)
        {
            _startupEventArgs = startupEventArgs;
        }
    }
}