//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    /// <summary>
    /// This parameter is passed to the GazeElement.Invoked event and allows 
    /// the application to prevent default invocation when the user dwells on a control
    /// </summary>
    public class DwellInvokedRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The application should set this value to true to prevent invoking the control when the user dwells on a control
        /// </summary>
        public bool Handled { get; set; }

        internal DwellInvokedRoutedEventArgs()
        {
        }
    }
}