﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Data;
using HR.Data.Models;

namespace HR.Data.Repositories
{
    public interface ICompanyRepository:ICRUDRepository<Company,Guid>
    {
        Task<IEnumerable<Company>> GetDetailedItems();
    }
}
