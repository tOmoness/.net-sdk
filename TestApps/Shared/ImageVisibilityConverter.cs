// -----------------------------------------------------------------------
// <copyright file="ImageVisibilityConverter.cs" company="MixRadio">
// Copyright © 2015, MixRadio. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MixRadio.Types;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MixRadio.TestApp
{
    /// <summary>
    /// Converts an MusicItem type into whether an image should be shown
    /// </summary>
    public class ImageVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value as Artist != null || value as Product != null || value as Mix != null || value as UserEvent != null)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
