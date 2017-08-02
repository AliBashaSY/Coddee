﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Services;
using Coddee.WPF.Commands;
using Coddee.WPF.Events;
using Coddee.WPF.Navigation;

namespace Coddee.WPF.Modules.Navigation
{
    public class NavigationService : ViewModelBase<NavigationServiceView>, INavigationService
    {
        public NavigationService()
        {
            
        }

        public NavigationService(IGlobalEventsService globalEvents)
        {
            _globalEvents = globalEvents;
          
        }


        private bool _showTitles;
        public bool ShowTitles
        {
            get { return _showTitles; }
            set
            {
                SetProperty(ref this._showTitles, value);
                NavigationItems?.ForEach(e => e.ShowTitle = value);
            }
        }

        private Region _navigationRegion;

        public event EventHandler<NavigationEventArgs> Navigated;
        private INavigationItem _currentNavligationItem;
        public void Initialize(Region navbarRegion,
                               Region navigationRegion,
                               IEnumerable<INavigationItem> navigationItems)
        {
            _navigationRegion = navigationRegion;
            NavigationItems = AsyncObservableCollection<INavigationItem>.Create();
            navigationItems.ForEach(AddNavigationItem);
            navbarRegion.View(this);
            _globalEvents.GetEvent<ApplicationStartedEvent>().Subscribe(e =>
            {
                if (navigationItems.Any())
                {
                    var first = navigationItems.First();
                    first.IsSelected = true;
                    if (first is NavigationItem nav)
                        nav.IsFirstItem = true;
                    first.Navigate();
                }
            });
        }

        public void AddNavigationItem(INavigationItem navigationItem)
        {
            NavigationItems.Add(navigationItem);
            if (!navigationItem.DestinationResolved)
                navigationItem.SetDestination((IPresentable)Resolve<IShellViewModel>()
                                                  .CreateViewModel(navigationItem.DestinationType));
            navigationItem.NavigationRequested += NavigationItem_NavigationRequested;
            navigationItem.Initialize();
        }

        private AsyncObservableCollection<INavigationItem> _navigationItems;
        private IGlobalEventsService _globalEvents;
        public AsyncObservableCollection<INavigationItem> NavigationItems
        {
            get { return _navigationItems; }
            set { SetProperty(ref this._navigationItems, value); }
        }
        public ICommand ExpandMenuCommand => new RelayCommand(ExpandMenu);

        private void ExpandMenu()
        {
            ShowTitles = !ShowTitles;
        }

        private void NavigationItem_NavigationRequested(object sender, IPresentable e)
        {
            var nav = sender as INavigationItem;
            foreach (var navigationItem in NavigationItems)
            {
                navigationItem.IsSelected = false;
            }
            Navigated?.Invoke(this, new NavigationEventArgs { OldLocation = _currentNavligationItem, NewLocation = nav });
            _navigationRegion.View(e);
            _currentNavligationItem = nav;
            ShowTitles = false;
            nav.IsSelected = true;
        }
    }
}