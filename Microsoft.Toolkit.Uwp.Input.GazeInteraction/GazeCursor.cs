// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
// See LICENSE in the project root for license information.

using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal sealed class GazeCursor
    {
        private const int DEFAULT_CURSOR_RADIUS = 5;
        private const bool DEFAULT_CURSOR_VISIBILITY = true;

        private Popup _gazePopup;
        private Point _cursorPosition;
        private int _cursorRadius = DEFAULT_CURSOR_RADIUS;
        private bool _isCursorVisible = DEFAULT_CURSOR_VISIBILITY;
        private bool _isGazeEntered;

        internal GazeCursor()
        {
            _gazePopup = new Popup();
            _gazePopup.IsHitTestVisible = false;

            var gazeCursor = new Ellipse();
            gazeCursor.Fill = new SolidColorBrush(Colors.IndianRed);
            gazeCursor.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            gazeCursor.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            gazeCursor.Width = 2 * CursorRadius;
            gazeCursor.Height = 2 * CursorRadius;
            gazeCursor.Margin = new Thickness(-CursorRadius, -CursorRadius, 0, 0);
            gazeCursor.IsHitTestVisible = false;

            _gazePopup.Child = gazeCursor;
        }

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

        public int CursorRadius
        {
            get
            {
                return _cursorRadius;
            }

            set
            {
                _cursorRadius = value;
                var gazeCursor = CursorElement;
                if (gazeCursor != null)
                {
                    gazeCursor.Width = 2 * _cursorRadius;
                    gazeCursor.Height = 2 * _cursorRadius;
                    gazeCursor.Margin = new Thickness(-_cursorRadius, -_cursorRadius, 0, 0);
                }
            }
        }

        public bool IsCursorVisible
        {
            get
            {
                return _isCursorVisible;
            }

            set
            {
                _isCursorVisible = value;
                SetVisibility();
            }
        }

        public bool IsGazeEntered
        {
            get
            {
                return _isGazeEntered;
            }

            set
            {
                _isGazeEntered = value;
                SetVisibility();
            }
        }

        public void LoadSettings(ValueSet settings)
        {
            if (settings.ContainsKey("GazeCursor.CursorRadius"))
            {
                CursorRadius = (int)settings["GazeCursor.CursorRadius"];
            }

            if (settings.ContainsKey("GazeCursor.CursorVisibility"))
            {
                IsCursorVisible = (bool)settings["GazeCursor.CursorVisibility"];
            }
        }

        private void SetVisibility()
        {
            var isOpen = _isCursorVisible && _isGazeEntered;
            if (_gazePopup.IsOpen != isOpen)
            {
                _gazePopup.IsOpen = isOpen;
            }
            else if (isOpen)
            {
                var topmost = VisualTreeHelper.GetOpenPopups(Window.Current).First();
                if (_gazePopup != topmost)
                {
                    _gazePopup.IsOpen = false;
                    _gazePopup.IsOpen = true;
                }
            }
        }
    }
}