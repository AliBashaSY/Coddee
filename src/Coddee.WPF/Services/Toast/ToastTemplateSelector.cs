﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;

namespace Coddee.Services.Toast
{
    /// <summary>
    /// A template selector for the toast service
    /// </summary>
    public class ToastTemplateSelector : DataTemplateSelector
    {

        public DataTemplate InformationTemplate { get; set; }
        public DataTemplate SuccessTemplate { get; set; }
        public DataTemplate WarningTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var toast = item as Toast;
            if (toast != null)
            {
                switch (toast.Type)
                {
                    case ToastType.Information:
                        return InformationTemplate;
                    case ToastType.Success:
                        return SuccessTemplate;
                    case ToastType.Warning:
                        return WarningTemplate;
                    case ToastType.Error:
                        return ErrorTemplate;
                }
            }
            return null;
        }
    }
}