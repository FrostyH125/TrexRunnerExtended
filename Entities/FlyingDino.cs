using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrexRunner.Graphics;

namespace TrexRunner.Entities
{
    public class FlyingDino : Obstacle
    {
        private const int TEXTURE_COORDS_X = 134;
        private const int TEXTURE_COORDS_Y = 0;

        private const int SPRITE_WIDTH = 46;
        private const int SPRITE_HEIGHT = 42;
        private const float ANIMATION_FRAME_LENGTH = 0.2f;

        private const int VERTICAL_COLLISION_INSET = 10;
        private const int HORIZONTAL_COLLISION_INSET = 6;

        private const float SPEED_PPS = 80f;

        private SpriteAnimation _animation;

        private Trex _trex;

        public override Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), SPRITE_WIDTH, SPRITE_HEIGHT);
                box.Inflate(-HORIZONTAL_COLLISION_INSET, -VERTICAL_COLLISION_INSET);
                return box;
            }
        }


        public FlyingDino(Trex trex, Vector2 position, Texture2D spriteSheet) : base(trex, position)
        {
            _trex = trex;

            Sprite spriteA = new Sprite(spriteSheet, TEXTURE_COORDS_X, TEXTURE_COORDS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
            Sprite spriteB = new Sprite(spriteSheet, TEXTURE_COORDS_X + SPRITE_WIDTH, TEXTURE_COORDS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
            
            _animation = new SpriteAnimation();
            _animation.AddFrame(spriteA, 0);
            _animation.AddFrame(spriteB, ANIMATION_FRAME_LENGTH);
            _animation.AddFrame(spriteA, ANIMATION_FRAME_LENGTH * 2);
            _animation.ShouldLoop = true;
            _animation.Play();

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _animation.Draw(spriteBatch, Position);
        }

        //stops updating the animation when the game is over
        public override void Update(GameTime gameTime)
        {
            //updates the base class (moves the flying dino first)
            base.Update(gameTime);

            if(_trex.IsAlive)
            {
                //makes the flying dino move faster than the speed of the trex
                //after the base class updates its position
                Position = new Vector2(Position.X - SPEED_PPS * (float)gameTime.ElapsedGameTime.TotalSeconds, Position.Y);
                _animation.Update(gameTime);
            }
        }

    }
}