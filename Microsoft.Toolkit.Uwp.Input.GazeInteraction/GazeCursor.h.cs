//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal sealed partial class GazeCursor
    {
        private const int DEFAULT_CURSOR_RADIUS = 5;
        private const bool DEFAULT_CURSOR_VISIBILITY = true;

        public Point Position
        {
            get
            {
                return _cursorPosition;
            }

            set
            {
                _cursorPosition = value;
                _gazePopup.HorizontalOffset = value.X;
                _gazePopup.VerticalOffset = value.Y;
                SetVisibility();
            }
        }

        public UIElement PopupChild
        {
            get
            {
                return _gazePopup.Child;
            }
            set
            {
                _gazePopup.Child = value;
            }
        }

        public FrameworkElement CursorElement
        {
            get
            {
                return _gazePopup.Child as FrameworkElement;
            }
        }

        private Popup _gazePopup;
        private Point _cursorPosition;
        private int _cursorRadius = DEFAULT_CURSOR_RADIUS;
        private bool _isCursorVisible = DEFAULT_CURSOR_VISIBILITY;
        private bool _isGazeEntered;
    }
}
