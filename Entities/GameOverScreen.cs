using System.Numerics;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrexRunner.Graphics;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TrexRunner.Entities
{
    public class GameOverScreen : IGameEntity
    {
        private const int GAME_OVER_TEXTURE_POS_X = 655;
        private const int GAME_OVER_TEXTURE_POS_Y = 14;

        public const int GAME_OVER_SPRITE_WIDTH = 192;
        private const int GAME_OVER_SPRITE_HEIGHT = 14;

        private const int BUTTON_TEXTURE_POS_X = 1;
        private const int BUTTON_TEXTURE_POS_Y = 1;

        private const int BUTTON_SPRITE_WIDTH = 38;
        private const int BUTTON_SPRITE_HEIGHT = 38;

        private Sprite _textSprite;
        private Sprite _buttonSprite;

        private KeyboardState _previousKeyboardState;

        private TrexRunnerGame _game;

        public int DrawOrder => 100;

        public Vector2 Position { get; set; }

        public bool IsEnabled { get; set; }

        //defines the position of the button texture
        //uses the position of the whole game over texture rectangle as a starting reference point
        private Vector2 ButtonPosition => Position + new Vector2((GAME_OVER_SPRITE_WIDTH / 2) - (BUTTON_SPRITE_WIDTH / 2) , GAME_OVER_SPRITE_HEIGHT + 20);

        //gives the x and y position of the starting point of the rectangle, then defines the width and height of it
        //bounds change depending on the current display mode
        private Rectangle ButtonBounds
            => new Rectangle(
                (ButtonPosition * _game.ZoomFactor).ToPoint(),
                new Point(
                    (int)(BUTTON_SPRITE_WIDTH * _game.ZoomFactor),
                    (int)(BUTTON_SPRITE_HEIGHT * _game.ZoomFactor)));

        public GameOverScreen(Texture2D spriteSheet, TrexRunnerGame game)
        {
            _textSprite = new Sprite
            (
                spriteSheet, 
                GAME_OVER_TEXTURE_POS_X, 
                GAME_OVER_TEXTURE_POS_Y, 
                GAME_OVER_SPRITE_WIDTH, 
                GAME_OVER_SPRITE_HEIGHT
            );

            _buttonSprite = new Sprite
            (
                spriteSheet, 
                BUTTON_TEXTURE_POS_X, 
                BUTTON_TEXTURE_POS_Y, 
                BUTTON_SPRITE_WIDTH, 
                BUTTON_SPRITE_HEIGHT
            );

            _game = game;
            
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            _textSprite.Draw(spriteBatch, Position);
            _buttonSprite.Draw(spriteBatch, ButtonPosition);
        }

        public void Update(GameTime gameTime)
        {
            if(!IsEnabled)
                return;

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            bool isKeyPressed = keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up);
            bool wasKeyPressed = _previousKeyboardState.IsKeyDown(Keys.Space) || _previousKeyboardState.IsKeyDown(Keys.Up);

            if((ButtonBounds.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
                //checks if the key is released rather than pressed, as to prevent 
                //it from auto restarting if you die while holding jump
                || (wasKeyPressed && !isKeyPressed))
            {

                _game.Replay();

            }

            _previousKeyboardState = keyboardState;
        }

    }
}