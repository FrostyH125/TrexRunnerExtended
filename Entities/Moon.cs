using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrexRunner.Graphics;

namespace TrexRunner.Entities
{

    public class Moon : SkyObject
    {
        private const int RIGHTMOST_SPRITE_COORDS_X = 624;
        private const int RIGHTMOST_SPRITE_COORDS_Y = 2;
        private const int SPRITE_WIDTH = 20;
        private const int SPRITE_HEIGHT = 40;

        private const int SPRITE_COUNT = 7;

        private Sprite _sprite;

        private readonly IDayNightCycle _dayNightCycle;

        public override float Speed => _trex.Speed * 0.1f;

        public Moon(IDayNightCycle dayNightCycle, Texture2D spriteSheet, Trex trex, Vector2 position) : base(trex, position)
        {
            _dayNightCycle = dayNightCycle;
            _sprite = new Sprite(spriteSheet, RIGHTMOST_SPRITE_COORDS_X, RIGHTMOST_SPRITE_COORDS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            UpdateSprite();

            if(_dayNightCycle.IsNight)
                _sprite.Draw(spriteBatch, Position);
        }

        private void UpdateSprite()
        {
            //returns index of sprite to use based on night count
            //if night count was 3, 3 % 7, 7 goes into 3 zero times, with a remainder of 3
            //if night count was say, 37, 37 % 7, 7 goes into 37 5 times, with a remainder of 2
            int spriteIndex = _dayNightCycle.NightCount % SPRITE_COUNT;

            int spriteWidth = SPRITE_WIDTH;
            int spriteHeight = SPRITE_HEIGHT;

            //draws the big moon sprite at this index
            if(spriteIndex == 3)
                spriteWidth *= 2;
            //increment the sprite index as the big moon sprite takes up 2 sprite indexes (so after, sprite index will be behind by 1)
            if(spriteIndex >= 3)
                spriteIndex++;

            _sprite.Height = spriteHeight;
            _sprite.Width = spriteWidth;

            //sets the X coord to move based on the sprite index in increments of the sprite width
            _sprite.X = RIGHTMOST_SPRITE_COORDS_X - spriteIndex * SPRITE_WIDTH;
            _sprite.Y = RIGHTMOST_SPRITE_COORDS_Y;

        }

    }
}