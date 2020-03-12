//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    public sealed partial class GazePointer
    {
        internal sealed class NonInvokeGazeTargetItem : GazeTargetItem
        {
            internal NonInvokeGazeTargetItem()
                : base(new Page())
            {
            }

            internal override bool IsInvokable { get { return false; } }

            internal override void Invoke()
            {
            }
        };

        internal static GazePointer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GazePointer();
                }
                return _instance;
            }
        }
        [ThreadStatic]
        static GazePointer _instance;

        internal void AddRoot(int proxyId)
        {
            _roots.Insert(0, proxyId);

            if (_roots.Count == 1)
            {
                _isShuttingDown = false;
                InitializeGazeInputSource();
            }
        }

        internal void RemoveRoot(int proxyId)
        {
            var index = _roots.IndexOf(proxyId);
            if (index != -1)
            {
                _roots.RemoveAt(index);
            }
            else
            {
                Debug.Fail("Did not find proxyId");
            }

            if (_roots.Count == 0)
            {
                _isShuttingDown = true;
                _gazeCursor.IsGazeEntered = false;
                DeinitializeGazeInputSource();
            }
        }

        private GazePointer()
        {
            _nonInvokeGazeTargetItem = new NonInvokeGazeTargetItem();

            // Default to not filtering sample data
            Filter = new NullFilter();

            _gazeCursor = new GazeCursor();

            // timer that gets called back if there gaze samples haven't been received in a while
            _eyesOffTimer = new DispatcherTimer();
            _eyesOffTimer.Tick += OnEyesOff;

            // provide a default of GAZE_IDLE_TIME microseconds to fire eyes off 
            EyesOffDelay = GAZE_IDLE_TIME;

            InitializeHistogram();

            _devices = new List<GazeDevicePreview>();
            _watcher = GazeInputSourcePreview.CreateWatcher();
            _watcher.Added += OnDeviceAdded;
            _watcher.Removed += OnDeviceRemoved;
            _watcher.Start();
        }

        public event EventHandler<GazeEventArgs> GazeEvent
        {
            add
            {
                return EventRegistrationTokenTable<EventHandler<GazeEventArgs>>.
                    GetOrCreateEventRegistrationTokenTable(ref _gazeEventTokenTable).
                    AddEventHandler(value);
            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<GazeEventArgs>>.
                    GetOrCreateEventRegistrationTokenTable(ref _gazeEventTokenTable).
                    RemoveEventHandler(value);
            }
        }
        private static EventRegistrationTokenTable<EventHandler<GazeEventArgs>>
            _gazeEventTokenTable = null;

        private void OnDeviceAdded(GazeDeviceWatcherPreview sender, GazeDeviceWatcherAddedPreviewEventArgs args)
        {
            _devices.Add(args.Device);

            if (_devices.Count == 1)
            {
                IsDeviceAvailableChanged?.Invoke(null, null);

                InitializeGazeInputSource();
            }
        }

        private void OnDeviceRemoved(GazeDeviceWatcherPreview sender, GazeDeviceWatcherRemovedPreviewEventArgs args)
        {
            var index = 0;
            while (index < _devices.Count && _devices[index].Id != args.Device.Id)
            {
                index++;
            }

            if (index < _devices.Count)
            {
                _devices.RemoveAt(index);
            }
            else
            {
                _devices.RemoveAt(0);
            }

            if (_devices.Count == 0)
            {
                IsDeviceAvailableChanged(null, null);
            }
        }

        ~GazePointer()
        {
            _watcher.Added -= OnDeviceAdded;
            _watcher.Removed -= OnDeviceRemoved;

            if (_gazeInputSource != null)
            {
                _gazeInputSource.GazeEntered -= OnGazeEntered;
                _gazeInputSource.GazeMoved -= OnGazeMoved;
                _gazeInputSource.GazeExited -= OnGazeExited;
            }
        }

        private static TimeSpan TimeSpanFromMicroseconds(int value)
        {
            return TimeSpan.FromMilliseconds(value / 1000.0);
        }

        private static TimeSpan TimeSpanFromMicroseconds(ulong value)
        {
            return TimeSpan.FromMilliseconds(value / 1000.0);
        }

        /// <summary>
        /// Loads a settings collection into GazePointer.
        /// </summary>
        public void LoadSettings(ValueSet settings)
        {
            _gazeCursor.LoadSettings(settings);
            Filter.LoadSettings(settings);

            // TODO Add logic to protect against missing settings

            if (settings.ContainsKey("GazePointer.FixationDelay"))
            {
                _defaultFixation = TimeSpanFromMicroseconds((int)(settings["GazePointer.FixationDelay"]));
            }

            if (settings.ContainsKey("GazePointer.DwellDelay"))
            {
                _defaultDwell = TimeSpanFromMicroseconds((int)(settings["GazePointer.DwellDelay"]));
            }

            if (settings.ContainsKey("GazePointer.DwellRepeatDelay"))
            {
                _defaultDwellRepeatDelay = TimeSpanFromMicroseconds((int)(settings["GazePointer.DwellRepeatDelay"]));
            }

            if (settings.ContainsKey("GazePointer.RepeatDelay"))
            {
                _defaultRepeatDelay = TimeSpanFromMicroseconds((int)(settings["GazePointer.RepeatDelay"]));
            }

            if (settings.ContainsKey("GazePointer.ThresholdDelay"))
            {
                _defaultThreshold = TimeSpanFromMicroseconds((int)(settings["GazePointer.ThresholdDelay"]));
            }

            // TODO need to set fixation and dwell for all elements
            if (settings.ContainsKey("GazePointer.FixationDelay"))
            {
                SetElementStateDelay(_offScreenElement, PointerState.Fixation, TimeSpanFromMicroseconds((int)(settings["GazePointer.FixationDelay"])));
            }
            if (settings.ContainsKey("GazePointer.DwellDelay"))
            {
                SetElementStateDelay(_offScreenElement, PointerState.Dwell, TimeSpanFromMicroseconds((int)(settings["GazePointer.DwellDelay"])));
            }

            if (settings.ContainsKey("GazePointer.GazeIdleTime"))
            {
                EyesOffDelay = TimeSpanFromMicroseconds((int)(settings["GazePointer.GazeIdleTime"]));
            }

            if (settings.ContainsKey("GazePointer.IsSwitchEnabled"))
            {
                IsSwitchEnabled = (bool)(settings["GazePointer.IsSwitchEnabled"]);
            }
        }

        private void InitializeHistogram()
        {
            _activeHitTargetTimes = new List<GazeTargetItem>();

            _offScreenElement = new UserControl();
            SetElementStateDelay(_offScreenElement, PointerState.Fixation, _defaultFixation);
            SetElementStateDelay(_offScreenElement, PointerState.Dwell, _defaultDwell);

            _maxHistoryTime = DEFAULT_MAX_HISTORY_DURATION;    // maintain about 3 seconds of history (in microseconds)
            _gazeHistory = new List<GazeHistoryItem>();
        }

        private void InitializeGazeInputSource()
        {
            if (!_initialized)
            {
                if (_roots.Count != 0 && _devices.Count != 0)
                {
                    if (_gazeInputSource == null)
                    {
                        _gazeInputSource = GazeInputSourcePreview.GetForCurrentView();
                    }

                    if (_gazeInputSource != null)
                    {
                        _gazeInputSource.GazeEntered += OnGazeEntered;
                        _gazeInputSource.GazeMoved += OnGazeMoved;
                        _gazeInputSource.GazeExited += OnGazeExited;

                        _initialized = true;
                    }
                }
            }
        }

        private void DeinitializeGazeInputSource()
        {
            if (_initialized)
            {
                _initialized = false;

                _gazeInputSource.GazeEntered -= OnGazeEntered;
                _gazeInputSource.GazeMoved -= OnGazeMoved;
                _gazeInputSource.GazeExited -= OnGazeExited;
            }
        }

        private static DependencyProperty GetProperty(PointerState state)
        {
            switch (state)
            {
                case PointerState.Fixation: return GazeInput.FixationDurationProperty;
                case PointerState.Dwell: return GazeInput.DwellDurationProperty;
                case PointerState.DwellRepeat: return GazeInput.DwellRepeatDurationProperty;
                case PointerState.Enter: return GazeInput.ThresholdDurationProperty;
                case PointerState.Exit: return GazeInput.ThresholdDurationProperty;
                default: return null;
            }
        }

        private TimeSpan GetDefaultPropertyValue(PointerState state)
        {
            switch (state)
            {
                case PointerState.Fixation: return _defaultFixation;
                case PointerState.Dwell: return _defaultDwell;
                case PointerState.DwellRepeat: return _defaultRepeatDelay;
                case PointerState.Enter: return _defaultThreshold;
                case PointerState.Exit: return _defaultThreshold;
                default: throw new NotImplementedException();
            }
        }

        internal void SetElementStateDelay(UIElement element, PointerState pointerState, TimeSpan stateDelay)
        {
            var property = GetProperty(pointerState);
            element.SetValue(property, stateDelay);

            // fix up _maxHistoryTime in case the new param exceeds the history length we are currently tracking
            var dwellTime = GetElementStateDelay(element, PointerState.Dwell);
            var repeatTime = GetElementStateDelay(element, PointerState.DwellRepeat);
            _maxHistoryTime = new TimeSpan(2 * Math.Max(dwellTime.Ticks, repeatTime.Ticks));
        }

        /// <summary>
        /// Find the parent to inherit properties from.
        /// </summary>
        private static UIElement GetInheritenceParent(UIElement child)
        {
            // The result value.
            Object parent = null;

            // Get the automation peer...
            var peer = FrameworkElementAutomationPeer.FromElement(child);
            if (peer != null)
            {
                // ...if it exists, get the peer's parent...
                var peerParent = peer.Navigate(AutomationNavigationDirection.Parent) as FrameworkElementAutomationPeer;
                if (peerParent != null)
                {
                    // ...and if it has a parent, get the corresponding object.
                    parent = peerParent.Owner;
                }
            }

            // If the above failed to find a parent...
            if (parent == null)
            {
                // ...use the visual parent.
                parent = VisualTreeHelper.GetParent(child);
            }

            // Safely pun the value we found to a UIElement reference.
            return parent as UIElement;
        }

        internal TimeSpan GetElementStateDelay(UIElement element, DependencyProperty property, TimeSpan defaultValue)
        {
            UIElement walker = element;
            Object valueAtWalker = walker.GetValue(property);

            while (GazeInput.UnsetTimeSpan.Equals(valueAtWalker) && walker != null)
            {
                walker = GetInheritenceParent(walker);

                if (walker != null)
                {
                    valueAtWalker = walker.GetValue(property);
                }
            }

            var ticks = GazeInput.UnsetTimeSpan.Equals(valueAtWalker) ? defaultValue : (TimeSpan)valueAtWalker;

            return ticks;
        }

        internal TimeSpan GetElementStateDelay(UIElement element, PointerState pointerState)
        {
            var property = GetProperty(pointerState);
            var defaultValue = GetDefaultPropertyValue(pointerState);
            var ticks = GetElementStateDelay(element, property, defaultValue);

            switch (pointerState)
            {
                case PointerState.Dwell:
                case PointerState.DwellRepeat:
                    _maxHistoryTime = new TimeSpan(Math.Max(_maxHistoryTime.Ticks, 2 * ticks.Ticks));
                    break;
            }

            return ticks;
        }

        internal void Reset()
        {
            _activeHitTargetTimes.Clear();
            _gazeHistory.Clear();

            _maxHistoryTime = DEFAULT_MAX_HISTORY_DURATION;
        }

        private GazeTargetItem GetHitTarget(Point gazePoint)
        {
            GazeTargetItem invokable;

            switch (Window.Current.CoreWindow.ActivationMode)
            {
                default:
                    if (!_isAlwaysActivated)
                    {
                        invokable = _nonInvokeGazeTargetItem;
                        break;
                    }
                    goto case CoreWindowActivationMode.ActivatedInForeground;

                case CoreWindowActivationMode.ActivatedInForeground:
                case CoreWindowActivationMode.ActivatedNotForeground:
                    var elements = VisualTreeHelper.FindElementsInHostCoordinates(gazePoint, null, false);
                    var element = elements.FirstOrDefault();

                    invokable = null;

                    if (element != null)
                    {
                        invokable = GazeTargetItem.GetOrCreate(element);

                        while (element != null && !invokable.IsInvokable)
                        {
                            element = VisualTreeHelper.GetParent(element) as UIElement;

                            if (element != null)
                            {
                                invokable = GazeTargetItem.GetOrCreate(element);
                            }
                        }
                    }

                    if (element == null || !invokable.IsInvokable)
                    {
                        invokable = _nonInvokeGazeTargetItem;
                    }
                    else
                    {
                        Interaction interaction;
                        do
                        {
                            interaction = GazeInput.GetInteraction(element);
                            if (interaction == Interaction.Inherited)
                            {
                                element = GetInheritenceParent(element);
                            }
                        } while (interaction == Interaction.Inherited && element != null);

                        if (interaction == Interaction.Inherited)
                        {
                            interaction = GazeInput.Interaction;
                        }

                        if (interaction != Interaction.Enabled)
                        {
                            invokable = _nonInvokeGazeTargetItem;
                        }
                    }
                    break;
            }

            return invokable;
        }

        private void ActivateGazeTargetItem(GazeTargetItem target)
        {
            var index = _activeHitTargetTimes.IndexOf(target);
            if (index == -1)
            {
                _activeHitTargetTimes.Append(target);

                // calculate the time that the first DwellRepeat needs to be fired after. this will be updated every time a DwellRepeat is 
                // fired to keep track of when the next one is to be fired after that.
                var nextStateTime = GetElementStateDelay(target.TargetElement, PointerState.Enter);

                target.Reset(nextStateTime);
            }
        }

        private GazeTargetItem ResolveHitTarget(Point gazePoint, TimeSpan timestamp)
        {
            // TODO: The existance of a GazeTargetItem should be used to indicate that
            // the target item is invokable. The method of invokation should be stored
            // within the GazeTargetItem when it is created and not recalculated when
            // subsequently needed.

            // create GazeHistoryItem to deal with this sample
            var target = GetHitTarget(gazePoint);
            var historyItem = new GazeHistoryItem();
            historyItem.HitTarget = target;
            historyItem.Timestamp = timestamp;
            historyItem.Duration = TimeSpan.Zero;
            Debug.Assert(historyItem.HitTarget != null);

            // create new GazeTargetItem with a (default) total elapsed time of zero if one does not exist already.
            // this ensures that there will always be an entry for target elements in the code below.
            ActivateGazeTargetItem(target);
            target.LastTimestamp = timestamp;

            // find elapsed time since we got the last hit target
            historyItem.Duration = timestamp - _lastTimestamp;
            if (historyItem.Duration > MAX_SINGLE_SAMPLE_DURATION)
            {
                historyItem.Duration = MAX_SINGLE_SAMPLE_DURATION;
            }
            _gazeHistory.Append(historyItem);

            // update the time this particular hit target has accumulated
            target.DetailedTime += historyItem.Duration;

            // drop the oldest samples from the list until we have samples only 
            // within the window we are monitoring
            //
            // historyItem is the last item we just appended a few lines above. 
            for (var evOldest = _gazeHistory[0];
                historyItem.Timestamp - evOldest.Timestamp > _maxHistoryTime;
                evOldest = _gazeHistory[0])
            {
                _gazeHistory.RemoveAt(0);

                // subtract the duration obtained from the oldest sample in _gazeHistory
                var targetItem = evOldest.HitTarget;
                Debug.Assert(targetItem.DetailedTime - evOldest.Duration >= TimeSpan.Zero);
                targetItem.DetailedTime -= evOldest.Duration;
                if (targetItem.ElementState != PointerState.PreEnter)
                {
                    targetItem.OverflowTime += evOldest.Duration;
                }
            }

            _lastTimestamp = timestamp;

            // Return the most recent hit target 
            // Intuition would tell us that we should return NOT the most recent
            // hitTarget, but the one with the most accumulated time in 
            // in the maintained history. But the effect of that is that
            // the user will feel that they have clicked on the wrong thing
            // when they are looking at something else.
            // That is why we return the most recent hitTarget so that 
            // when its dwell time has elapsed, it will be invoked
            return target;
        }

        private void OnEyesOff(Object sender, Object ea)
        {
            _eyesOffTimer.Stop();

            CheckIfExiting(_lastTimestamp + EyesOffDelay);
            RaiseGazePointerEvent(null, PointerState.Enter, EyesOffDelay);
        }

        private void CheckIfExiting(TimeSpan curTimestamp)
        {
            for (var index = 0; index < _activeHitTargetTimes.Count; index++)
            {
                var targetItem = _activeHitTargetTimes[index];
                var targetElement = targetItem.TargetElement;
                var exitDelay = GetElementStateDelay(targetElement, PointerState.Exit);

                var idleDuration = curTimestamp - targetItem.LastTimestamp;
                if (targetItem.ElementState != PointerState.PreEnter && idleDuration > exitDelay)
                {
                    targetItem.ElementState = PointerState.PreEnter;

                    // Transitioning to exit - clear the cached fixated element
                    _currentlyFixatedElement = null;

                    RaiseGazePointerEvent(targetItem, PointerState.Exit, targetItem.ElapsedTime);
                    targetItem.GiveFeedback();

                    _activeHitTargetTimes.RemoveAt(index);

                    // remove all history samples referring to deleted hit target
                    for (var i = 0; i < _gazeHistory.Count;)
                    {
                        var hitTarget = _gazeHistory[i].HitTarget;
                        if (hitTarget.TargetElement == targetElement)
                        {
                            _gazeHistory.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }

                    // return because only one element can be exited at a time and at this point
                    // we have done everything that we can do
                    return;
                }
            }
        }

        static string[] PointerStates = {
            "Exit",
            "PreEnter",
            "Enter",
            "Fixation",
            "Dwell",
            "DwellRepeat"
        };

        private void RaiseGazePointerEvent(GazeTargetItem target, PointerState state, TimeSpan elapsedTime)
        {
            var control = target != null ? target.TargetElement : null;
            //assert(target != _rootElement);
            var gpea = new StateChangedEventArgs(control, state, elapsedTime);
            //var buttonObj = dynamic_cast<Button >(target);
            //if (buttonObj && buttonObj.Content)
            //{
            //    String buttonText = dynamic_cast<String>(buttonObj.Content);
            //    Debug.WriteLine(L"GPE: %s . %s, %d", buttonText, PointerStates[(int)state], elapsedTime);
            //}
            //else
            //{
            //    Debug.WriteLine(L"GPE: 0x%08x . %s, %d", target != null ? target.GetHashCode() : 0, PointerStates[(int)state], elapsedTime);
            //}

            var gazeElement = target != null ? GazeInput.GetGazeElement(control) : null;

            if (gazeElement != null)
            {
                gazeElement.RaiseStateChanged(control, gpea);
            }

            if (state == PointerState.Dwell)
            {
                var handled = false;

                if (gazeElement != null)
                {
                    var args = new DwellInvokedRoutedEventArgs();
                    gazeElement.RaiseInvoked(control, args);
                    handled = args.Handled;
                }

                if (!handled)
                {
                    target.Invoke();
                }
            }
        }

        private void OnGazeEntered(GazeInputSourcePreview provider, GazeEnteredPreviewEventArgs args)
        {
            //Debug.WriteLine(L"Entered at %ld", args.CurrentPoint.Timestamp);
            _gazeCursor.IsGazeEntered = true;
        }

        private void OnGazeMoved(GazeInputSourcePreview provider, GazeMovedPreviewEventArgs args)
        {
            if (!_isShuttingDown)
            {
                var intermediatePoints = args.GetIntermediatePoints();
                foreach (var point in intermediatePoints)
                {
                    var position = point.EyeGazePosition;
                    if (position != null)
                    {
                        _gazeCursor.IsGazeEntered = true;
                        ProcessGazePoint(TimeSpanFromMicroseconds(point.Timestamp), position.Value);
                    }
                    else
                    {
                        //Debug.WriteLine(L"Null position eaten at %ld", point.Timestamp);
                    }
                }
            }
        }

        private void OnGazeExited(GazeInputSourcePreview provider, GazeExitedPreviewEventArgs args)
        {
            //Debug.WriteLine(L"Exited at %ld", args.CurrentPoint.Timestamp);
            _gazeCursor.IsGazeEntered = false;
        }

        private void ProcessGazePoint(TimeSpan timestamp, Point position)
        {
            var ea = new GazeFilterArgs(position, timestamp);

            var fa = Filter.Update(ea);
            _gazeCursor.Position = fa.Location;

            if (_gazeEventCount != 0)
            {
                _gazeEventArgs.Set(fa.Location, timestamp);
                var invokationList = EventRegistrationTokenTable<EventHandler<GazeEventArgs>>.
                    GetOrCreateEventRegistrationTokenTable(ref _gazeEventTokenTable).
                    InvocationList;
                if (invokationList != null)
                {
                    invokationList(this, _gazeEventArgs);
                }
                if (_gazeEventArgs.Handled)
                {
                    return;
                }
            }

            var targetItem = ResolveHitTarget(fa.Location, fa.Timestamp);
            Debug.Assert(targetItem != null);

            //Debug.WriteLine(L"ProcessGazePoint: %llu . [%d, %d], %llu", hitTarget.GetHashCode(), (int)fa.Location.X, (int)fa.Location.Y, fa.Timestamp);

            // check to see if any element in _hitTargetTimes needs an exit event fired.
            // this ensures that all exit events are fired before enter event
            CheckIfExiting(fa.Timestamp);

            PointerState nextState = (PointerState)((int)targetItem.ElementState + 1);

            //Debug.WriteLine(L"%llu . State=%d, Elapsed=%d, NextStateTime=%d", targetItem.TargetElement, targetItem.ElementState, targetItem.ElapsedTime, targetItem.NextStateTime);

            if (targetItem.ElapsedTime > targetItem.NextStateTime)
            {
                var prevStateTime = targetItem.NextStateTime;

                // prevent targetItem from ever actually transitioning into the DwellRepeat state so as
                // to continuously emit the DwellRepeat event
                if (nextState != PointerState.DwellRepeat)
                {
                    targetItem.ElementState = nextState;
                    nextState = (PointerState)((int)nextState + 1);     // nextState++
                    targetItem.NextStateTime += GetElementStateDelay(targetItem.TargetElement, nextState);

                    if (targetItem.ElementState == PointerState.Dwell)
                    {
                        targetItem.NextStateTime += GetElementStateDelay(targetItem.TargetElement, GazeInput.RepeatDelayDurationProperty, _defaultDwellRepeatDelay);
                    }
                }
                else
                {
                    // move the NextStateTime by one dwell period, while continuing to stay in Dwell state
                    targetItem.NextStateTime += GetElementStateDelay(targetItem.TargetElement, PointerState.DwellRepeat);
                }

                if (targetItem.ElementState == PointerState.Dwell)
                {
                    targetItem.RepeatCount++;
                    if (targetItem.MaxDwellRepeatCount < targetItem.RepeatCount)
                    {
                        targetItem.NextStateTime = TimeSpan.MaxValue;
                    }
                }

                if (targetItem.ElementState == PointerState.Fixation)
                {
                    // Cache the fixated item
                    _currentlyFixatedElement = targetItem;

                    // We are about to transition into the Dwell state
                    // If switch input is enabled, make sure dwell never completes
                    // via eye gaze
                    if (_isSwitchEnabled)
                    {
                        // Don't allow the next state (Dwell) to progress
                        targetItem.NextStateTime = TimeSpan.MaxValue;
                    }
                }

                RaiseGazePointerEvent(targetItem, targetItem.ElementState, targetItem.ElapsedTime);
            }

            targetItem.GiveFeedback();

            _eyesOffTimer.Start();
            _lastTimestamp = fa.Timestamp;
        }

        /// <summary>
        /// When in switch mode, will issue a click on the currently fixated element
        /// </summary>
        public void Click()
        {
            if (_isSwitchEnabled &&
                _currentlyFixatedElement != null)
            {
                _currentlyFixatedElement.Invoke();
            }
        }

        /// <summary>
        /// Run device calibration.
        /// </summary>
        public IAsyncOperation<bool> RequestCalibrationAsync()
        {
            if (_devices.Count == 1)
            {
                return _devices[0].RequestCalibrationAsync();
            }
            else
            {
                return null;
            }
        }

    }
}
