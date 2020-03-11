//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    /// <summary>
    /// Helper class that helps track which UIElements in the visual tree are enabled.
    ///
    /// The GazePointer is enabled when one or more UIElements in the visual tree have
    /// their GazeInput.InteractionProperty value set to Enabled. Notice that there are
    /// two conditions for enablement: that attached property is Enabled; that the UIElement
    /// is in the visual tree.
    /// </summary>
    internal sealed class GazePointerProxy
    {
        /// <summary>
        /// A private attached property for associating an instance of this class with the UIElement
        /// to which it refers.
        /// </summary>
        public static DependencyProperty GazePointerProxyProperty
        {
            get { throw new ToDoException(); }
        }

    /// <summary>
    /// Method called when the GazeInput.Interaction attached property is set to a new value.
    /// </summary>
    /// <param name="element">The element being set. May be null to indicate whole user interface.</param>
    /// <param name="value">The interaction enablement value being set.</param>
        internal static void SetInteraction(FrameworkElement element, Interaction value) { throw new ToDoException(); }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">The element proxy is attaching to.</param>
        private GazePointerProxy(FrameworkElement element) { throw new ToDoException(); }

        /// <summary>
        /// Set the enablement of this proxy.
        /// </summary>
        /// <param name="sender">The object setting the enable value.</param>
        /// <param name="value">The new enable value.</param>
        private void SetIsEnabled(Object sender, bool value) { throw new ToDoException(); }

        /// <summary>
        /// The handler to be called when the corresponding element joins the visual tree.
        /// </summary>
        private void OnLoaded(Object sender, RoutedEventArgs args) { throw new ToDoException(); }

        /// <summary>
        /// The handler to be called when the corresponding element leaves the visual tree.
        /// </summary>
        private void OnUnloaded(Object sender, RoutedEventArgs args) { throw new ToDoException(); }

        /// <summary>
        /// Non-zero ID associated with this instance.
        /// </summary>
        private int _uniqueId;

        /// <summary>
        /// Indicator that the corresponding element is part of the visual tree.
        /// </summary>
        private bool _isLoaded;

        /// <summary>
        /// Boolean representing whether gaze is enabled for the corresponding element and its subtree.
        /// </summary>
        private bool _isEnabled;
    }
}


