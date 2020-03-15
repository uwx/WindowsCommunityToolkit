//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

//
// http://www.lifl.fr/~casiez/1euro/
// http://www.lifl.fr/~casiez/publications/CHI2012-casiez.pdf
//

using System;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal sealed partial class OneEuroFilter : IGazeFilter
    {
        public OneEuroFilter()
        {

            _lastTimestamp = TimeSpan.Zero;
            Beta = ONEEUROFILTER_DEFAULT_BETA;
            Cutoff = ONEEUROFILTER_DEFAULT_CUTOFF;
            VelocityCutoff = ONEEUROFILTER_DEFAULT_VELOCITY_CUTOFF;
        }

        public OneEuroFilter(float cutoff, float beta)
        {
            _lastTimestamp = TimeSpan.Zero;
            Beta = beta;
            Cutoff = cutoff;
            VelocityCutoff = ONEEUROFILTER_DEFAULT_VELOCITY_CUTOFF;
        }

        public GazeFilterArgs Update(GazeFilterArgs args)
        {
            if (_lastTimestamp == TimeSpan.Zero)
            {
                _lastTimestamp = args.Timestamp;
                _pointFilter = new LowpassFilter(args.Location);
                _deltaFilter = new LowpassFilter(new Point());
                return new GazeFilterArgs(args.Location, args.Timestamp);
            }

            Point gazePoint = args.Location;

            // Reducing _beta increases lag. Increasing beta decreases lag and improves response time
            // But a really high value of beta also contributes to jitter
            float beta = Beta;

            // This simply represents the cutoff frequency. A lower value reduces jiiter
            // and higher value increases jitter
            float cf = Cutoff;
            Point cutoff = new Point(cf, cf);

            // determine sampling frequency based on last time stamp
            float samplingFrequency = 10000000.0f / Math.Max(1, (args.Timestamp - _lastTimestamp).Ticks);
            _lastTimestamp = args.Timestamp;

            // calculate change in distance...
            Point deltaDistance;
            deltaDistance.X = gazePoint.X - _pointFilter.Previous.X;
            deltaDistance.Y = gazePoint.Y - _pointFilter.Previous.Y;

            // ...and velocity
            Point velocity = new Point(deltaDistance.X * samplingFrequency, deltaDistance.Y * samplingFrequency);

            // find the alpha to use for the velocity filter
            var velocityAlpha = Alpha(samplingFrequency, VelocityCutoff);
            Point velocityAlphaPoint = new Point(velocityAlpha, velocityAlpha);

            // find the filtered velocity
            Point filteredVelocity = _deltaFilter.Update(velocity, velocityAlphaPoint);

            // ignore sign since it will be taken care of by deltaDistance
            filteredVelocity.X = Math.Abs(filteredVelocity.X);
            filteredVelocity.Y = Math.Abs(filteredVelocity.Y);

            // compute new cutoff to use based on velocity
            cutoff.X += beta * filteredVelocity.X;
            cutoff.Y += beta * filteredVelocity.Y;

            // find the new alpha to use to filter the points
            Point distanceAlpha = new Point(Alpha(samplingFrequency, cutoff.X), Alpha(samplingFrequency, cutoff.Y));

            // find the filtered point
            Point filteredPoint = _pointFilter.Update(gazePoint, distanceAlpha);

            // compute the new args
            var fa = new GazeFilterArgs(filteredPoint, args.Timestamp);
            return fa;
        }

        private double Alpha(float rate, double cutoff)
        {
            var te = 1.0f / rate;
            var tau = 1.0f / (2 * Math.PI * cutoff);
            var alpha = te / (te + tau);
            return alpha;
        }

        public void LoadSettings(ValueSet settings)
        {
            if (settings.ContainsKey("OneEuroFilter.Beta"))
            {
                Beta = (float)(settings["OneEuroFilter.Beta"]);
            }
            if (settings.ContainsKey("OneEuroFilter.Cutoff"))
            {
                Cutoff = (float)(settings["OneEuroFilter.Cutoff"]);
            }
            if (settings.ContainsKey("OneEuroFilter.VelocityCutoff"))
            {
                VelocityCutoff = (float)(settings["OneEuroFilter.VelocityCutoff"]);
            }
        }
    }
}
