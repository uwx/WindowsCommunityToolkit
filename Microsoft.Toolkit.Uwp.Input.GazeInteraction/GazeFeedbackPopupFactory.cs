// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
// See LICENSE in the project root for license information.

using System.Collections.Generic;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal class GazeFeedbackPopupFactory
    {
        private List<Popup> _cache = new List<Popup>();

        public Popup Get()
        {
            Popup popup;
            Rectangle rectangle;

            if (_cache.Count != 0)
            {
                popup = _cache[0];
                _cache.RemoveAt(0);

                rectangle = (Rectangle)popup.Child;
            }
            else
            {
                popup = new Popup();

                rectangle = new Rectangle();
                rectangle.IsHitTestVisible = false;

                popup.Child = rectangle;
            }

            rectangle.StrokeThickness = GazeInput.DwellStrokeThickness;

            return popup;
        }

        public void Return(Popup popup)
        {
            popup.IsOpen = false;
            _cache.Add(popup);
        }
    }
}
