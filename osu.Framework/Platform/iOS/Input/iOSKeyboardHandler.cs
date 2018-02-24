// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

extern alias IOS;

using System;
using osu.Framework.Input.Handlers;
using IOS::Foundation;
using IOS::UIKit;
using osu.Framework.Input;
using OpenTK;
using OpenTK.Input;
using System.Linq;

namespace osu.Framework.Platform.iOS.Input
{
    public class iOSKeyboardHandler : InputHandler
    {
        private readonly iOSPlatformGameView view;

        public iOSKeyboardHandler(iOSPlatformGameView view)
        {
            this.view = view;
            view.KeyboardTextField.HandleShouldChangeCharacters += handleShouldChangeCharacters;
            view.KeyboardTextField.HandleShouldReturn += handleShouldReturn;
            view.KeyboardTextField.HandleKeyCommand += handleKeyCommand;
        }

        private void handleShouldChangeCharacters(NSRange range, string text)
        {
            if (text.Length == 0)
            {
                if (range.Length > 0)
                {
                    Key key = range.Location < iOSPlatformGameView.DummyTextField.cursor_position ? Key.BackSpace : Key.Delete;
                    PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState { Keys = new Key[] { key } } });
                    PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState() });
                }
                return;
            }

            bool keysAdded = false;
            foreach (char c in text)
            {
                bool shiftHeld = char.IsUpper(c);

                Key? key = keyForString(char.ToString(c));

                if (key.HasValue)
                {
                    var basicState = new Framework.Input.KeyboardState();
                    if (shiftHeld)
                        basicState.Keys = new Key[] { key.Value, Key.LShift };
                    else
                        basicState.Keys = new Key[] { key.Value };

                    PendingStates.Enqueue(new InputState { Keyboard = basicState });
                    keysAdded = true;
                }
            }

            if (keysAdded)
                PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState() });
        }

        private void handleShouldReturn()
        {
            PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState { Keys = new Key[] { Key.Enter }}});
            PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState() });
        }

        private void handleKeyCommand(UIKeyCommand cmd)
        {
            Key? key;
            // UIKeyCommand constants are not actually constants, so we can't use a switch
            if (cmd.Input == UIKeyCommand.LeftArrow)
                key = Key.Left;
            else if (cmd.Input == UIKeyCommand.RightArrow)
                key = Key.Right;
            else if (cmd.Input == UIKeyCommand.UpArrow)
                key = Key.Up;
            else if (cmd.Input == UIKeyCommand.DownArrow)
                key = Key.Down;
            else
                key = keyForString(cmd.Input);
            
            if (key.HasValue)
            {
                bool shiftHeld = (cmd.ModifierFlags & UIKeyModifierFlags.Shift) > 0;
                var basicState = new Framework.Input.KeyboardState();
                if (shiftHeld)
                    basicState.Keys = new Key[] { key.Value, Key.LShift };
                else
                    basicState.Keys = new Key[] { key.Value };
                
                PendingStates.Enqueue(new InputState { Keyboard = basicState });
                PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState() });
            }
        }

        private Key? keyForString(string str)
        {
            string keyName;

            if (str.Length > 1)
            {
                // TODO: special strings
                keyName = "";
            }
            else if (char.IsLetter(str[0]))
                keyName = char.ToString(str[0]).ToUpper();
            else if (char.IsDigit(str[0]))
                keyName = "Number" + str[0];
            else
                return null;

            Key result;
            if (Enum.TryParse(keyName, out result))
                return result;
            
            return null;
        }

        public override bool IsActive => true;

        public override int Priority => 0;

        protected override void Dispose(bool disposing)
        {
            view.KeyboardTextField.HandleShouldChangeCharacters -= handleShouldChangeCharacters;
            view.KeyboardTextField.HandleShouldReturn -= handleShouldReturn;
            view.KeyboardTextField.HandleKeyCommand -= handleKeyCommand;
            base.Dispose(disposing);
        }

        public override bool Initialize(GameHost host)
        {
            return true;
        }
    }
}
