//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Automation.Peers;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction { /*

static DependencyProperty GazeTargetItemProperty = DependencyProperty.RegisterAttached("_GazeTargetItem", GazeTargetItem.typeid, GazeTargetItem.typeid, new PropertyMetadata(null));

template<PatternInterface P, typename T>
ref class PatternGazeTargetItem abstract : GazeTargetItem
{
internal:

    PatternGazeTargetItem(UIElement element)
        : GazeTargetItem(element)
    {
    }

    static T GetPattern(AutomationPeer peer)
    {
        var pattern = peer.GetPattern(P);
        return safe_cast<T>(pattern);
    }

    static bool IsCandidate(AutomationPeer peer)
    {
        var provider = GetPattern(peer);
        return provider != null;
    }

    void Invoke() override sealed
    {
        var peer = FrameworkElementAutomationPeer.FromElement(TargetElement);
        var provider = GetPattern(peer);
        Invoke(provider);
    }

    virtual void Invoke(T provider) = 0;
};

ref class InvokePatternGazeTargetItem : PatternGazeTargetItem<PatternInterface.Invoke, IInvokeProvider>
{
internal:

    InvokePatternGazeTargetItem(UIElement element)
        : PatternGazeTargetItem(element)
    {
    }

    void Invoke(IInvokeProvider provider) override sealed
    {
        provider.Invoke();
    }
};

ref class TogglePatternGazeTargetItem : PatternGazeTargetItem<PatternInterface.Toggle, IToggleProvider>
{
internal:

    TogglePatternGazeTargetItem(UIElement element)
        : PatternGazeTargetItem(element)
    {
    }

    void Invoke(IToggleProvider provider) override
    {
        provider.Toggle();
    }
};

ref class SelectionItemPatternGazeTargetItem : PatternGazeTargetItem<PatternInterface.SelectionItem, ISelectionItemProvider>
{
internal:

    SelectionItemPatternGazeTargetItem(UIElement element)
        : PatternGazeTargetItem(element)
    {
    }

    void Invoke(ISelectionItemProvider provider) override
    {
        provider.Select();
    }
};

ref class ExpandCollapsePatternGazeTargetItem : PatternGazeTargetItem<PatternInterface.ExpandCollapse, IExpandCollapseProvider>
{
internal:

    ExpandCollapsePatternGazeTargetItem(UIElement element)
        : PatternGazeTargetItem(element)
    {
    }

    void Invoke(IExpandCollapseProvider provider) override
    {
        switch (provider.ExpandCollapseState)
        {
        case ExpandCollapseState.Collapsed:
            provider.Expand();
            break;

        case ExpandCollapseState.Expanded:
            provider.Collapse();
            break;
        }
    }
};

ref class ComboBoxItemGazeTargetItem sealed : GazeTargetItem
{
internal:

    ComboBoxItemGazeTargetItem(UIElement element)
        : GazeTargetItem(element)
    {
    }

    void Invoke() override
    {
        var peer = FrameworkElementAutomationPeer.FromElement(TargetElement);
        var comboBoxItemAutomationPeer = dynamic_cast<ComboBoxItemAutomationPeer>(peer);
        var comboBoxItem = safe_cast<ComboBoxItem>(comboBoxItemAutomationPeer.Owner);

        AutomationPeer ancestor = comboBoxItemAutomationPeer;
        var comboBoxAutomationPeer = dynamic_cast<ComboBoxAutomationPeer>(ancestor);
        while (comboBoxAutomationPeer == null)
        {
            ancestor = safe_cast<AutomationPeer>(ancestor.Navigate(AutomationNavigationDirection.Parent));
            comboBoxAutomationPeer = dynamic_cast<ComboBoxAutomationPeer>(ancestor);
        }

        comboBoxItem.IsSelected = true;
        comboBoxAutomationPeer.Collapse();
    }
};

ref class PivotItemGazeTargetItem sealed : GazeTargetItem
{
internal:

    PivotItemGazeTargetItem(UIElement element)
        : GazeTargetItem(element)
    {
    }

    void Invoke() override
    {
        var headerItem = safe_cast<PivotHeaderItem>(TargetElement);
        var headerPanel = safe_cast<PivotHeaderPanel>(VisualTreeHelper.GetParent(headerItem));
        unsigned index;
        headerPanel.Children.IndexOf(headerItem, &index);

        DependencyObject walker = headerPanel;
        Pivot pivot;
        do
        {
            walker = VisualTreeHelper.GetParent(walker);
            pivot = dynamic_cast<Pivot>(walker);
        } while (pivot == null);

        pivot.SelectedIndex = index;
    }
};

GazeTargetItem GazeTargetItem.GetOrCreate(UIElement element)
{
    GazeTargetItem item;

    var value = element.ReadLocalValue(GazeTargetItemProperty);

    if (value != DependencyProperty.UnsetValue)
    {
        item = safe_cast<GazeTargetItem>(value);
    }
    else
    {
        var peer = FrameworkElementAutomationPeer.FromElement(element);

        if (peer == null)
        {
            if (dynamic_cast<PivotHeaderItem>(element) != null)
            {
                item = new PivotItemGazeTargetItem(element);
            }
            else
            {
                item = GazePointer.Instance._nonInvokeGazeTargetItem;
            }
        }
        else if (InvokePatternGazeTargetItem.IsCandidate(peer))
        {
            item = new InvokePatternGazeTargetItem(element);
        }
        else if (TogglePatternGazeTargetItem.IsCandidate(peer))
        {
            item = new TogglePatternGazeTargetItem(element);
        }
        else if (SelectionItemPatternGazeTargetItem.IsCandidate(peer))
        {
            item = new SelectionItemPatternGazeTargetItem(element);
        }
        else if (ExpandCollapsePatternGazeTargetItem.IsCandidate(peer))
        {
            item = new ExpandCollapsePatternGazeTargetItem(element);
        }
        else if (dynamic_cast<ComboBoxItemAutomationPeer>(peer) != null)
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

void GazeTargetItem.RaiseProgressEvent(DwellProgressState state)
{
    // TODO: We should eliminate non-invokable controls before we arrive here!
    if (dynamic_cast<Page>(TargetElement) != null)
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

            var control = safe_cast<FrameworkElement>(TargetElement);

            var transform = control.TransformToVisual(_feedbackPopup);
            var bounds = transform.TransformBounds(*new Rect(*new Point(0, 0),
                *new Size(safe_cast<float>(control.ActualWidth), safe_cast<float>(control.ActualHeight))));
            var rectangle = safe_cast<.Windows.UI.Xaml.Shapes.Rectangle>(_feedbackPopup.Child);

            if (state == DwellProgressState.Progressing)
            {
                var progress = ((double)(ElapsedTime - _prevStateTime).Duration) / (_nextStateTime - _prevStateTime).Duration;

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

*/ }
