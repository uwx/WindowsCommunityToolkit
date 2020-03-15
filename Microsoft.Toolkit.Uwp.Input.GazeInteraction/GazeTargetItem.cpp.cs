//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using System;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal class NewPatternGazeTargetItem<T> : GazeTargetItem
    {
        readonly PatternInterface _patternInterface;
        readonly Action<T> _invoke;

        internal NewPatternGazeTargetItem(PatternInterface patternInterface, Action<T> invoke, UIElement element)
        : base(element)
        {
            _patternInterface = patternInterface;
            _invoke = invoke;
        }

        internal T GetPattern(AutomationPeer peer)
        {
            var pattern = peer.GetPattern(_patternInterface);
            return (T)pattern;
        }

        internal sealed override void Invoke()
        {
            var peer = FrameworkElementAutomationPeer.FromElement(TargetElement);
            var provider = GetPattern(peer);
            _invoke(provider);
        }
    }

    internal sealed class ComboBoxItemGazeTargetItem : GazeTargetItem
    {
        internal ComboBoxItemGazeTargetItem(UIElement element)
            : base(element)
        {
        }

        internal sealed override void Invoke()
        {
            var peer = FrameworkElementAutomationPeer.FromElement(TargetElement);
            var comboBoxItemAutomationPeer = peer as ComboBoxItemAutomationPeer;
            var comboBoxItem = (ComboBoxItem)comboBoxItemAutomationPeer.Owner;

            AutomationPeer ancestor = comboBoxItemAutomationPeer;
            var comboBoxAutomationPeer = ancestor as ComboBoxAutomationPeer;
            while (comboBoxAutomationPeer == null)
            {
                ancestor = (AutomationPeer)ancestor.Navigate(AutomationNavigationDirection.Parent);
                comboBoxAutomationPeer = ancestor as ComboBoxAutomationPeer;
            }

            comboBoxItem.IsSelected = true;
            comboBoxAutomationPeer.Collapse();
        }
    }

    internal sealed class PivotItemGazeTargetItem : GazeTargetItem
    {
        internal PivotItemGazeTargetItem(UIElement element)
            : base(element)
        {
        }

        internal sealed override void Invoke()
        {
            var headerItem = (PivotHeaderItem)TargetElement;
            var headerPanel = (PivotHeaderPanel)VisualTreeHelper.GetParent(headerItem);
            var index = headerPanel.Children.IndexOf(headerItem);

            DependencyObject walker = headerPanel;
            Pivot pivot;
            do
            {
                walker = VisualTreeHelper.GetParent(walker);
                pivot = walker as Pivot;
            } while (pivot == null);

            pivot.SelectedIndex = index;
        }
    }

    internal abstract partial class GazeTargetItem
    {
        static DependencyProperty GazeTargetItemProperty = DependencyProperty.RegisterAttached("_GazeTargetItem", typeof(GazeTargetItem), typeof(GazeTargetItem), new PropertyMetadata(null));

        private static bool CreatePatternItem<T>(PatternInterface pattern, Action<T> action, AutomationPeer peer, UIElement element, out GazeTargetItem item)
            where T : class
        {
            var automationPeer = peer.GetPattern(pattern) as T;

            if (automationPeer != null)
            {
                item = new NewPatternGazeTargetItem<T>(pattern, action, element);
            }
            else
            {
                item = null;
            }

            return item != null;
        }

        internal static GazeTargetItem GetOrCreate(UIElement element)
        {
            GazeTargetItem item;

            var value = element.ReadLocalValue(GazeTargetItemProperty);

            if (value != DependencyProperty.UnsetValue)
            {
                item = (GazeTargetItem)value;
            }
            else
            {
                var peer = FrameworkElementAutomationPeer.FromElement(element);

                if (peer == null)
                {
                    if (element as PivotHeaderItem != null)
                    {
                        item = new PivotItemGazeTargetItem(element);
                    }
                    else
                    {
                        item = GazePointer.Instance._nonInvokeGazeTargetItem;
                    }
                }
                else if (CreatePatternItem<IInvokeProvider>(PatternInterface.Invoke, p => p.Invoke(), peer, element, out item))
                {
                }
                else if (CreatePatternItem<IToggleProvider>(PatternInterface.Toggle, p => p.Toggle(), peer, element, out item))
                {
                }
                else if (CreatePatternItem<ISelectionItemProvider>(PatternInterface.SelectionItem, p => p.Select(), peer, element, out item))
                {
                }
                else if (CreatePatternItem<IExpandCollapseProvider>(PatternInterface.ExpandCollapse, p =>
                    {
                        {
                            switch (p.ExpandCollapseState)
                            {
                                case ExpandCollapseState.Collapsed:
                                    p.Expand();
                                    break;

                                case ExpandCollapseState.Expanded:
                                    p.Collapse();
                                    break;
                            }
                        }
                    }, peer, element, out item))
                {
                }
                else if (peer as ComboBoxItemAutomationPeer != null)
                {
                    item = new ComboBoxItemGazeTargetItem(element);
                }
                else
                {
                    item = GazePointer.Instance._nonInvokeGazeTargetItem;
                }

                element.SetValue(GazeTargetItemProperty, item);
            }

            return item;
        }

        private void RaiseProgressEvent(DwellProgressState state)
        {
            // TODO: We should eliminate non-invokable controls before we arrive here!
            if (TargetElement as Page != null)
            {
                return;
            }

            if (_notifiedProgressState != state || state == DwellProgressState.Progressing)
            {
                var handled = false;

                var gazeElement = GazeInput.GetGazeElement(TargetElement);
                if (gazeElement != null)
                {
                    handled = gazeElement.RaiseProgressFeedback(TargetElement, state, ElapsedTime - _prevStateTime, _nextStateTime - _prevStateTime);
                }

                if (!handled && state != DwellProgressState.Idle)
                {
                    if (_feedbackPopup == null)
                    {
                        _feedbackPopup = GazePointer.Instance._gazeFeedbackPopupFactory.Get();
                    }

                    var control = (FrameworkElement)TargetElement;

                    var transform = control.TransformToVisual(_feedbackPopup);
                    var bounds = transform.TransformBounds(new Rect(new Point(0, 0),
                        new Size(control.ActualWidth, control.ActualHeight)));
                    var rectangle = (Rectangle)_feedbackPopup.Child;

                    if (state == DwellProgressState.Progressing)
                    {
                        var progress = ((double)(ElapsedTime - _prevStateTime).Ticks) / (_nextStateTime - _prevStateTime).Ticks;

                        if (0 <= progress && progress < 1)
                        {
                            rectangle.Stroke = GazeInput.DwellFeedbackProgressBrush;
                            rectangle.Width = (1 - progress) * bounds.Width;
                            rectangle.Height = (1 - progress) * bounds.Height;

                            _feedbackPopup.HorizontalOffset = bounds.Left + progress * bounds.Width / 2;
                            _feedbackPopup.VerticalOffset = bounds.Top + progress * bounds.Height / 2;
                        }
                    }
                    else
                    {
                        rectangle.Stroke = state == DwellProgressState.Fixating ?
                            GazeInput.DwellFeedbackEnterBrush : GazeInput.DwellFeedbackCompleteBrush;
                        rectangle.Width = bounds.Width;
                        rectangle.Height = bounds.Height;

                        _feedbackPopup.HorizontalOffset = bounds.Left;
                        _feedbackPopup.VerticalOffset = bounds.Top;
                    }

                    _feedbackPopup.IsOpen = true;
                }
                else
                {
                    if (_feedbackPopup != null)
                    {
                        GazePointer.Instance._gazeFeedbackPopupFactory.Return(_feedbackPopup);
                        _feedbackPopup = null;
                    }
                }
            }

            _notifiedProgressState = state;
        }
    }
}
