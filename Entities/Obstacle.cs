using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrexRunner.Entities
{

    public abstract class Obstacle : IGameEntity, ICollidable
    {
        private Trex _trex;

        public abstract Rectangle CollisionBox { get; }

        public int DrawOrder { get; set; }

        public Vector2 Position { get; protected set; }

        protected Obstacle(Trex trex, Vector2 position)
        {
            _trex = trex;
            Position = position;
        }


        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);        


        public virtual void Update(GameTime gameTime)
        {
            float posX = Position.X - _trex.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position = new Vector2(posX, Position.Y);

            CheckCollisions();
        }

        private void CheckCollisions() //if adding platforms, make this virtual as to override it
        {
            Rectangle obstacleCollisionBox = CollisionBox;
            Rectangle trexCollisionBox = _trex.CollisionBox;

            if(obstacleCollisionBox.Intersects(trexCollisionBox))
            {
                _trex.Die();
            }
        }

    }
}