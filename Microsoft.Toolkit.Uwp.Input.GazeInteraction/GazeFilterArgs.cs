// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
// See LICENSE in the project root for license information.

using System;
using System.Drawing;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// This struct encapsulates the location and timestamp associated with the user's gaze
    /// and is used as an input and output parameter for the IGazeFilter::Update method
    /// </summary>
    internal class GazeFilterArgs
    {
        /// <summary>
        /// The current point in the gaze stream
        /// </summary>
        public Point Location => _location;

        /// <summary>
        /// Gets the timestamp associated with the current point
        /// </summary>
        public TimeSpan Timestamp => _timestamp;

        internal GazeFilterArgs(Point location, TimeSpan timestamp)
        {
            _location = location;
            _timestamp = timestamp;
        }

        private Point _location;
        private TimeSpan _timestamp;
    }
}
