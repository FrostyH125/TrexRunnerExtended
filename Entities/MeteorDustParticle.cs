
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using SharpDX.Direct3D9;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Sprite = TrexRunner.Graphics.Sprite;

namespace TrexRunner.Entities
{
    public class MeteorDustParticle : IGameEntity
    {
        private const int SMALL_DUST_SPRITE_X_CORD = 48;
        private const int SMALL_DUST_SPRITE_Y_CORD = 107;
        private const int SMALL_DUST_SPRITE_WIDTH = 5;
        private const int SMALL_DUST_SPRITE_HEIGHT = 5;

        private const int LARGE_DUST_SPRITE_X_CORD = 40;
        private const int LARGE_DUST_SPRITE_Y_CORD = 106;
        private const int LARGE_DUST_SPRITE_WIDTH = 8;
        private const int LARGE_DUST_SPRITE_HEIGHT = 8;

        private bool IsBig { get; set; }

        private Vector2 Position { get; set; }

        private int Speed { get; set; }

        private float LifeSpan { get; set; }

        private Vector2 TrajectoryOffset { get; set; }

        private float _EOLTimer { get; set; }

        private Random _random { get; set; }

        private Sprite _dustSprite { get; set; }

        private Texture2D _spriteSheet { get; set; }

        private Trex _trex;

        private EntityManager _entityManager;

        public int DrawOrder => 0;


        public MeteorDustParticle(Texture2D spriteSheet, EntityManager entityManager, Trex trex, Vector2 position)
        {
            _random = new Random();
            Speed = SetRandomSpeed();
            LifeSpan = SetRandomLifespan();
            TrajectoryOffset = SetRandomTrajectoryOffset();
            IsBig = SetParticleSize();
            _spriteSheet = spriteSheet;
            _trex = trex;
            Position = position;
            _entityManager = entityManager;


            _dustSprite = GenerateSprite();
        }

        public void Update(GameTime gameTime)
        {
           
            Vector2 targetPosition = new Vector2
                (
                _trex.Position.X + (Trex.TREX_DEFAULT_SPRITE_WIDTH / 2),
                TrexRunnerGame.TREX_START_POS_Y - Trex.TREX_DEFAULT_SPRITE_HEIGHT + (Trex.TREX_DEFAULT_SPRITE_HEIGHT / 2) 
                ) 
                + TrajectoryOffset;

            Vector2 direction = targetPosition - Position;
            direction.Normalize();
            Position -= direction * (float)gameTime.ElapsedGameTime.TotalSeconds * Speed ;

            _EOLTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_EOLTimer > LifeSpan)
            {
                _entityManager.RemoveEntity(this);
            } 
        
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _dustSprite.Draw(spriteBatch, Position);

        }

        private int SetRandomSpeed()
        {   
            int randomSpeed = _random.Next(25, 50);
            return randomSpeed;
        }

        private float SetRandomLifespan()
        {
            float randomLifespan = _random.NextFloat(0.5f, 2);
            return randomLifespan;
        }

        private Vector2 SetRandomTrajectoryOffset()
        {
            Vector2 randomTrajectoryOffset = new Vector2(_random.NextInt64(-20, 20), _random.NextInt64(-20, 20));
            return randomTrajectoryOffset;
        }

        private bool SetParticleSize()
        {
            int oneOrZero = _random.Next(0, 2);
            if (oneOrZero == 0)
                return false;
            if (oneOrZero == 1)
                return true;

            else throw new Exception("Meteor Particle Size Deciding Integer Not Within Specified Range, Check 'SetParticleSize()'");
        }

        private Sprite GenerateSprite()
        {
            Sprite sprite = null;

            if(!IsBig)
                sprite = new Sprite
                (
                    _spriteSheet,
                    SMALL_DUST_SPRITE_X_CORD,
                    SMALL_DUST_SPRITE_Y_CORD,
                    SMALL_DUST_SPRITE_WIDTH,
                    SMALL_DUST_SPRITE_HEIGHT
                );
            
            if(IsBig)
                sprite = new Sprite
                (
                    _spriteSheet,
                    LARGE_DUST_SPRITE_X_CORD,
                    LARGE_DUST_SPRITE_Y_CORD,
                    LARGE_DUST_SPRITE_WIDTH,
                    LARGE_DUST_SPRITE_HEIGHT
                );


            return sprite;
        }

    }
}