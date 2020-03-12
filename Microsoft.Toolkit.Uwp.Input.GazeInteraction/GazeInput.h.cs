//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    /// <summary>
    /// Static class primarily providing access to attached properties controlling gaze behavior.
    /// </summary>
    [WebHostHidden]
    public static partial class GazeInput
    {
        /// <summary>
        /// Identifyes the Interaction dependency property
        /// </summary>
        public static DependencyProperty InteractionProperty
        {
            get
            {
                return s_interactionProperty;
            }
        }

        /// <summary>
        /// Identifyes the IsCursorVisible dependency property
        /// </summary>
        public static DependencyProperty IsCursorVisibleProperty
        {
            get
            {
                return s_isCursorVisibleProperty;
            }
        }

        /// <summary>
        /// Identifyes the CursorRadius dependency property
        /// </summary>
        public static DependencyProperty CursorRadiusProperty
        {
            get { return s_cursorRadiusProperty; }
        }

        /// <summary>
        /// Identifyes the GazeElement dependency property
        /// </summary>
        public static DependencyProperty GazeElementProperty
        {
            get { return s_gazeElementProperty; }
        }

        /// <summary>
        /// Identifyes the FixationDuration dependency property
        /// </summary>
        public static DependencyProperty FixationDurationProperty
        {
            get { return s_fixationDurationProperty; }
        }

        /// <summary>
        /// Identifies the DwellDuration dependency property
        /// </summary>
        public static DependencyProperty DwellDurationProperty
        {
            get { return s_dwellDurationProperty; }
        }

        /// <summary>
        /// Identifies the RepeatDelayDuration dependency property
        /// </summary>
        public static DependencyProperty RepeatDelayDurationProperty
        {
            get { return s_repeatDelayDurationProperty; }
        }

        /// <summary>
        /// Identifies the DwellRepeatDuration dependency property
        /// </summary>
        public static DependencyProperty DwellRepeatDurationProperty
        {
            get { return s_dwellRepeatDurationProperty; }
        }

        /// <summary>
        /// Identifies the ThresholdDuration dependency property
        /// </summary>
        public static DependencyProperty ThresholdDurationProperty
        {
            get { return s_thresholdDurationProperty; }
        }

        /// <summary>
        /// Identifies the MaxDwellRepeatCount dependency property
        /// </summary>
        public static DependencyProperty MaxDwellRepeatCountProperty
        {
            get { return s_maxRepeatCountProperty; }
        }

        /// <summary>
        /// Identifyes the IsSwitchEnabled dependency property
        /// </summary>
        public static DependencyProperty IsSwitchEnabledProperty
        {
            get { return s_isSwitchEnabledProperty; }
        }

        /// <summary>
        /// Gets the status of gaze interaction over that particular XAML element.
        /// </summary>
        public static Interaction GetInteraction(UIElement element) { return (Interaction)element.GetValue(InteractionProperty); }

        /// <summary>
        /// Gets Boolean indicating whether cursor is shown while user is looking at the school.
        /// </summary>
        public static bool GetIsCursorVisible(UIElement element) { return (bool)element.GetValue(IsCursorVisibleProperty); }

        /// <summary>
        /// Gets the size of the gaze cursor radius.
        /// </summary>
        public static int GetCursorRadius(UIElement element) { return (int)element.GetValue(CursorRadiusProperty); }

        /// <summary>
        /// Gets the GazeElement associated with an UIElement.
        /// </summary>
        public static GazeElement GetGazeElement(UIElement element) { return (GazeElement)element.GetValue(GazeElementProperty); }

        /// <summary>
        /// Gets the duration for the control to transition from the Enter state to the Fixation state. At this point, a StateChanged event is fired with PointerState set to Fixation. This event should be used to control the earliest visual feedback the application needs to provide to the user about the gaze location. The default is 350ms.
        /// </summary>
        public static TimeSpan GetFixationDuration(UIElement element) { return (TimeSpan)element.GetValue(FixationDurationProperty); }

        /// <summary>
        /// Gets the duration for the control to transition from the Fixation state to the Dwell state. At this point, a StateChanged event is fired with PointerState set to Dwell. The Enter and Fixation states are typicaly achieved too rapidly for the user to have much control over. In contrast Dwell is conscious event. This is the point at which the control is invoked, e.g. a button click. The application can modify this property to control when a gaze enabled UI element gets invoked after a user starts looking at it.
        /// </summary>
        public static TimeSpan GetDwellDuration(UIElement element) { return (TimeSpan)element.GetValue(DwellDurationProperty); }

        /// <summary>
        /// Gets the additional duration for the first repeat to occur.This prevents inadvertent repeated invocation.
        /// </summary>
        public static TimeSpan GetRepeatDelayDuration(UIElement element) { return (TimeSpan)element.GetValue(RepeatDelayDurationProperty); }

        /// <summary>
        /// Gets the duration of repeated dwell invocations, should the user continue to dwell on the control. The first repeat will occur after an additional delay specified by RepeatDelayDuration. Subsequent repeats happen after every period of DwellRepeatDuration. A control is invoked repeatedly only if MaxDwellRepeatCount is set to greater than zero.
        /// </summary>
        public static TimeSpan GetDwellRepeatDuration(UIElement element) { return (TimeSpan)element.GetValue(DwellRepeatDurationProperty); }

        /// <summary>
        /// Gets the duration that controls when the PointerState moves to either the Enter state or the Exit state. When this duration has elapsed after the user's gaze first enters a control, the PointerState is set to Enter. And when this duration has elapsed after the user's gaze has left the control, the PointerState is set to Exit. In both cases, a StateChanged event is fired. The default is 50ms.
        /// </summary>
        public static TimeSpan GetThresholdDuration(UIElement element) { return (TimeSpan)element.GetValue(ThresholdDurationProperty); }

        /// <summary>
        /// Gets the maximum times the control will invoked repeatedly without the user's gaze having to leave and re-enter the control. The default value is zero which disables repeated invocation of a control. Developers can set a higher value to enable repeated invocation.
        /// </summary>
        public static int GetMaxDwellRepeatCount(UIElement element) { return (int)element.GetValue(MaxDwellRepeatCountProperty); }

        /// <summary>
        /// Gets the Boolean indicating whether gaze plus switch is enabled.
        /// </summary>
        public static bool GetIsSwitchEnabled(UIElement element) { return (bool)element.GetValue(IsSwitchEnabledProperty); }

        /// <summary>
        /// Sets the status of gaze interaction over that particular XAML element.
        /// </summary>
        public static void SetInteraction(UIElement element, GazeInteraction.Interaction value) { element.SetValue(InteractionProperty, value); }

        /// <summary>
        /// Sets Boolean indicating whether cursor is shown while user is looking at the school.
        /// </summary>
        public static void SetIsCursorVisible(UIElement element, bool value) { element.SetValue(IsCursorVisibleProperty, value); }

        /// <summary>
        /// Sets the size of the gaze cursor radius.
        /// </summary>
        public static void SetCursorRadius(UIElement element, int value) { element.SetValue(CursorRadiusProperty, value); }

        /// <summary>
        /// Sets the GazeElement associated with an UIElement.
        /// </summary>
        public static void SetGazeElement(UIElement element, GazeElement value) { element.SetValue(GazeElementProperty, value); }

        /// <summary>
        /// Sets the duration for the control to transition from the Enter state to the Fixation state. At this point, a StateChanged event is fired with PointerState set to Fixation. This event should be used to control the earliest visual feedback the application needs to provide to the user about the gaze location. The default is 350ms.
        /// </summary>
        public static void SetFixationDuration(UIElement element, TimeSpan value) { element.SetValue(FixationDurationProperty, value); }

        /// <summary>
        /// Sets the duration for the control to transition from the Fixation state to the Dwell state. At this point, a StateChanged event is fired with PointerState set to Dwell. The Enter and Fixation states are typicaly achieved too rapidly for the user to have much control over. In contrast Dwell is conscious event. This is the point at which the control is invoked, e.g. a button click. The application can modify this property to control when a gaze enabled UI element gets invoked after a user starts looking at it.
        /// </summary>
        public static void SetDwellDuration(UIElement element, TimeSpan value) { element.SetValue(DwellDurationProperty, value); }

        /// <summary>
        /// Sets the additional duration for the first repeat to occur.This prevents inadvertent repeated invocation.
        /// </summary>
        public static void SetRepeatDelayDuration(UIElement element, TimeSpan value) { element.SetValue(RepeatDelayDurationProperty, value); }

        /// <summary>
        /// Sets the duration of repeated dwell invocations, should the user continue to dwell on the control. The first repeat will occur after an additional delay specified by RepeatDelayDuration. Subsequent repeats happen after every period of DwellRepeatDuration. A control is invoked repeatedly only if MaxDwellRepeatCount is set to greater than zero.
        /// </summary>
        public static void SetDwellRepeatDuration(UIElement element, TimeSpan value) { element.SetValue(DwellRepeatDurationProperty, value); }

        /// <summary>
        /// Sets the duration that controls when the PointerState moves to either the Enter state or the Exit state. When this duration has elapsed after the user's gaze first enters a control, the PointerState is set to Enter. And when this duration has elapsed after the user's gaze has left the control, the PointerState is set to Exit. In both cases, a StateChanged event is fired. The default is 50ms.
        /// </summary>
        public static void SetThresholdDuration(UIElement element, TimeSpan value) { element.SetValue(ThresholdDurationProperty, value); }

        /// <summary>
        /// Sets the maximum times the control will invoked repeatedly without the user's gaze having to leave and re-enter the control. The default value is zero which disables repeated invocation of a control. Developers can set a higher value to enable repeated invocation.
        /// </summary>
        public static void SetMaxDwellRepeatCount(UIElement element, int value) { element.SetValue(MaxDwellRepeatCountProperty, value); }

        /// <summary>
        /// Sets the Boolean indicating whether gaze plus switch is enabled.
        /// </summary>
        public static void SetIsSwitchEnabled(UIElement element, bool value) { element.SetValue(IsSwitchEnabledProperty, value); }

        /// <summary>
        /// Gets the GazePointer object.
        /// </summary>
        public static GazePointer GetGazePointer(Page page)
        {
            return GazePointer.Instance;
        }

        /// <summary>
        /// Invoke the default action of the specified UIElement.
        /// </summary>
        public static void Invoke(UIElement element)
        {
            var item = GazeTargetItem.GetOrCreate(element);
            item.Invoke();
        }

        /// <summary>
        /// Reports whether a gaze input device is available, and hence whether there is any possibility of gaze events occurring in the application.
        /// </summary>
        public static bool IsDeviceAvailable
        {
            get
            {
                return GazePointer.Instance.IsDeviceAvailable;
            }
        }

        /// <summary>
        /// Event triggered whenever IsDeviceAvailable changes value.
        /// </summary>
        public static event EventHandler<Object> IsDeviceAvailableChanged
        {
            add { return GazePointer.Instance.IsDeviceAvailableChanged += value; }
            remove { GazePointer.Instance.IsDeviceAvailableChanged -= value; }
        }

        /// <summary>
        /// Loads a settings collection into GazeInput.
        /// Note: This must be loaded from a UI thread to be valid, since the GazeInput
        /// instance is tied to the UI thread.
        /// </summary>
        public static void LoadSettings(ValueSet settings)
        {
            GazePointer.Instance.LoadSettings(settings);
        }
    }
}
