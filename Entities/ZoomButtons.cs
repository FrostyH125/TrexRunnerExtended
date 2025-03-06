using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrexRunner.Graphics;

namespace TrexRunner.Entities
{
    public class ZoomButtons : IGameEntity
    {
        private const int ONE_X_ZOOM_BUTTON_TEXTURE_CORD_X = 38;
        private const int ONE_X_ZOOM_BUTTON_TEXTURE_CORD_Y = 70;

        private const int TWO_X_ZOOM_BUTTON_TEXTURE_CORD_X = 1;
        private const int TWO_X_ZOOM_BUTTON_TEXTURE_CORD_Y = 70;

        private const int FOUR_X_ZOOM_BUTTON_TEXTURE_CORD_X = 1;
        private const int FOUR_X_ZOOM_BUTTON_TEXTURE_CORD_Y = 103;

        private const int ZOOM_BUTTON_WIDTH = 38;
        private const int ZOOM_BUTTON_HEIGHT = 34;

        private const int ZOOM_BUTTON_MARGIN = 5;

        private Sprite _oneXButtonSprite;
        private Sprite _twoXButtonSprite;
        private Sprite _fourXButtonSprite;

        public int DrawOrder => 100;

        public bool IsEnabled { get; set; }

        private Texture2D _spriteSheetTexture;

        private TrexRunnerGame _game;

        private Vector2 _position;

        private MouseState previousMouseState;

        private Rectangle One_X_ButtonBounds
            => new Rectangle(
                (_position * _game.ZoomFactor).ToPoint(),
                new Point(
                    (int)(ZOOM_BUTTON_WIDTH * _game.ZoomFactor),
                    (int)(ZOOM_BUTTON_HEIGHT * _game.ZoomFactor)));

        private Rectangle Two_X_ButtonBounds
            => new Rectangle(
                ((_position + new Vector2(ZOOM_BUTTON_MARGIN + ZOOM_BUTTON_WIDTH, 0)) * _game.ZoomFactor).ToPoint(),
                new Point(
                    (int)(ZOOM_BUTTON_WIDTH * _game.ZoomFactor),
                    (int)(ZOOM_BUTTON_HEIGHT * _game.ZoomFactor)));
        
        private Rectangle Four_X_ButtonBounds
            => new Rectangle(
                ((_position + new Vector2((ZOOM_BUTTON_MARGIN + ZOOM_BUTTON_WIDTH) * 2, 0)) * _game.ZoomFactor).ToPoint(),
                new Point(
                    (int)(ZOOM_BUTTON_WIDTH * _game.ZoomFactor),
                    (int)(ZOOM_BUTTON_HEIGHT * _game.ZoomFactor)));

        public ZoomButtons(Texture2D spriteSheetTexture, Vector2 position, TrexRunnerGame game)
        {
            _spriteSheetTexture = spriteSheetTexture;
            _game = game;
            _position = position;

            _oneXButtonSprite = new Sprite(
                _spriteSheetTexture,
                ONE_X_ZOOM_BUTTON_TEXTURE_CORD_X,
                ONE_X_ZOOM_BUTTON_TEXTURE_CORD_Y,
                ZOOM_BUTTON_WIDTH,
                ZOOM_BUTTON_HEIGHT
            );

            _twoXButtonSprite = new Sprite(
                _spriteSheetTexture, 
                TWO_X_ZOOM_BUTTON_TEXTURE_CORD_X, 
                TWO_X_ZOOM_BUTTON_TEXTURE_CORD_Y, 
                ZOOM_BUTTON_WIDTH, 
                ZOOM_BUTTON_HEIGHT);
            
            _fourXButtonSprite = new Sprite(
                _spriteSheetTexture, 
                FOUR_X_ZOOM_BUTTON_TEXTURE_CORD_X, 
                FOUR_X_ZOOM_BUTTON_TEXTURE_CORD_Y, 
                ZOOM_BUTTON_WIDTH, 
                ZOOM_BUTTON_HEIGHT);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(!IsEnabled)
                return;
            
            _oneXButtonSprite.Draw(spriteBatch, _position);
            _twoXButtonSprite.Draw(spriteBatch, _position + new Vector2(ZOOM_BUTTON_WIDTH + ZOOM_BUTTON_MARGIN, 0));
            _fourXButtonSprite.Draw(spriteBatch, _position + new Vector2((ZOOM_BUTTON_WIDTH + ZOOM_BUTTON_MARGIN) * 2, 0));

        }

        public void Update(GameTime gameTime)
        {
            if(!IsEnabled)
                return;
            
            MouseState mouseState = Mouse.GetState();
            bool IsClicking = mouseState.LeftButton == ButtonState.Pressed;
            bool WasClicking = previousMouseState.LeftButton == ButtonState.Pressed;

            if((One_X_ButtonBounds.Contains(mouseState.Position) && IsClicking && !WasClicking))
            {
                _game.ToggleDisplay1X();
            }

            if((Two_X_ButtonBounds.Contains(mouseState.Position) && IsClicking && !WasClicking))
            {
                _game.ToggleDisplay2X();
            }

            if((Four_X_ButtonBounds.Contains(mouseState.Position) && IsClicking && !WasClicking))
            {
                _game.ToggleDisplay4X();
            }

            previousMouseState = mouseState;
            
        }

    }
}