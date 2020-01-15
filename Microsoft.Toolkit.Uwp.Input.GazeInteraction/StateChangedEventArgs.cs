// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
// See LICENSE in the project root for license information.

using System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// This parameter is passed to the StateChanged event.
    /// </summary>
    public sealed class StateChangedEventArgs
    {
        /// <summary>
        /// The state of user's gaze with respect to a control
        /// </summary>
        public PointerState PointerState => _pointerState;

        /// <summary>
        /// Gets elapsed time since the last state
        /// </summary>
        public TimeSpan ElapsedTime => _elapsedTime;

        internal StateChangedEventArgs(UIElement target, PointerState state, TimeSpan elapsedTime)
        {
            _hitTarget = target;
            _pointerState = state;
            _elapsedTime = elapsedTime;
        }

        private UIElement _hitTarget;
        private PointerState _pointerState;
        private TimeSpan _elapsedTime;
    }
}
