﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Data;
using Coddee.Data.Rest;
using HR.Data.Repositories;

namespace HR.Data.REST.Repositories
{
    [Repository(typeof(ICompanyRepository))]
    public class CompanyRepository : CRUDRESTRepositoryBase<Models.Company, Guid>, ICompanyRepository
    {
        public override string ControllerName => "Company";
    }
}