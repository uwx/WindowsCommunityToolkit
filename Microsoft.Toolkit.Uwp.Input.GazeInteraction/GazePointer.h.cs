//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    /// <summary>
    /// Class of singleton object coordinating gaze input.
    /// </summary>
    public sealed partial class GazePointer
    {
        // units in microseconds
        private static TimeSpan DEFAULT_FIXATION_DELAY = TimeSpan.FromMilliseconds(350);
        private static TimeSpan DEFAULT_DWELL_DELAY = TimeSpan.FromMilliseconds(400);
        private static TimeSpan DEFAULT_DWELL_REPEAT_DELAY = TimeSpan.FromMilliseconds(400);
        private static TimeSpan DEFAULT_REPEAT_DELAY = TimeSpan.FromMilliseconds(400);
        private static TimeSpan DEFAULT_THRESHOLD_DELAY = TimeSpan.FromMilliseconds(50);
        private static TimeSpan DEFAULT_MAX_HISTORY_DURATION = TimeSpan.FromMilliseconds(3000);
        private static TimeSpan MAX_SINGLE_SAMPLE_DURATION = TimeSpan.FromMilliseconds(100);

        private static TimeSpan GAZE_IDLE_TIME = TimeSpan.FromSeconds(2.5);

        /// <summary>
        /// The UIElement representing the cursor.
        /// </summary>
        public UIElement CursorElement
        {
            get { return _gazeCursor.PopupChild; }
            set { _gazeCursor.PopupChild = value; }
        }

        private readonly GazeEventArgs _gazeEventArgs = new GazeEventArgs();
        private int _gazeEventCount = 0;

        internal Brush _enterBrush = null;

        internal Brush _progressBrush = new SolidColorBrush(Colors.Green);

        internal Brush _completeBrush = new SolidColorBrush(Colors.Red);

        internal double _dwellStrokeThickness = 2;

        internal Interaction _interaction = Interaction.Disabled;

        internal GazeTargetItem _nonInvokeGazeTargetItem;

        internal GazeFeedbackPopupFactory _gazeFeedbackPopupFactory = new GazeFeedbackPopupFactory();

        // Provide a configurable delay for when the EyesOffDelay event is fired
        // GOTCHA: this value requires that _eyesOffTimer is instantiated so that it
        // can update the timer interval 
        internal TimeSpan EyesOffDelay
        {
            get { return _eyesOffDelay; }
            set
            {
                _eyesOffDelay = value;

                // convert GAZE_IDLE_TIME units (microseconds) to 100-nanosecond units used
                // by TimeSpan struct
                _eyesOffTimer.Interval = EyesOffDelay;
            }
        }

        // Pluggable filter for eye tracking sample data. This defaults to being set to the
        // NullFilter which performs no filtering of input samples.
        internal IGazeFilter Filter { get; set; }

        internal bool IsCursorVisible
        {
            get { return _gazeCursor.IsCursorVisible; }
            set { _gazeCursor.IsCursorVisible = value; }
        }

        internal int CursorRadius
        {
            get { return _gazeCursor.CursorRadius; }
            set { _gazeCursor.CursorRadius = value; }
        }

        internal bool IsSwitchEnabled
        {
            get { return _isSwitchEnabled; }
            set { _isSwitchEnabled = value; }
        }

        public bool IsAlwaysActivated
        {
            get { return _isAlwaysActivated; }
            set { _isAlwaysActivated = value; }
        }

        internal bool IsDeviceAvailable
        {
            get { return _devices.Count != 0; }
        }
        internal event EventHandler<Object> IsDeviceAvailableChanged;

        private bool _initialized;
        private bool _isShuttingDown;

        private List<int> _roots = new List<int>();

        private TimeSpan _eyesOffDelay;

        private GazeCursor _gazeCursor;
        private DispatcherTimer _eyesOffTimer;

        // _offScreenElement is a pseudo-element that represents the area outside
        // the screen so we can track how long the user has been looking outside
        // the screen and appropriately trigger the EyesOff event
        private Control _offScreenElement;

        // The value is the total time that FrameworkElement has been gazed at
        private List<GazeTargetItem> _activeHitTargetTimes;

        // A vector to track the history of observed gaze targets
        private List<GazeHistoryItem> _gazeHistory;
        private TimeSpan _maxHistoryTime;

        // Used to determine if exit events need to be fired by adding GAZE_IDLE_TIME to the last 
        // saved timestamp
        private TimeSpan _lastTimestamp;

        private GazeInputSourcePreview _gazeInputSource;

        private GazeDeviceWatcherPreview _watcher;
        private List<GazeDevicePreview> _devices;

        private TimeSpan _defaultFixation = DEFAULT_FIXATION_DELAY;
        private TimeSpan _defaultDwell = DEFAULT_DWELL_DELAY;
        private TimeSpan _defaultDwellRepeatDelay = DEFAULT_DWELL_REPEAT_DELAY;
        private TimeSpan _defaultRepeatDelay = DEFAULT_REPEAT_DELAY;
        private TimeSpan _defaultThreshold = DEFAULT_THRESHOLD_DELAY;

        private bool _isAlwaysActivated;
        private bool _isSwitchEnabled;
        private GazeTargetItem _currentlyFixatedElement;
    }
}
