﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.Data
{
    /// <summary>
    /// A container for the repositories, responsible for discovering, initializing and retrieving 
    /// the repositories
    /// </summary>
    public interface IRepositoryManager
    {
        void Initialize(IObjectMapper mapper);

        /// <summary>
        /// Gets a repository by its interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        TInterface GetRepository<TInterface>() where TInterface:IRepository;

        /// <summary>
        /// Returns all the registered repositories
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRepository> GetRepositories();

        /// <summary>
        /// Discover and register the repository from assemblies
        /// </summary>
        /// <param name="assemblies">The assemblies to search for the repositories</param>
        /// <returns></returns>
        void RegisterRepositories(params string[] assemblies);

        /// <summary>
        /// Register the provided repositories
        /// </summary>
        /// <param name="repositories">The repositories to register</param>
        /// <returns></returns>
        void RegisterRepositories(params KeyValuePair<Type, Type>[] repositories);
    }
}