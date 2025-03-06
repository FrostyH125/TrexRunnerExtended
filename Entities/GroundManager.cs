using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Sprite = TrexRunner.Graphics.Sprite;

namespace TrexRunner.Entities
{
    public partial class GroundManager : IGameEntity
    {

        private const float GROUND_TILE_POS_Y =  119;

        private const int SPRITE_WIDTH = 600;
        private const int SPRITE_HEIGHT = 14;

        private const int SPRITE_POS_X =2;
        private const int SPRITE_POS_Y =54;

        private Texture2D _spriteSheet;
        private readonly EntityManager _entityManager;

        private readonly List<GroundTile> _groundTiles;

        private Sprite _regularSprite;
        private Sprite _bumpySprite;

        private Trex _trex;

        private Random _random;


        public int DrawOrder { get; set; }

        public GroundManager(Texture2D spritesheet, EntityManager entityManager, Trex trex)
        {
            _spriteSheet = spritesheet;
            _groundTiles = new List<GroundTile>();
            _entityManager = entityManager;

            //Creates instances of reusable sprites for each ground tile in the constructor
            _regularSprite = new Sprite(spritesheet, SPRITE_POS_X, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
            _bumpySprite = new Sprite(spritesheet, SPRITE_POS_X + SPRITE_WIDTH, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            _trex = trex;
            _random = new Random();

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            
            if(_groundTiles.Any())
            {
                //finds the highest x position in the list of ground tiles
                float maxPosX = _groundTiles.Max(g => g.PositionX);

                //if the ground tile moves even a part of itself offscreen, 
                //invoke method, where maxpos x gets added to the sprite width
                //to spawn a tile at the point where there is no ground
                if(maxPosX < 0)
                    SpawnTile(maxPosX);
                
            }
            
            List<GroundTile> tilesToRemove = new List<GroundTile>();

            foreach(GroundTile gt in _groundTiles)
            {

                gt.PositionX -= _trex.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //if the ground tile is off screen, remove it
                if(gt.PositionX < -SPRITE_WIDTH)
                {

                    _entityManager.RemoveEntity(gt);
                    tilesToRemove.Add(gt);

                }

            }

            foreach(GroundTile gt in tilesToRemove)
                _groundTiles.Remove(gt);

        }

        public void Initialize()
        {
            _groundTiles.Clear();

            //removes each GroundTile type from the entity manager
            foreach(GroundTile gt in _entityManager.GetEntitiesOfType<GroundTile>())
            {
                _entityManager.RemoveEntity(gt);
            }

            //spawns a default tile automatically to begin the cycle, and adds it to the list
            //as well as adds it as an entity as to automatically draw and update it
            GroundTile groundTile = CreateRegularTile(0);
            _groundTiles.Add(groundTile);

            _entityManager.AddEntity(groundTile);

        }

        private GroundTile CreateRegularTile(float positionX)
        {

            GroundTile groundTile = new GroundTile(positionX, GROUND_TILE_POS_Y, _regularSprite);

            return groundTile;

        }

        private GroundTile CreateBumpyTile(float positionX)
        {

            GroundTile groundTile = new GroundTile(positionX, GROUND_TILE_POS_Y, _bumpySprite);

            return groundTile;

        }

        private void SpawnTile(float maxPosX)
        {

            //variable which returns a random number between 0.1 and 1.0
            double randomNumber = _random.NextDouble();

            float posX = maxPosX + SPRITE_WIDTH;
            
            GroundTile groundTile;

            //decides based on random number which tile to create
            if(randomNumber > 0.5)
                groundTile = CreateBumpyTile(posX);
            else
                groundTile = CreateRegularTile(posX);

            //adds the created tile to the tiles list and adds it as an entity
            _entityManager.AddEntity(groundTile);
            _groundTiles.Add(groundTile);

        }

    }
}