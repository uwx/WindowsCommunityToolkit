//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// This parameter is passed to the GazeElement.DwellProgressFeedback event. The event is fired to inform the application of the user's progress towards completing dwelling on a control
    /// </summary>
    public class DwellProgressEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// An enum that reflects the current state of dwell progress
        /// </summary>
        public DwellProgressState State { get { return _state; } }

        /// <summary>
        /// A value between 0 and 1 that reflects the fraction of progress towards completing dwell
        /// </summary>
        public double Progress { get { return _progress; } }

        /// <summary>
        /// A parameter for the application to set to true if it handles the event. If this parameter is set to true, the library suppresses default animation for dwell feedback on the control
        /// </summary>
        public bool Handled { get; set; }

        internal DwellProgressEventArgs(DwellProgressState state, TimeSpan elapsedDuration, TimeSpan triggerDuration)
        {
            _state = state;
            _progress = ((double)elapsedDuration.Ticks) / triggerDuration.Ticks;
        }

        private DwellProgressState _state;
        private double _progress;
    }
}
