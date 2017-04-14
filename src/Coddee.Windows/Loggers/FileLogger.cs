﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.IO;

namespace Coddee.Loggers
{
    public class FileLogger : LoggerBase
    {
        private FileInfo _file;

        public void Initialize(LogRecordTypes type, string fileName)
        {
            base.Initialize(type);
            _file = new FileInfo(fileName);
            if (!_file.Exists)
                _file.Create().Dispose();
        }

        protected override async void CommitLog(LogRecord record)
        {
            using (var sw= _file.AppendText())
            {
                await sw.WriteAsync(BuildEvent(record));
            }
        }
    }
}