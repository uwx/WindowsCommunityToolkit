//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System.Collections.Generic;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{

    internal class GazeFeedbackPopupFactory
    {
        private List<Popup> s_cache = new List<Popup>();

        public Popup Get() { throw new ToDoException(); }

        public void Return(Popup popup) { throw new ToDoException(); }
    }
}
