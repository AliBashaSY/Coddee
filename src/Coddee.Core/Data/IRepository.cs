﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.Data
{
    /// <summary>
    /// A configuration object that will be passed to the repository on initialization.
    /// </summary>
    public class RepositoryConfigurations
    {
        /// <summary>
        /// An encryption key that can be used to decrypt the data.
        /// </summary>
        public string EncryptionKey { get; set; }
    }

    /// <summary>
    /// A data repository
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// The type of the repository <seealso cref="RepositoryTypes"/>
        /// </summary>
        int RepositoryType { get; }

        /// <summary>
        /// Indicates that the repository is initialized and ready to use.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// The IRepository interface that this repository implements.
        /// </summary>
        Type ImplementedInterface { get; }

        /// <summary>
        /// Do any required initialization
        /// </summary>
        void Initialize(IRepositoryManager repositoryManager,
                        IObjectMapper mapper,
                        Type implementedInterface,
                        RepositoryConfigurations config = null);

        /// <summary>
        /// Set the sync service to be used in the repository
        /// </summary>
        void SetSyncService(IRepositorySyncService syncService, bool sendSyncRequests = true);

        /// <summary>
        /// Sets the context object that may change the repository behavior according to the context
        /// <remarks>The context may change on each call to the repository.</remarks>
        /// </summary>
        void SetContext(object context);
    }

    /// <summary>
    /// A data repository
    /// </summary>
    public interface IRepository<TModel, TKey> : IRepository
        where TModel : IUniqueObject<TKey>
    {
        /// <summary>
        /// This event will be triggered when an item is added, edited or deleted in the repository
        /// </summary>
        event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;
    }

    /// <summary>
    /// A data repository contains an indexer to retrieve an item by its key
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The Key(ID) Type</typeparam>
    public interface IIndexedRepository<TModel, TKey> : IRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>
    {


        /// <summary>
        /// Return the item by its key(ID)
        /// </summary>
        /// <param name="index">The item Key(ID)</param>
        /// <returns></returns>
        Task<TModel> this[TKey index]
        {
            [ApiAction("{repository}/GetItem",HttpMethod.Get)]
            get;
        }
    }

    /// <summary>
    /// A data repository for read only functionality
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The Key(ID) Type</typeparam>
    public interface IReadOnlyRepository<TModel, TKey> : IIndexedRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>

    {
        /// <summary>
        /// Returns all the items in the repository
        /// </summary>
        /// <returns></returns>
        [ApiAction("{repository}/GetItems",HttpMethod.Get)]
        Task<IEnumerable<TModel>> GetItems();
    }

    /// <summary>
    /// A data repository that support CURD operations(Create,Update,Retrieve,Delete)
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The Key(ID) Type</typeparam>
    public interface ICRUDRepository<TModel, TKey> : IReadOnlyRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>

    {

        /// <summary>
        /// Updates and items in the repository
        /// </summary>
        [ApiAction("{repository}/UpdateItem", HttpMethod.Post)]
        Task<TModel> UpdateItem(TModel item);

        /// <summary>
        /// Inserts a new items to the repository
        /// </summary>
        [ApiAction("{repository}/InsertItem", HttpMethod.Post)]
        Task<TModel> InsertItem(TModel item);

        /// <summary>
        /// Deletes an item from the repository by it's key
        /// </summary>
        [ApiAction("{repository}/DeleteItemByKey", HttpMethod.Post)]
        Task DeleteItemByKey(TKey ID);

        /// <summary>
        /// Deletes an item from the repository 
        /// </summary>
        [ApiAction("{repository}/DeleteItem", HttpMethod.Post)]
        Task DeleteItem(TModel item);
    }
}