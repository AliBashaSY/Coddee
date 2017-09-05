﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Loggers
{
    public class ConsoleLogger : LoggerBase
    {
        protected override void CommitLog(LogRecord record)
        {
            Console.WriteLine(BuildEvent(record));
        }
    }
}