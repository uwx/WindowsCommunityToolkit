//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    internal abstract class GazeTargetItem
    {
        internal TimeSpan DetailedTime { get; set; }
        internal TimeSpan OverflowTime { get; set; }
        internal TimeSpan ElapsedTime
        {
            get { return DetailedTime + OverflowTime; }
        }
        internal TimeSpan NextStateTime { get; set; }
        internal TimeSpan LastTimestamp { get; set; }
        internal PointerState ElementState { get; set; }
        internal UIElement TargetElement { get; set; }
        internal int RepeatCount { get; set; }
        internal int MaxDwellRepeatCount { get; set; }

        internal GazeTargetItem(UIElement target)
        {
            TargetElement = target;
        }

        internal static GazeTargetItem GetOrCreate(UIElement element) { throw new ToDoException(); }

        internal abstract void Invoke();

        internal virtual bool IsInvokable
        {
            get { return true; }
        }

        internal void Reset(TimeSpan nextStateTime)
        {
            ElementState = PointerState.PreEnter;
            DetailedTime = TimeSpan.Zero;
            OverflowTime = TimeSpan.Zero;
            NextStateTime = nextStateTime;
            RepeatCount = 0;
            MaxDwellRepeatCount = GazeInput.GetMaxDwellRepeatCount(TargetElement);
        }

        internal void GiveFeedback()
        {
            if (_nextStateTime != NextStateTime)
            {
                _prevStateTime = _nextStateTime;
                _nextStateTime = NextStateTime;
            }

            if (ElementState != _notifiedPointerState)
            {
                switch (ElementState)
                {
                    case PointerState.Enter:
                        RaiseProgressEvent(DwellProgressState.Fixating);
                        break;

                    case PointerState.Dwell:
                    case PointerState.Fixation:
                        RaiseProgressEvent(DwellProgressState.Progressing);
                        break;

                    case PointerState.Exit:
                    case PointerState.PreEnter:
                        RaiseProgressEvent(DwellProgressState.Idle);
                        break;
                }

                _notifiedPointerState = ElementState;
            }
            else if (ElementState == PointerState.Dwell || ElementState == PointerState.Fixation)
            {
                if (RepeatCount <= MaxDwellRepeatCount)
                {
                    RaiseProgressEvent(DwellProgressState.Progressing);
                }
                else
                {
                    RaiseProgressEvent(DwellProgressState.Complete);
                }
            }
        }

        private void RaiseProgressEvent(DwellProgressState state) { throw new ToDoException(); }

        private PointerState _notifiedPointerState = PointerState.Exit;
        private TimeSpan _prevStateTime;
        private TimeSpan _nextStateTime;
        private DwellProgressState _notifiedProgressState = DwellProgressState.Idle;
        private Popup _feedbackPopup;
    }
}
