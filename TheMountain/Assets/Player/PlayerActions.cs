namespace Player.Input
{
    using InControl;
    using UnityEngine;


    public class PlayerActions : PlayerActionSet
    {
        public PlayerAction LightAttack;
        public PlayerAction HeavyAttack;
        public PlayerAction Jump;
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerAction Sprint;
        public PlayerAction Crouch;
        public PlayerAction MouseRight;
        public PlayerAction MouseLeft;
        public PlayerAction MouseUp;
        public PlayerAction MouseDown;

        public PlayerTwoAxisAction Move;
        public PlayerTwoAxisAction MoveMouse;

        public PlayerActions()
        {
            LightAttack = CreatePlayerAction("LightAttack");
            HeavyAttack = CreatePlayerAction("HeavyAttack");
            Jump = CreatePlayerAction("Jump");
            Left = CreatePlayerAction("Move Left");
            Right = CreatePlayerAction("Move Right");
            Up = CreatePlayerAction("Move Up");
            Down = CreatePlayerAction("Move Down");
            Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
            Sprint = CreatePlayerAction("Sprint");
            Crouch = CreatePlayerAction("Crouch");
            MouseLeft = CreatePlayerAction("Mouse Left");
            MouseRight = CreatePlayerAction("Mouse Right");
            MouseUp = CreatePlayerAction("Mouse Up");
            MouseDown = CreatePlayerAction("Mouse Down");
            MoveMouse = CreateTwoAxisPlayerAction(MouseLeft, MouseRight, MouseDown, MouseUp);
        }


        public static PlayerActions CreateWithDefaultBindings()
        {
            var playerActions = new PlayerActions();

            // How to set up mutually exclusive keyboard bindings with a modifier key.
            // playerActions.Back.AddDefaultBinding( Key.Shift, Key.Tab );
            // playerActions.Next.AddDefaultBinding( KeyCombo.With( Key.Tab ).AndNot( Key.Shift ) );
            
            // Action 1 is X, Action 2 is O, Action 3 is [], Action 4 is /\
            playerActions.LightAttack.AddDefaultBinding(InputControlType.Action3);
            playerActions.LightAttack.AddDefaultBinding(Mouse.LeftButton);
            
            playerActions.HeavyAttack.AddDefaultBinding(InputControlType.Action4);
            playerActions.HeavyAttack.AddDefaultBinding(Mouse.RightButton);

            playerActions.Jump.AddDefaultBinding(Key.Space);
            playerActions.Jump.AddDefaultBinding(InputControlType.Action3);
            playerActions.Jump.AddDefaultBinding(InputControlType.Back);

            playerActions.Sprint.AddDefaultBinding(Key.LeftShift);
            playerActions.Sprint.AddDefaultBinding(InputControlType.LeftTrigger);

            playerActions.Crouch.AddDefaultBinding(Key.C);
            playerActions.Crouch.AddDefaultBinding(InputControlType.RightTrigger);

            playerActions.Up.AddDefaultBinding(Key.UpArrow);
            playerActions.Up.AddDefaultBinding(Key.W);
            playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
            playerActions.Up.AddDefaultBinding(InputControlType.DPadUp);

 

            playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
            playerActions.Down.AddDefaultBinding(Key.DownArrow);
            playerActions.Down.AddDefaultBinding(Key.S);
            playerActions.Down.AddDefaultBinding(InputControlType.DPadDown);


            playerActions.Left.AddDefaultBinding(Key.LeftArrow);
            playerActions.Left.AddDefaultBinding(Key.A);
            playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            playerActions.Left.AddDefaultBinding(InputControlType.DPadLeft);


            playerActions.Right.AddDefaultBinding(Key.RightArrow);
            playerActions.Right.AddDefaultBinding(Key.D);
            playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
            playerActions.Right.AddDefaultBinding(InputControlType.DPadRight);

            playerActions.MouseUp.AddDefaultBinding(Mouse.PositiveY);
            playerActions.MouseUp.AddDefaultBinding(InputControlType.RightStickUp);

            playerActions.MouseDown.AddDefaultBinding(Mouse.NegativeY);
            playerActions.MouseDown.AddDefaultBinding(InputControlType.RightStickDown);

            playerActions.MouseLeft.AddDefaultBinding(Mouse.NegativeX);
            playerActions.MouseLeft.AddDefaultBinding(InputControlType.RightStickLeft);

            playerActions.MouseRight.AddDefaultBinding(Mouse.PositiveX);
            playerActions.MouseRight.AddDefaultBinding(InputControlType.RightStickRight);

            playerActions.ListenOptions.IncludeUnknownControllers = true;
            playerActions.ListenOptions.MaxAllowedBindings = 4;
            //playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
            //playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
            playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
            //playerActions.ListenOptions.IncludeMouseButtons = true;
            //playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
            //playerActions.ListenOptions.IncludeMouseButtons = true;
            //playerActions.ListenOptions.IncludeMouseScrollWheel = true;

            playerActions.ListenOptions.OnBindingFound = (action, binding) => {
                if (binding == new KeyBindingSource(Key.Escape))
                {
                    action.StopListeningForBinding();
                    return false;
                }
                return true;
            };

            playerActions.ListenOptions.OnBindingAdded += (action, binding) => {
                Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
            };

            playerActions.ListenOptions.OnBindingRejected += (action, binding, reason) => {
                Debug.Log("Binding rejected... " + reason);
            };

            return playerActions;
        }
    }
}
