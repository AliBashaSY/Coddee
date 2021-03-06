﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Notification
{
    /// <summary>
    /// Application notification service
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send a notification to the application.
        /// </summary>
        void Notify(INotification notification);
    }
}