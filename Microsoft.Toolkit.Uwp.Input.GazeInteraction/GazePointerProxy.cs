// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
// See LICENSE in the project root for license information.

using System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// Helper class that helps track which UIElements in the visual tree are enabled.
    ///
    /// The GazePointer is enabled when one or more UIElements in the visual tree have
    /// their GazeInput.InteractionProperty value set to Enabled. Notice that there are
    /// two conditions for enablement: that attached property is Enabled; that the UIElement
    /// is in the visual tree.
    /// </summary>
    internal sealed class GazePointerProxy
    {
        /// <summary>
        /// A private attached property for associating an instance of this class with the UIElement
        /// to which it refers.
        /// </summary>
        public static DependencyProperty GazePointerProxyProperty { get; } = null;

        /// <summary>
        /// Method called when the GazeInput.Interaction attached property is set to a new value.
        /// </summary>
        /// <param name="element">The element being set. May be null to indicate whole user interface.</param>
        /// <param name="value">The interaction enablement value being set.</param>
        internal static void SetInteraction(FrameworkElement element, Interaction value) { throw new NotImplementedException(); }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">The element proxy is attaching to.</param>
        private GazePointerProxy(FrameworkElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the enablement of this proxy.
        /// </summary>
        /// <param name="sender">The object setting the enable value.</param>
        /// <param name="value">The new enable value.</param>
        private void SetIsEnabled(object sender, bool value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The handler to be called when the corresponding element joins the visual tree.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The handler to be called when the corresponding element leaves the visual tree.
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Non-zero ID associated with this instance.
        /// </summary>
        private int _uniqueId;

        /// <summary>
        /// Indicator that the corresponding element is part of the visual tree.
        /// </summary>
        private bool _isLoaded;

        /// <summary>
        /// Boolean representing whether gaze is enabled for the corresponding element and its subtree.
        /// </summary>
        private bool _isEnabled;

        /*
    # include "pch.h"
    # include "GazePointerProxy.h"
    # include "GazePointer.h"

    BEGIN_NAMESPACE_GAZE_INPUT

    /// <summary>
    /// The IsLoaded heuristic for testing whether a FrameworkElement is in the visual tree.
    /// </summary>
    static bool IsLoadedHeuristic(FrameworkElement^ element)
    {
        bool isLoaded;

        // element.Loaded has already happened if it is in the visual tree...
        auto parent = VisualTreeHelper::GetParent(element);
        if (parent != nullptr)
        {
            isLoaded = true;
        }
        // ...or...
        else
        {
            // ...if the element is a dynamically created Popup that has been opened.
            auto popup = dynamic_cast < Popup ^> (element);
            isLoaded = popup != nullptr && popup->IsOpen;
        }

        return isLoaded;
    }

    DependencyProperty^ GazePointerProxy::GazePointerProxyProperty::get()
    {
        // The attached property registration.
        static auto value = DependencyProperty::RegisterAttached("_GazePointerProxy", GazePointerProxy::typeid, GazePointerProxy::typeid,
            ref new PropertyMetadata(nullptr));
        return value;
    }

    GazePointerProxy::GazePointerProxy(FrameworkElement^ element)
    {
        static int lastId = 0;
    lastId++;
        _uniqueId = lastId;

        _isLoaded = IsLoadedHeuristic(element);

    // Start watching for the element to enter and leave the visual tree.
    element->Loaded += ref new RoutedEventHandler(this, &GazePointerProxy::OnLoaded);
    element->Unloaded += ref new RoutedEventHandler(this, &GazePointerProxy::OnUnloaded);
    }

    void GazePointerProxy::SetInteraction(FrameworkElement^ element, Interaction value)
    {
        // Get or create a GazePointerProxy for element.
        auto proxy = safe_cast < GazePointerProxy ^> (element->GetValue(GazePointerProxyProperty));
        if (proxy == nullptr)
        {
            proxy = ref new GazePointerProxy(element);
            element->SetValue(GazePointerProxyProperty, proxy);
        }

        // Set the proxy's _isEnabled value.
        proxy->SetIsEnabled(element, value == Interaction::Enabled);
    }

    void GazePointerProxy::SetIsEnabled(Object^ sender, bool value)
    {
        // If we have a new value...
        if (_isEnabled != value)
        {
            // ...record the new value.
            _isEnabled = value;

            // If we are in the visual tree...
            if (_isLoaded)
            {
                // ...if we're being enabled...
                if (value)
                {
                    // ...count the element in...
                    GazePointer::Instance->AddRoot(_uniqueId);
                }
                else
                {
                    // ...otherwise count the element out.
                    GazePointer::Instance->RemoveRoot(_uniqueId);
                }
            }
        }
    }

    void GazePointerProxy::OnLoaded(Object^ sender, RoutedEventArgs^ args)
    {
        assert(IsLoadedHeuristic(safe_cast < FrameworkElement ^> (sender)));

        if (!_isLoaded)
        {
            // Record that we are now loaded.
            _isLoaded = true;

            // If we were previously enabled...
            if (_isEnabled)
            {
                // ...we can now be counted as actively enabled.
                GazePointer::Instance->AddRoot(_uniqueId);
            }
        }
        else
        {
            Debug::WriteLine(L"Unexpected Load");
        }
    }

    void GazePointerProxy::OnUnloaded(Object^ sender, RoutedEventArgs^ args)
    {
        assert(!IsLoadedHeuristic(safe_cast < FrameworkElement ^> (sender)));

        if (_isLoaded)
        {
            // Record that we have left the visual tree.
            _isLoaded = false;

            // If we are set as enabled...
            if (_isEnabled)
            {
                // ...we no longer count as being actively enabled (because we have fallen out the visual tree).
                GazePointer::Instance->RemoveRoot(_uniqueId);
            }
        }
        else
        {
            Debug::WriteLine(L"Unexpected unload");
        }
    }

    END_NAMESPACE_GAZE_INPUT
                         */
    }
}
