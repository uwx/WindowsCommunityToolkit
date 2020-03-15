//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System.Collections.Generic;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    public sealed partial class GazeStats
    {

        public GazeStats(int maxHistoryLen)
        {
            _maxHistoryLen = maxHistoryLen;
            _history = new List<Point>();
        }

        public void Reset()
        {
            _sumX = 0;
            _sumY = 0;
            _sumSquaredX = 0;
            _sumSquaredY = 0;
            _history.Clear();
        }

        public void Update(float x, float y)
        {
            var pt = new Point(x, y);
            _history.Add(pt);

            if (_history.Count > _maxHistoryLen)
            {
                var oldest = _history[0];
                _history.RemoveAt(0);

                _sumX -= oldest.X;
                _sumY -= oldest.Y;
                _sumSquaredX -= oldest.X * oldest.X;
                _sumSquaredY -= oldest.Y * oldest.Y;
            }
            _sumX += x;
            _sumY += y;
            _sumSquaredX += x * x;
            _sumSquaredY += y * y;
        }
    }
}
