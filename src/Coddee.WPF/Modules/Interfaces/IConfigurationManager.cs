﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.WPF.Modules.Interfaces
{
    /// <summary>
    /// service for storing application configurations in a file
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Initialize the configurations manager
        /// On calling this method the configurations will be created or loaded if it exists
        /// </summary>
        /// <param name="configFile">The file path</param>
        void Initialize(string configFile = "config");

        /// <summary>
        /// Upsert a configuration value
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The new value</param>
        void SetValue(string key, object value);

        /// <summary>
        /// Try to read configuration value
        /// </summary>
        /// <typeparam name="TResult">The result value type</typeparam>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The new value</param>
        bool TryGetValue<TResult>(string key, out TResult value);

        /// <summary>
        /// When called the configuration manager will use encrpyted configuration file
        /// </summary>
        /// <param name="key">Encrpytion key</param>
        void SetEncrpytion(string key);

        /// <summary>
        /// Read the configuration file values
        /// </summary>
        void ReadFile();
    }
}