//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction { /*

Popup^ GazeFeedbackPopupFactory.Get()
{
    Popup^ popup;
    .Windows.UI.Xaml.Shapes.Rectangle^ rectangle;

    if (s_cache.Size != 0)
    {
        popup = s_cache.GetAt(0);
        s_cache.RemoveAt(0);

        rectangle = safe_cast<.Windows.UI.Xaml.Shapes.Rectangle^>(popup.Child);
    }
    else
    {
        popup = ref new Popup();

        rectangle = ref new .Windows.UI.Xaml.Shapes.Rectangle();
        rectangle.IsHitTestVisible = false;

        popup.Child = rectangle;
    }

    rectangle.StrokeThickness = GazeInput.DwellStrokeThickness;

    return popup;
}

void GazeFeedbackPopupFactory.Return(Popup^ popup)
{
    popup.IsOpen = false;
    s_cache.Append(popup);
}

*/ }
