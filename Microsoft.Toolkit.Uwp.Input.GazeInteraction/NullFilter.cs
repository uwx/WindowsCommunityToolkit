// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
// See LICENSE in the project root for license information.

using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
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
