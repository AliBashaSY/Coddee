﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee;
using Coddee.Collections;
using Coddee.Data;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Companies.Editors
{
    public class CompanyEditorViewModel : EditorViewModel<CompanyEditorView,ICompanyRepository,Company, Guid>
    {
       
        private AsyncObservableCollection<State> _states;
        public AsyncObservableCollection<State> States
        {
            get { return _states; }
            set { SetProperty(ref this._states, value); }
        }

        public override void PreSave()
        {
            EditedItem.StateID = States.SelectedItem.ID;
            EditedItem.StateName = States.SelectedItem.Name;
            base.PreSave();
        }
        


        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            States = AsyncObservableCollection<State>.Create(await Resolve<IStateRepository>().GetItems());
        }
    }
}