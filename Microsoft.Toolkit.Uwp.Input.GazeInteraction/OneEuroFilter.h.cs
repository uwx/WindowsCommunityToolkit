//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal sealed class LowpassFilter
    {
        public LowpassFilter()
        {
            Previous = new Point(0, 0);
        }

        public LowpassFilter(Point initial)
        {
            Previous = initial;
        }

        public Point Previous { get; set; }

        public Point Update(Point point, Point alpha)
        {
            Point pt;
            pt.X = (alpha.X * point.X) + ((1 - alpha.X) * Previous.X);
            pt.Y = (alpha.Y * point.Y) + ((1 - alpha.Y) * Previous.Y);
            Previous = pt;
            return Previous;
        }
    };

    internal sealed partial class OneEuroFilter : IGazeFilter
    {
        internal const float ONEEUROFILTER_DEFAULT_BETA = 5.0f;
        internal const float ONEEUROFILTER_DEFAULT_CUTOFF = 0.1f;
        internal const float ONEEUROFILTER_DEFAULT_VELOCITY_CUTOFF = 1.0f;

        public float Beta;
        public float Cutoff;
        public float VelocityCutoff;

        private TimeSpan _lastTimestamp;
        private LowpassFilter _pointFilter;
        private LowpassFilter _deltaFilter;
    }
}
