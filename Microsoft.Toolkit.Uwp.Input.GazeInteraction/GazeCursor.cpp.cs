//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System.Linq;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    internal sealed partial class GazeCursor
    {

        internal GazeCursor()
        {
            _gazePopup = new Popup();
            _gazePopup.IsHitTestVisible = false;

            var gazeCursor = new Ellipse();
            gazeCursor.Fill = new SolidColorBrush(Colors.IndianRed);
            gazeCursor.VerticalAlignment = VerticalAlignment.Top;
            gazeCursor.HorizontalAlignment = HorizontalAlignment.Left;
            gazeCursor.Width = 2 * CursorRadius;
            gazeCursor.Height = 2 * CursorRadius;
            gazeCursor.Margin = new Thickness(-CursorRadius, -CursorRadius, 0, 0);
            gazeCursor.IsHitTestVisible = false;

            _gazePopup.Child = gazeCursor;
        }

        public int CursorRadius
        {
            get { return _cursorRadius; }
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
            get { return _isCursorVisible; }
            set
            {
                _isCursorVisible = value;
                SetVisibility();
            }
        }

        public bool IsGazeEntered
        {
            get { return _isGazeEntered; }
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
                CursorRadius = (int)(settings["GazeCursor.CursorRadius"]);
            }
            if (settings.ContainsKey("GazeCursor.CursorVisibility"))
            {
                IsCursorVisible = (bool)(settings["GazeCursor.CursorVisibility"]);
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