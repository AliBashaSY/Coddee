﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Loggers
{
    /// <summary>
    /// Redirects it's log to multiple loggers
    /// </summary>
    public class LogAggregator : LoggerBase
    {
        private readonly IList<ILogger> _loggers;
        private readonly IList<LogRecord> _records;

        public LogAggregator()
        {
            _loggers = new List<ILogger>();
            _records = new List<LogRecord>();
        }
        
        protected override void CommitLog(LogRecord record)
        {
            _records.Add(record);
            foreach (var logger in _loggers)
                logger.Log(record);
        }

        public void AddLogger(ILogger logger)
        {
            foreach (var log in _records)
                if (log.Type >= _minimumLevel)
                    logger.Log(log);
            _loggers.Add(logger);
        }
    }
}