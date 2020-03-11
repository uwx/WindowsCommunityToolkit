//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction { /*

GazeStats.GazeStats(int maxHistoryLen)
{
    _maxHistoryLen = maxHistoryLen;
    _history = new Vector<Point>();
}

void GazeStats.Reset()
{
    _sumX = 0;
    _sumY = 0;
    _sumSquaredX = 0;
    _sumSquaredY = 0;
    _history.Clear();
}

void GazeStats.Update(float x, float y)
{
    Point pt(x, y);
    _history.Append(pt);

    if (_history.Size > _maxHistoryLen)
    {
        var oldest = _history.GetAt(0);
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

*/ }
