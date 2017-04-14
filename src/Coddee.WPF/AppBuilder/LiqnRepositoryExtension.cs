﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.AppBuilder;
using Coddee.Data.LinqToSQL;

namespace Coddee.WPF.AppBuilder
{
    public static class LiqnRepositoryExtension
    {
        public static IWPFApplicationBuilder UseLinqRepositoryManager<TDBManager, TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer,
            Action ConnectionStringNotFound)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {
            builder.SetBuildAction(BuildActions.Repository,delegate
            {
                var connectionString = builder.WPFBuilder.GetSQLDBConnection();
                if (string.IsNullOrEmpty(connectionString))
                {
                    ConnectionStringNotFound();
                    return;
                }
                var dbManager = new TDBManager();
                dbManager.Initialize(connectionString);
                var repositoryManager = new TRepositoryManager();
                repositoryManager.Initialize(dbManager, builder.WPFBuilder.GetMapper());
                repositoryManager.RegisterRepositories(repositoriesAssembly);
                builder.WPFBuilder.SetRepositoryManager(repositoryManager, registerTheRepositoresInContainer);
            });
            return builder;
        }

        public static IWPFApplicationBuilder UseLinqRepositoryManager<TDBManager, TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            string connectionString,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {
            builder.SetBuildAction(BuildActions.Repository,delegate
            {
                var dbManager = new TDBManager();
                dbManager.Initialize(connectionString);
                var repositoryManager = new TRepositoryManager();
                repositoryManager.Initialize(dbManager, builder.WPFBuilder.GetMapper());
                repositoryManager.RegisterRepositories(repositoriesAssembly);
                builder.WPFBuilder.SetRepositoryManager(repositoryManager, registerTheRepositoresInContainer);
            });
            return builder;
        }
    }
}