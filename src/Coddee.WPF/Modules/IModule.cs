﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules
{
    
    public interface IModule
    {
        /// <summary>
        /// Called by the ApplicationModulesManager to initialize the module
        /// </summary>
        /// <param name="container">The dependency container</param>
        void Initialize(IUnityContainer container);
    }
}