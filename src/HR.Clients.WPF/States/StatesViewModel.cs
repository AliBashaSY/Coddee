﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading;
using System.Threading.Tasks;
using Coddee.WPF;
using Coddee.WPF.Collections;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.States
{
    public class StatesViewModel : ViewModelBase<StatesView>
    {
        /// <summary>
        /// Design time constructor
        /// </summary>
        public StatesViewModel()
        {
            if (IsDesignMode())
            {
                StatesList = new AsyncObservableCollectionView<State>
                {
                    new State
                    {
                        Name = "State1"
                    },
                    new State
                    {
                        Name = "State2"
                    },
                    new State
                    {
                        Name = "State3"
                    },
                    new State
                    {
                        Name = "State4"
                    },
                    new State
                    {
                        Name = "State5"
                    }
                };
            }
        }

        private AsyncObservableCollectionView<State> _statesList;

        public AsyncObservableCollectionView<State> StatesList
        {
            get { return _statesList; }
            set { SetProperty(ref this._statesList, value); }
        }

        protected override async Task OnInitialization()
        {
            Thread.Sleep(2000);
            var statesRepo = Resolve<IStateRepository>();
            StatesList =
                AsyncObservableCollectionView<State>.Create((item, searchText) => item.Name.ToLower()
                                                                .Contains(searchText),
                                                            await statesRepo.GetItems());
        }
    }
}