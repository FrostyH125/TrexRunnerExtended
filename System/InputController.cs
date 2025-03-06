using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrexRunner.Entities;

namespace TrexRunner.System
{
    public class InputController
    {
        private bool _isBlocked;

        private Trex _trex;

        private KeyboardState _previousKeyboardState;

        public InputController(Trex trex)
        {
            _trex = trex;
        }

        public void ProcessControls(GameTime gameTime)
        {
            
            KeyboardState keyboardState = Keyboard.GetState();

            if(!_isBlocked)
            {
                bool isJumpKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space);
                bool wasJumpKeyPressed = _previousKeyboardState.IsKeyDown(Keys.Up) || _previousKeyboardState.IsKeyDown(Keys.Space);

                if (!wasJumpKeyPressed && isJumpKeyPressed)
                {
                    if(_trex.State != TrexState.Jumping)
                        _trex.BeginJump();

                }
                else if (_trex.State == TrexState.Jumping && !isJumpKeyPressed)
                    {
                        
                        _trex.CancelJump();
                        
                    }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {

                    if(_trex.State == TrexState.Jumping || _trex.State == TrexState.Falling)
                        _trex.Drop();
                    else
                        _trex.Duck();

                }
                else if (!keyboardState.IsKeyDown(Keys.Down) && _trex.State == TrexState.Ducking)
                {

                    _trex.GetUp();

                }


            }

            //if its blocked, it still needs the previous keyboard state to update to determine whether on the next pass
            //to activate any of these conditions (WasJumpKeyPressed will always return true when resetting the game
            //with the jump buttons, so it wont jump)
            _previousKeyboardState = keyboardState;

            _isBlocked = false;

        }

        //blocks input for a single pass of the update method (1 frame)
        public void BlockInputTemporarily()
        {
            _isBlocked = true;
        }

    }
}