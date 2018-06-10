// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

extern alias IOS;

using System;
using osu.Framework.Input.Handlers;
using IOS::Foundation;
using IOS::UIKit;
using osu.Framework.Input;
using OpenTK.Input;
using System.Collections.Generic;

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
                bool upper = false;

                Key? key = keyForString(char.ToString(c), out upper);

                if (key.HasValue)
                {
                    var basicState = new Framework.Input.KeyboardState();
                    if (upper)
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
            bool upper = false;
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
                key = keyForString(cmd.Input, out upper);

            if (key.HasValue)
            {
                bool shiftHeld = (cmd.ModifierFlags & UIKeyModifierFlags.Shift) > 0 || upper;
                bool superHeld = (cmd.ModifierFlags & UIKeyModifierFlags.Command) > 0;
                bool ctrlHeld = (cmd.ModifierFlags & UIKeyModifierFlags.Control) > 0;
                bool optionHeld = (cmd.ModifierFlags & UIKeyModifierFlags.Alternate) > 0;

                var keys = new List<Key>();
                keys.Add(key.Value);

                if (shiftHeld) keys.Add(Key.LShift);
                if (superHeld) keys.Add(Key.LWin);
                if (ctrlHeld) keys.Add(Key.LControl);
                if (optionHeld) keys.Add(Key.LAlt);

                PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState { Keys = keys.ToArray() } });
                PendingStates.Enqueue(new InputState { Keyboard = new Framework.Input.KeyboardState() });
            }
        }

        private Key? keyForString(string str, out bool upper)
        {
            upper = false;
            if (str.Length == 0)
                return null;

            char c = str[0];
            switch(c)
            {
                case '\t':
                    return Key.Tab;

                case '1':
                case '!':
                    upper = !char.IsDigit(c);
                    return Key.Number1;

                case '2':
                case '@':
                    upper = !char.IsDigit(c);
                    return Key.Number2;

                case '3':
                case '#':
                    upper = !char.IsDigit(c);
                    return Key.Number3;

                case '4':
                case '$':
                    upper = !char.IsDigit(c);
                    return Key.Number4;

                case '5':
                case '%':
                    upper = !char.IsDigit(c);
                    return Key.Number5;

                case '6':
                case '^':
                    upper = !char.IsDigit(c);
                    return Key.Number6;

                case '7':
                case '&':
                    upper = !char.IsDigit(c);
                    return Key.Number7;

                case '8':
                case '*':
                    upper = !char.IsDigit(c);
                    return Key.Number8;

                case '9':
                case '(':
                    upper = !char.IsDigit(c);
                    return Key.Number9;

                case '0':
                case ')':
                    upper = !char.IsDigit(c);
                    return Key.Number0;

                case '-':
                case '_':
                    upper = c == '_';
                    return Key.Minus;

                case '=':
                case '+':
                    upper = c == '+';
                    return Key.Plus;

                case '`':
                case '~':
                    upper = c == '~';
                    return Key.Tilde;

                case '[':
                case '{':
                    upper = c == '{';
                    return Key.BracketLeft;

                case ']':
                case '}':
                    upper = c == '}';
                    return Key.BracketRight;

                case '\\':
                case '|':
                    upper = c == '|';
                    return Key.BackSlash;

                case ';':
                case ':':
                    upper = c == ':';
                    return Key.Semicolon;

                case '\'':
                case '\"':
                    upper = c == '\"';
                    return Key.Quote;

                case ',':
                case '<':
                    upper = c == '<';
                    return Key.Comma;

                case '.':
                case '>':
                    upper = c == '>';
                    return Key.Period;

                case '/':
                case '?':
                    upper = c == '?';
                    return Key.Slash;

                default:
                    if (char.IsLetter(c))
                    {
                        string keyName = c.ToString().ToUpper();
                        Key result;
                        if (Enum.TryParse(keyName, out result))
                            return result;
                    }
                    return null;
            }
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
