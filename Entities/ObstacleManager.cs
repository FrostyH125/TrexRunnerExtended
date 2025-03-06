using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrexRunner.Entities
{
    public class ObstacleManager : IGameEntity
    {
        private static readonly int[] FLYING_DINO_Y_POSITIONS = new int[] { 90, 62, 24 };

        public int DrawOrder => 0;

        private const float MIN_SPAWN_DISTANCE = 10;

        private const int MIN_OBSTACLE_DISTANCE = 6;
        private const int MAX_OBSTACLE_DISTANCE = 28;

        private const int OBSTACLE_DISTANCE_SPEED_TOLERANCE = 5;
        private const int LARGE_CACTUS_POS_Y = 80;
        private const int SMALL_CACTUS_POS_Y = 94;

        private const int OBSTACLE_DRAW_ORDER = 12;
        private const int OBSTACLE_DESPAWN_POS_X = -200;

        private const int FLYING_DINO_SPAWN_SCORE_MIN = 150;

        private double _lastSpawnScore = -1;
        private double _currentTargetDistance;

        private readonly EntityManager _entityManager;
        private readonly Trex _trex;
        private readonly ScoreBoard _scoreBoard;

        private readonly Random _random;

        private Texture2D _spriteSheet;

        public bool IsEnabled { get; set; }

        //true if ObstacleManager.IsEnabled = true and _scoreBoard.Score is greated than or equal to
        //MIN_SPAWN_DISTANCE (meaning that the trex has passed or reached score 20)
        public bool CanSpawnObstacle => IsEnabled && _scoreBoard.Score >= MIN_SPAWN_DISTANCE;

        public ObstacleManager(EntityManager entityManager, Trex trex, ScoreBoard scoreBoard, Texture2D spriteSheet)
        {
            _entityManager = entityManager;
            _trex = trex;
            _scoreBoard = scoreBoard;
            _random = new Random();
            _spriteSheet = spriteSheet;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            if(!IsEnabled)
                return;

            //if it can spawn an obstacle and the last spawn score has not been set yet 
            //or current score minus the last time an object was spawned
            //is greater than or equal to the currenttargetdistance for spawning a new object
            if(CanSpawnObstacle && 
                (_lastSpawnScore <= 0 || (_scoreBoard.Score - _lastSpawnScore >= _currentTargetDistance)))
            {

                //multiplies 0.0 to 1.0 by the max minus min distance (result ranging from 0 to 40) and then adding the min distance,
                //final resulting number has a range between 10 and 50 (literally genius)
                _currentTargetDistance = _random.NextDouble()
                    * (MAX_OBSTACLE_DISTANCE - MIN_OBSTACLE_DISTANCE) + MIN_OBSTACLE_DISTANCE;

                //increases the target distance by a portion of the speed the trex is going as to spread
                //out the obstacles more (if trex speed is max speed, the division will return 1 and be multiplied
                //by the speed tolerance, and will add 5 to _currentTargetDistance)
                _currentTargetDistance += (_trex.Speed - Trex.START_SPEED) / (Trex.MAX_SPEED - Trex.START_SPEED) * OBSTACLE_DISTANCE_SPEED_TOLERANCE;

                _lastSpawnScore = _scoreBoard.Score;
                SpawnRandomObstacle();

            }

            foreach (Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
            {

                if(obstacle.Position.X < OBSTACLE_DESPAWN_POS_X)
                    _entityManager.RemoveEntity(obstacle);

            }

        }

        private void SpawnRandomObstacle()
        {
            
            Obstacle obstacle = null;

            int cactusGroupSpawnRate = 75;
            int flyingDinoSpawnRate = _scoreBoard.Score >= FLYING_DINO_SPAWN_SCORE_MIN ? 25 : 0;

            //produces a number between 0 and the sum of these two spawnrates (plus 1 so each number has an equal chance)
            int rng = _random.Next(0, cactusGroupSpawnRate + flyingDinoSpawnRate + 1);

            if(rng <= cactusGroupSpawnRate)
            {
                //produces a random number between 0 and 2 (the +1 is so that every number has a fair chance to be chosen as it
                //trims the decimal off and doesnt round) and casts it to an instance of CactusGroup.GroupSize
                CactusGroup.GroupSize randomGroupSize = (CactusGroup.GroupSize)_random.Next((int)CactusGroup.GroupSize.Small, (int)CactusGroup.GroupSize.Large + 1);

                //generates a double between 0 and 1, if it is larger than 5, than isLarge = true
                bool isLarge = _random.NextDouble() > 0.5;

                //if isLarge = true then posY = 85, otherwise, posY = 95
                float posY = isLarge ? LARGE_CACTUS_POS_Y : SMALL_CACTUS_POS_Y;

                obstacle = new CactusGroup(_spriteSheet, isLarge, randomGroupSize, _trex, new Vector2(TrexRunnerGame.WINDOW_WIDTH, posY));
            }

            //chooses randomly a number between 0 and the length of the array for possible y positions [3]
            //spawns a flying dino at the y position of that entry of the array
            else
            {
                int verticalPosIndex = _random.Next(0, FLYING_DINO_Y_POSITIONS.Length);
                float posY = FLYING_DINO_Y_POSITIONS[verticalPosIndex];
                obstacle = new FlyingDino(_trex, new Vector2(TrexRunnerGame.WINDOW_WIDTH, posY), _spriteSheet);
            }

            obstacle.DrawOrder = OBSTACLE_DRAW_ORDER;

            _entityManager.AddEntity(obstacle);
            
        }

        public void Reset()
        {

            foreach(Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
            {

                _entityManager.RemoveEntity(obstacle);

            }
            _currentTargetDistance = 0;
            _lastSpawnScore = -1;
        }
    }
}