// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

extern alias IOS;

using System;
using IOS::Foundation;
using osu.Framework.Input;

namespace osu.Framework.Platform.iOS.Input
{
    public class iOSTextInput : ITextInputSource
    {
        private readonly iOSPlatformGameView view;

        private string pending = string.Empty;

        public iOSTextInput(iOSPlatformGameView view)
        {
            this.view = view;
        }

        public bool ImeActive => false;

        public string GetPendingText()
        {
            try
            {
                return pending;
            }
            finally
            {
                pending = string.Empty;
            }
        }

        private void handleShouldChangeCharacters(NSRange range, string text)
        {
            pending += text;
        }

        public void Deactivate(object sender)
        {
            view.KeyboardTextField.HandleShouldChangeCharacters -= handleShouldChangeCharacters;
        }

        public void Activate(object sender)
        {
            view.KeyboardTextField.HandleShouldChangeCharacters += handleShouldChangeCharacters;
        }

        public event Action<string> OnNewImeComposition;
        public event Action<string> OnNewImeResult;
    }
}
