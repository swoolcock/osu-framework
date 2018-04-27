// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

extern alias IOS;

using System;
using System.Diagnostics;
using IOS::System.Drawing;

using IOS::Foundation;
using IOS::GLKit;
using IOS::OpenGLES;
using IOS::ObjCRuntime;
using IOS::CoreAnimation;
using IOS::CoreGraphics;
using IOS::UIKit;
using OpenTK;
using OpenTK.Graphics.ES20;
using OpenTK.Platform.iPhoneOS;
using System.Threading.Tasks;

namespace osu.Framework.Platform.iOS
{
    [Register("iOSPlatformGameView")]
    public class iOSPlatformGameView : iPhoneOSGameView
    {
        public event Action<NSSet> HandleTouches;

        public DummyTextField KeyboardTextField { get; private set; }

        [Export("layerClass")]
        static Class LayerClass()
        {
            return iPhoneOSGameView.GetLayerClass();
        }

        protected override void ConfigureLayer(CAEAGLLayer eaglLayer)
        {
            eaglLayer.Opaque = true;
            ExclusiveTouch = true;
            MultipleTouchEnabled = true;
            UserInteractionEnabled = true;
        }

        [Export("initWithFrame:")]
        public iOSPlatformGameView(IOS::System.Drawing.RectangleF frame) : base(frame)
        {
            LayerRetainsBacking = false;
            LayerColorFormat = EAGLColorFormat.RGBA8;
            ContextRenderingApi = EAGLRenderingAPI.OpenGLES3;

            Scale = (float)UIScreen.MainScreen.Scale;
            ContentScaleFactor = UIScreen.MainScreen.Scale;

            AddSubview(KeyboardTextField = new DummyTextField());
        }

        public float Scale { get; private set; }

        public override void TouchesBegan(NSSet touches, UIEvent evt) => HandleTouches?.Invoke(touches);
        public override void TouchesCancelled(NSSet touches, UIEvent evt) => HandleTouches?.Invoke(touches);
        public override void TouchesEnded(NSSet touches, UIEvent evt) => HandleTouches?.Invoke(touches);
        public override void TouchesMoved(NSSet touches, UIEvent evt) => HandleTouches?.Invoke(touches);

        public class DummyTextField : UITextField
        {
            public event Action<NSRange, string> HandleShouldChangeCharacters;
            public event Action HandleShouldReturn;
            public event Action<UIKeyCommand> HandleKeyCommand;

            public const int cursor_position = 5;
            private int responderSemaphore;

            public DummyTextField()
            {
                AutocapitalizationType = UITextAutocapitalizationType.None;
                AutocorrectionType = UITextAutocorrectionType.No;
                KeyboardType = UIKeyboardType.Default;
                KeyboardAppearance = UIKeyboardAppearance.Default;

                resetText();

                ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    resetText();
                    HandleShouldChangeCharacters?.Invoke(range, replacementString);
                    return false;
                };

                ShouldReturn = (textField) =>
                {
                    resetText();
                    HandleShouldReturn?.Invoke();
                    return false;
                };
            }

            public override UIKeyCommand[] KeyCommands => new[]
            {
                UIKeyCommand.Create(UIKeyCommand.LeftArrow, 0, new Selector("keyPressed:")),
                UIKeyCommand.Create(UIKeyCommand.RightArrow, 0, new Selector("keyPressed:")),
                UIKeyCommand.Create(UIKeyCommand.UpArrow, 0, new Selector("keyPressed:")),
                UIKeyCommand.Create(UIKeyCommand.DownArrow, 0, new Selector("keyPressed:"))
            };

            [Export("keyPressed:")]
            void keyPressed(UIKeyCommand cmd)
            {
                HandleKeyCommand?.Invoke(cmd);
            }

            private void resetText()
            {
                // we put in some dummy text and move the cursor to the middle so that backspace (and potentially delete or cursor keys) will be detected
                Text = "dummytext";
                var newPosition = GetPosition(BeginningOfDocument, cursor_position);
                SelectedTextRange = GetTextRange(newPosition, newPosition);
            }

            public void UpdateFirstResponder(bool become)
            {
                if (become)
                {
                    responderSemaphore = Math.Max(responderSemaphore + 1, 1);
                    InvokeOnMainThread(() => BecomeFirstResponder());
                }
                else
                {
                    responderSemaphore = Math.Max(responderSemaphore - 1, 0);
                    Task.Delay(200).ContinueWith(task =>
                    {
                        if (responderSemaphore <= 0)
                            InvokeOnMainThread(() => ResignFirstResponder());
                    });
                }
            }
        }
    }
}
