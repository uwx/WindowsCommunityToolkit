// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
// See LICENSE in the project root for license information.

using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    // Every filter must provide an Wpdate method which transforms sample data
    // and returns filtered output
    internal interface IGazeFilter
    {
        GazeFilterArgs Update(GazeFilterArgs args);

        void LoadSettings(ValueSet settings);
    }
}
