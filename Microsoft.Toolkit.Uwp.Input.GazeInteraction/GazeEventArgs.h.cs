//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    public sealed class GazeEventArgs
    {
        public bool Handled { get; set; }
        public Point Location { get; set; }
        public TimeSpan Timestamp { get; set; }

        public GazeEventArgs()
        {
        }

        public void Set(Point location, TimeSpan timestamp)
        {
            Handled = false;
            Location = location;
            Timestamp = timestamp;
        }
    }
}
