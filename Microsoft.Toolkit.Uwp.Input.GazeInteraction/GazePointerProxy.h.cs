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
    internal sealed partial class GazePointerProxy
    {
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


