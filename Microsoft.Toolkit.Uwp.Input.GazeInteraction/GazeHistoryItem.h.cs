//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    internal struct GazeHistoryItem
    {
        public GazeTargetItem HitTarget { get; set; }
        public TimeSpan Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
