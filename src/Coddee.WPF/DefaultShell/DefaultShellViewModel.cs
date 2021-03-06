﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Mvvm;
using Coddee.WPF.Commands;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.WPF.DefaultShell
{
    /// <summary>
    /// The default shell viewModel
    /// this viewModel will be used if you don't specify a custom shell at the application build
    /// </summary>
    public class DefaultShellViewModel : ViewModelBase<DefaultShellView>, IDefaultShellViewModel
    {
        /// <summary>
        /// Design time constructor
        /// </summary>
        public DefaultShellViewModel()
        {
            if (IsDesignMode())
            {
                ApplicationName = "HR application";
            }
        }

        #region ToolBar properties

        private string _username;

        /// <summary>
        /// The username of the logged in user.
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref this._username, value); }
        }

        private string _applicationName;

        /// <summary>
        /// The application name.
        /// </summary>
        public string ApplicationName
        {
            get { return _applicationName; }
            set { SetProperty(ref this._applicationName, value); }
        }

        private bool _hasError;

        /// <summary>
        /// Indicates whether any errors has occurred in the application/
        /// </summary>
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty(ref this._hasError, value); }
        }

        private UIElement _toolbarContent;

        /// <summary>
        /// The content  of the tool-bar area.
        /// </summary>
        public UIElement ToolbarContent
        {
            get { return _toolbarContent; }
            set { SetProperty(ref this._toolbarContent, value); }
        }

        
        /// <inheritdoc cref="Minimize"/>
        public ICommand MinimizeCommand => new RelayCommand(Minimize);

        /// <inheritdoc cref="Exit"/>
        public ICommand ExitCommand => new RelayCommand(Exit);

        #endregion

        private IDialogExplorer _dialogExplorer;

        /// <see cref="IDialogExplorer"/>
        public IDialogExplorer DialogExplorer
        {
            get { return _dialogExplorer; }
            set { SetProperty(ref _dialogExplorer, value); }
        }

        private bool _useNavigation;
        /// <summary>
        /// If true the navigation bar will be visible.
        /// </summary>
        public bool UseNavigation
        {
            get { return _useNavigation; }
            set { SetProperty(ref this._useNavigation, value); }
        }

        /// <summary>
        /// Called on exit command
        /// </summary>
        protected virtual void Exit()
        {
            ((WPFApplication)_app).GetSystemApplication().Shutdown();
        }

        /// <summary>
        /// Called on minimize command
        /// </summary>
        protected virtual void Minimize()
        {
            View.WindowState = WindowState.Minimized;
        }

        private IPresentableViewModel _mainViewModel;

        /// <inheritdoc />
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            if (!_mainViewModel.IsInitialized)
                await _mainViewModel.Initialize();
            DialogExplorer = Resolve<IDialogExplorer>();
        }
        
        /// <inheritdoc />
        public IPresentableViewModel SetMainContent(Type defaultPresentable, bool useNavigation)
        {
            _username = _globalVariables.GetVariable<UsernameGlobalVariable>().GetValue();
            _applicationName = _globalVariables.GetVariable<ApplicationNameGlobalVariable>().GetValue();
            
            UseNavigation = useNavigation;
            _mainViewModel = (IPresentableViewModel)CreateViewModel(defaultPresentable);
            DefaultRegions.ApplicationMainRegion.View((IPresentable)_mainViewModel);
            return _mainViewModel;
        }

        /// <inheritdoc />
        public IPresentableViewModel GetMainContent()
        {
            return _mainViewModel;
        }

        /// <inheritdoc />
        public void SetToolbarContent(UIElement content)
        {
            ToolbarContent = content;
        }
    }
}