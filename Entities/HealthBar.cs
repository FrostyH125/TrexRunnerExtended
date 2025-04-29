//TODO
//ADD TO ENTITIES


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrexRunner.Graphics;

namespace TrexRunner.Entities
{
    public class HealthBar : IGameEntity
    {
        public int DrawOrder => 100;

        private int HEALTHBAR_POS_X = (TrexRunnerGame.WINDOW_WIDTH/ 2) - (HEALTHBAR_SIZE / 2);
        private const int HEALTHBAR_POS_Y = 5;

        private const int HEALTHBAR_TEXTURE_POS_X = 75;
        private const int HEALTHBAR_TEXTURE_POS_Y = 70;

        private const int HEALTHBAR_SIZE = 100;
        
        private const int HEART_WIDTH = 35;
        private const int HEART_HEIGHT = 35;
        
        private float margin {
            get 
            {
                if (_numberOfHearts < 3)
                    return (HEALTHBAR_SIZE - (HEART_WIDTH * 3)) / 2;
                else
                    return (HEALTHBAR_SIZE - (HEART_WIDTH * _numberOfHearts)) / (_numberOfHearts - 1);
            }
        }

        private Trex _trex;
        
        private int _numberOfHearts => _trex.Health;

        private Vector2 Position => new Vector2(HEALTHBAR_POS_X, HEALTHBAR_POS_Y);

        private Sprite _heartSprite;
        


        public HealthBar(Texture2D spriteSheet, Trex trex)
        {
            _heartSprite = new Sprite(spriteSheet, HEALTHBAR_TEXTURE_POS_X, HEALTHBAR_TEXTURE_POS_Y, HEART_WIDTH, HEART_HEIGHT);
            _trex = trex;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawHearts(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            
        }

        private void DrawHearts(SpriteBatch spriteBatch)
        {
            float posX = HEALTHBAR_POS_X;

            for (int i = 0; i < _numberOfHearts; i++)
            {
                _heartSprite.Draw(spriteBatch, new Vector2(posX, HEALTHBAR_POS_Y));
                posX += HEART_WIDTH + margin;
            }
        }

    }
}