using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.MediaFoundation;
using TrexRunner.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Sprite = TrexRunner.Graphics.Sprite;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TrexRunner.Entities
{

    public class Meteor : Obstacle
    {
        //FOR TESTING IM USING THE SMALL CACTUS SPRITE
        private const int METEOR_SPRITE_COORD_X = 111;
        private const int METEOR_SPRITE_COORD_Y = 75;
        private const int METEOR_SPRITE_WIDTH = 32;
        private const int METEOR_SPRITE_HEIGHT = 20;

        private const int TREX_START_POS_Y = TrexRunnerGame.TREX_START_POS_Y;
        private const float MIN_DUST_SPAWN_TIME = 0.04f;
        private const float MAX_DUST_SPAWN_TIME = 0.12f;

        public Sprite Sprite { get; set; }

        public Vector2 Distance = new();

        public float Rotation { get; set; }

        public Trex Trex { get; set; }

        private Random _random { get; set; }

        private bool IsSpawningDust { get; set; } = false;

        private float nextDustCountdown { get; set; }

        private float elapsedTime { get; set; }

        private EntityManager _entityManager { get; set; }

        private Texture2D _spriteSheet { get; set; }


        public override Rectangle CollisionBox {
            get
            {
                Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), Sprite.Width, Sprite.Height);
                box.Inflate(0, 0);
                return box;
            }
        }


        public Meteor(Texture2D spriteSheet, EntityManager entityManager, Trex trex, Vector2 position) : base(trex, position)
        {   
            _entityManager = entityManager;
            _random = new Random();
            _spriteSheet = spriteSheet;
            Trex = trex;
            Position = position;
            Sprite = new Sprite(spriteSheet, METEOR_SPRITE_COORD_X, METEOR_SPRITE_COORD_Y, METEOR_SPRITE_WIDTH, METEOR_SPRITE_HEIGHT);
            SpawnRandomDustParticle();

            Vector2 distance = new();
            distance.X = Trex.Position.X - Position.X;
            distance.Y = TREX_START_POS_Y - Position.Y;
            Rotation = (float)Math.Atan2(distance.Y, distance.X);

        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position, Rotation);
        }

        public override void Update(GameTime gameTime)
        {   
            Vector2 targetPosition = new Vector2(Trex.Position.X + (Trex.TREX_DEFAULT_SPRITE_WIDTH / 2), TrexRunnerGame.TREX_START_POS_Y - Trex.TREX_DEFAULT_SPRITE_HEIGHT + (Trex.TREX_DEFAULT_SPRITE_HEIGHT / 2) );
            Vector2 dir = targetPosition - Position;
            dir.Normalize();
            Position += dir * (Trex.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds / 4);
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(elapsedTime > nextDustCountdown)
            {
                SpawnRandomDustParticle();
            }


        }
        public void SpawnRandomDustParticle()
        {
            //Makes the dust trail thicker by spawning with a variance in Y coord. Might make this more sophisticated in the future
            MeteorDustParticle dust = new MeteorDustParticle(_spriteSheet, _entityManager, Trex, Position);
            MeteorDustParticle topdust = new MeteorDustParticle(_spriteSheet, _entityManager, Trex, Position + new Vector2(0, -4));
            MeteorDustParticle bottomdust = new MeteorDustParticle(_spriteSheet, _entityManager, Trex, Position + new Vector2(0, 4));

            _entityManager.AddEntity(topdust);
            _entityManager.AddEntity(dust);
            _entityManager.AddEntity(bottomdust);

            nextDustCountdown = _random.NextFloat(MIN_DUST_SPAWN_TIME, MAX_DUST_SPAWN_TIME);
            elapsedTime = 0;
        }
    }   

}


//TODO
//Implement Collision on the Meteors
//Make the meteors spawn at random locations
//Make the trex able to fire at the meteors
//make the meteor delete itself when it collides with a fireball
