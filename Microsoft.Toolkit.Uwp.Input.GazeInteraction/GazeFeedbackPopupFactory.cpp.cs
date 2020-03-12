//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    internal partial class GazeFeedbackPopupFactory
    {
        public Popup Get()
        {
            Popup popup;
            Rectangle rectangle;

            if (s_cache.Count != 0)
            {
                popup = s_cache[0];
                s_cache.RemoveAt(0);

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
            s_cache.Add(popup);
        }
    }
}
