//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    public static partial class GazeInput
    {
        /// <summary>
        /// Gets or sets the brush to use when displaying the default indication that gaze entered a control
        /// </summary>
        public static Brush DwellFeedbackEnterBrush
        {
            get
            {
                return GazePointer.Instance._enterBrush;
            }

            set
            {
                GazePointer.Instance._enterBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush to use when displaying the default animation for dwell press
        /// </summary>
        public static Brush DwellFeedbackProgressBrush
        {
            get
            {
                return GazePointer.Instance._progressBrush;
            }

            set
            {
                GazePointer.Instance._progressBrush = value;
            }
        }


        /// <summary>
        /// Gets or sets the brush to use when displaying the default animation for dwell complete
        /// </summary>
        public static Brush DwellFeedbackCompleteBrush
        {
            get
            {
                return GazePointer.Instance._completeBrush;
            }

            set
            {
                GazePointer.Instance._completeBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the lines animated for dwell.
        /// </summary>
        public static double DwellStrokeThickness
        {
            get
            {
                return GazePointer.Instance._dwellStrokeThickness;
            }

            set
            {
                GazePointer.Instance._dwellStrokeThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the interaction default
        /// </summary>
        public static Interaction Interaction
        {
            get
            {
                return GazePointer.Instance._interaction;
            }

            set
            {
                if (GazePointer.Instance._interaction != value)
                {
                    if (value == GazeInteraction.Interaction.Enabled)
                    {
                        GazePointer.Instance.AddRoot(0);
                    }
                    else if (GazePointer.Instance._interaction == GazeInteraction.Interaction.Enabled)
                    {
                        GazePointer.Instance.RemoveRoot(0);
                    }

                    GazePointer.Instance._interaction = value;
                }
            }
        }


        internal static TimeSpan UnsetTimeSpan = new TimeSpan(-1);

        private static void OnInteractionChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            var element = (FrameworkElement)ob;
            var interaction = (Interaction)args.NewValue;
            GazePointerProxy.SetInteraction(element, interaction);
        }

        private static void OnIsCursorVisibleChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            GazePointer.Instance.IsCursorVisible = (bool)args.NewValue;
        }

        private static void OnCursorRadiusChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            GazePointer.Instance.CursorRadius = (int)args.NewValue;
        }

        static void OnIsSwitchEnabledChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            GazePointer.Instance.IsSwitchEnabled = (bool)args.NewValue;
        }

        static DependencyProperty s_interactionProperty = DependencyProperty.RegisterAttached("Interaction", typeof(Interaction), typeof(GazeInput),
            new PropertyMetadata(Interaction.Inherited, OnInteractionChanged));
        static DependencyProperty s_isCursorVisibleProperty = DependencyProperty.RegisterAttached("IsCursorVisible", typeof(bool), typeof(GazeInput),
            new PropertyMetadata(true, OnIsCursorVisibleChanged));
        static DependencyProperty s_cursorRadiusProperty = DependencyProperty.RegisterAttached("CursorRadius", typeof(int), typeof(GazeInput),
            new PropertyMetadata(6, OnCursorRadiusChanged));
        static DependencyProperty s_gazeElementProperty = DependencyProperty.RegisterAttached("GazeElement", typeof(GazeElement), typeof(GazeInput), new PropertyMetadata(null));
        static DependencyProperty s_fixationDurationProperty = DependencyProperty.RegisterAttached("FixationDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(GazeInput.UnsetTimeSpan));
        static DependencyProperty s_dwellDurationProperty = DependencyProperty.RegisterAttached("DwellDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(GazeInput.UnsetTimeSpan));
        static DependencyProperty s_repeatDelayDurationProperty = DependencyProperty.RegisterAttached("RepeatDelayDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(GazeInput.UnsetTimeSpan));
        static DependencyProperty s_dwellRepeatDurationProperty = DependencyProperty.RegisterAttached("DwellRepeatDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(GazeInput.UnsetTimeSpan));
        static DependencyProperty s_thresholdDurationProperty = DependencyProperty.RegisterAttached("ThresholdDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(GazeInput.UnsetTimeSpan));
        static DependencyProperty s_maxRepeatCountProperty = DependencyProperty.RegisterAttached("MaxDwellRepeatCount", typeof(int), typeof(GazeInput), new PropertyMetadata((Object)0));
        static DependencyProperty s_isSwitchEnabledProperty = DependencyProperty.RegisterAttached("IsSwitchEnabled", typeof(bool), typeof(GazeInput),
            new PropertyMetadata(false, OnIsSwitchEnabledChanged));
    }
}
