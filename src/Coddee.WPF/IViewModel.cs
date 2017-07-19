﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Coddee.Validation;

namespace Coddee.WPF
{
    public interface IViewModel:IDisposable,INotifyPropertyChanged
    {
        event EventHandler Initialized;
        event EventHandler<IViewModel> ChildCreated;

        IViewModel ParentViewModel { get; set; }
        IList<IViewModel> ChildViewModels { get; }

        bool IsInitialized { get; }
        Task Initialize();

        RequiredFieldCollection RequiredFields { get; }
    }

    public interface IPresentableViewModel : IViewModel, IPresentable
    {
        
    }
}