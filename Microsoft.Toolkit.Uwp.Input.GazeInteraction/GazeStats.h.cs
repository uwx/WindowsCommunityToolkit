//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    public sealed class GazeStats
    {
        public GazeStats(int maxHistoryLen) { throw new ToDoException(); }
        public void Reset() { throw new ToDoException(); }
        public void Update(float x, float y) { throw new ToDoException(); }

        public Point Mean
        {
            get
            {
                var count = _history.Count;
                return new Point((float)_sumX / count, (float)_sumY / count);
            }
        }

        //
        // StdDev = sqrt(Variance) = sqrt(E[X2] – (E[X])2)
        //
        public Point StandardDeviation
        {
            get
            {
                var count = _history.Count;
                if (count < _maxHistoryLen)
                {
                    return new Point(0.0f, 0.0f);
                }
                double meanX = _sumX / count;
                double meanY = _sumY / count;
                float stddevX = (float)Math.Sqrt((_sumSquaredX / count) - (meanX * meanX));
                float stddevY = (float)Math.Sqrt((_sumSquaredY / count) - (meanY * meanY));
                return new Point(stddevX, stddevY);
            }
        }

        private uint _maxHistoryLen;
        private double _sumX;
        private double _sumY;
        private double _sumSquaredX;
        private double _sumSquaredY;
        List<Point> _history;
    }
}
