﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Coddee.WPF
{
    /// <summary>
    /// Text input types.
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// Text is alphabetic characters only.
        /// </summary>
        Alphabetic,

        /// <summary>
        /// Text is numbers only.
        /// </summary>
        Numeric
    }

    /// <summary>
    /// A class to specify the input type of a TextBoxBase element
    /// </summary>
    public class InputMask
    {
        /// <summary>
        /// The required text type.
        /// </summary>
        public static readonly DependencyProperty InputTypeProperty = DependencyProperty.RegisterAttached(
                                                                "InputType",
                                                                typeof(InputType?),
                                                                typeof(InputMask),
                                                                new PropertyMetadata(default(InputType?),MaskTypeSet));

        private static readonly Regex _alphabeticReg = new Regex(@"^[A-Z a-z ء-ي]+$",RegexOptions.Compiled);
        private static readonly Regex _numbericReg = new Regex(@"^[\d]+$", RegexOptions.Compiled);

        private static void MaskTypeSet(DependencyObject d, DependencyPropertyChangedEventArgs arg)
        {
            if (d is TextBoxBase textBox && arg.NewValue!=null)
            {
                Func<string, bool> func = (InputType) arg.NewValue == InputType.Alphabetic
                    ? (Func<string, bool>)(e=>!_alphabeticReg.IsMatch(e))
                    : e=>!_numbericReg.IsMatch(e);

                textBox.PreviewTextInput+=(sender,e)=>e.Handled = func(e.Text);
            }
        }

        /// <summary>
        /// Set <see cref="InputTypeProperty"/>
        /// </summary>
        public static void SetInputType(DependencyObject element, InputType? value)
        {
            element.SetValue(InputTypeProperty, value);
        }

        /// <summary>
        /// Get <see cref="InputTypeProperty"/>
        /// </summary>
        public static InputType? GetInputType(DependencyObject element)
        {
            return (InputType?) element.GetValue(InputTypeProperty);
        }
    }
}
