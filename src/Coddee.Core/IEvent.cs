﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee
{
    /// <summary>
    /// An application event
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Specifies the way the event will propagate.
        /// </summary>
        EventRoutingStrategy EventRoutingStrategy { get; }
    } 
}
