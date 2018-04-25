﻿namespace BreadcrumbLib.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Implements a Converter that can be used to convert a true/false
    /// input flag into either binding of two additional bindings.
    /// 
    /// This converter requires 3 bindings:
    /// 1> Boolean Flag
    /// 2> Binding to return if 1> is true
    /// 3> Binding to return if 1> is true
    /// 
    /// Sample Code:
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(object))]
    public class BoolToOneOfTwoBindingsConverter : DependencyObject, IValueConverter
    {
        public object TrueSource
        {
            get { return (object)GetValue(TrueSourceProperty); }
            set { SetValue(TrueSourceProperty, value); }
        }

        public static readonly DependencyProperty TrueSourceProperty =
            DependencyProperty.Register("TrueSource", typeof(object), typeof(BoolToOneOfTwoBindingsConverter), new PropertyMetadata(null));

        public object FalseSource
        {
            get { return (object)GetValue(FalseSourceProperty); }
            set { SetValue(FalseSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FalseSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FalseSourceProperty =
            DependencyProperty.Register("FalseSource", typeof(object), typeof(BoolToOneOfTwoBindingsConverter), new PropertyMetadata(null));



        /// <summary>
        /// The source path (for CLR bindings) of the True value binding.
        /// </summary>
        //public object TrueSource { get; set; }

        /// <summary>
        /// The source path (for CLR bindings) of the False value binding.
        /// </summary>
        //public object FalseSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool) == false)
                return Binding.DoNothing;

            var inputFlag = (bool)value;

            if (inputFlag == true)       // Return corresponding binding to input flag
            {
                Debug.WriteLine("--> True Source is returned.");
                return TrueSource;
            }
            else
            {
                Debug.WriteLine("--> False Source is returned.");
                return FalseSource;
            }
        }

        /// <summary>
        /// The convert expects 3 bindings:
        /// 1> True/False value to determine whether updates should be shown to UI or not.
        /// TrueSource  (property) The binding that should be shown if 1> was True.
        /// FalseSource (property) The binding that should be shown if 1> was False.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
