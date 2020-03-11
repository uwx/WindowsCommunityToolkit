//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    /// <summary>
    /// This struct encapsulates the location and timestamp associated with the user's gaze 
    /// and is used as an input and output parameter for the IGazeFilter.Update method
    /// </summary>
    internal struct GazeFilterArgs
    {
        /// <summary>
        /// The current point in the gaze stream
        /// </summary>
        internal Point Location
        {
            get { return _location; }
        }

        /// <summary>
        /// The timestamp associated with the current point
        /// </summary>
        internal TimeSpan Timestamp
        {
            get { return _timestamp; }
        }

        internal GazeFilterArgs(Point location, TimeSpan timestamp)
        {
            _location = location;
            _timestamp = timestamp;
        }

        private Point _location;
        private TimeSpan _timestamp;
    };

    // Every filter must provide an Wpdate method which transforms sample data 
    // and returns filtered output
    internal interface IGazeFilter
    {
        GazeFilterArgs Update(GazeFilterArgs args);
        void LoadSettings(ValueSet settings);
    };


    // Basic filter which performs no input filtering -- easy to
    // use as a default filter.
    internal sealed class NullFilter : IGazeFilter
    {
        public GazeFilterArgs Update(GazeFilterArgs args)
        {
            return args;
        }

        public void LoadSettings(ValueSet settings)
        {

        }
    }
}
