﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Data.Linq;

namespace Coddee.Data.LinqToSQL
{
    /// <summary>
    /// Defines the requirements for a LinqToSQL repository
    /// </summary>
    /// <typeparam name="TDataContext"></typeparam>
    public interface ILinqRepository<TDataContext> : IRepository where TDataContext : DataContext
    {
        void Initialize(
            LinqDBManager<TDataContext> dbManager,
            IRepositoryManager repositoryManager,
            IObjectMapper mapper, Type implementedInterface);
    }
}