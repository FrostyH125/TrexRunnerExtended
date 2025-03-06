using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrexRunner.Entities
{
    public class SkyManager : IGameEntity, IDayNightCycle
    {
        private const float EPSILON = 0.01f;

        private const int CLOUD_DRAW_ORDER = -1;
        private const int STAR_DRAW_ORDER = -3;
        private const int MOON_DRAW_ORDER = -2;

        //CLOUD VARIABLES
        private const int CLOUD_MIN_POS_Y = 20;
        private const int CLOUD_MAX_POS_Y = 70;
        private const int CLOUD_MIN_DISTANCE = 150;
        private const int CLOUD_MAX_DISTANCE = 400;

        //STAR VARIABLES
        private const int STAR_MIN_POS_Y = 10;
        private const int STAR_MAX_POS_Y = 60;
        private const int STAR_MIN_DISTANCE = 120;
        private const int STAR_MAX_DISTANCE = 380;

        //MOON VARIABLES
        private const int MOON_POS_Y = 20;

        //NIGHT VARIABLES
        private const int NIGHT_TIME_SCORE = 700;
        private const int NIGHT_TIME_DURATION_SCORE = 250;
        private const float TRANSITION_DURATION = 2f;

        private float _normalizedScreenColor = 1f;
        private int _previousScore;
        private int _nightTimeStartScore;
        private bool _isTransitioningToNight = false;
        private bool _isTransitioningToDay = false;

        private int _targetCloudDistance;
        private int _targetStarDistance;

        public int DrawOrder => int.MaxValue;

        public int NightCount { get; private set; }

        public bool IsNight => _normalizedScreenColor < 0.5f;

        //returns progress of overlay transparency
        //for example, if normalized screen color was 0.3f, 0.25f - 0.2f (the difference between 0.5 and 0.3) is 0.05
        //0.05 / 0.25 = 0.2, 1/5th of the way to 1, 1 being the midpoint at normalized screen color 0.5f
        //clamp makes sure that it doesn't go below 0 or above 1
        private float OverlayVisibility => MathHelper.Clamp((0.25f - MathHelper.Distance(0.5f, _normalizedScreenColor)) / 0.25f, 0, 1);
        private Texture2D _overlay;

        private readonly EntityManager _entityManager;
        private readonly ScoreBoard _scoreBoard;
        private Texture2D _spriteSheet;
        private Texture2D _invertedSpriteSheet;
        private readonly Trex _trex;
        private Moon _moon;

        private Random _random;

        public Color[] _textureData;
        public Color[] _invertedTextureData;

        public Color ClearColor => new Color(_normalizedScreenColor, _normalizedScreenColor, _normalizedScreenColor); 

        public SkyManager(Trex trex, Texture2D spriteSheet, Texture2D invertedSpriteSheet, EntityManager entityManager, ScoreBoard scoreBoard)
        {
            _entityManager = entityManager;
            _scoreBoard = scoreBoard;
            _random = new Random();
            _spriteSheet = spriteSheet;
            _invertedSpriteSheet = invertedSpriteSheet;
            _trex = trex;

            _textureData = new Color[_spriteSheet.Width * _spriteSheet.Height];
            _invertedTextureData = new Color[_spriteSheet.Width * _spriteSheet.Height];
            
            _spriteSheet.GetData(_textureData);
            _invertedSpriteSheet.GetData(_invertedTextureData);

            _overlay = new Texture2D(spriteSheet.GraphicsDevice, 1, 1);
            Color[] overlayData = new[] { Color.Gray };
            _overlay.SetData(overlayData);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //draws the overlay (with the adjusted transparency) ONLY IF the visibility of the overlay is greater than the minimum amount)
            if(OverlayVisibility > EPSILON)
            spriteBatch.Draw(_overlay, new Rectangle(0, 0, TrexRunnerGame.WINDOW_WIDTH, TrexRunnerGame.WINDOW_HEIGHT), Color.White * OverlayVisibility);

        }

        public void Update(GameTime gameTime)
        {
            if(_moon == null)
            {
                _moon = new Moon(this, _spriteSheet, _trex , new Vector2(TrexRunnerGame.WINDOW_WIDTH, MOON_POS_Y));
                _moon.DrawOrder = MOON_DRAW_ORDER;
                _entityManager.AddEntity(_moon);
            }

            HandleCloudSpawning();
            HandleStarSpawning();
            

            foreach(SkyObject skyObject in _entityManager.GetEntitiesOfType<SkyObject>().Where(s => s.Position.X < -100))
            {
                //resets the moon to the right side of the screen when it reaches the left side
                if (skyObject is Moon moon)
                {
                    moon.Position = new Vector2(TrexRunnerGame.WINDOW_WIDTH, MOON_POS_Y);
                }
                //removes all other entities when reaching the left side of the screen
                else
                    _entityManager.RemoveEntity(skyObject);
            }

            //transitions to night time when score passes a multiple of 700
            if(_previousScore != 0 && _previousScore < _scoreBoard.DisplayScore && _previousScore / NIGHT_TIME_SCORE != _scoreBoard.DisplayScore / NIGHT_TIME_SCORE)
            {
                TransitionToNightTime();
            }

            //transitions back to day time after 250 score is passed while it is night time
            if(IsNight &&  _scoreBoard.DisplayScore - _nightTimeStartScore >= NIGHT_TIME_DURATION_SCORE)
            {
                TransitionToDayTime();
            }
            
            //transitions to day time if the score is less than 700 and it is night still or if its transitioning to night
            if(_scoreBoard.DisplayScore < NIGHT_TIME_SCORE && (IsNight || _isTransitioningToNight))
            {
                TransitionToDayTime();
            }

            UpdateTransition(gameTime);

            _previousScore = _scoreBoard.DisplayScore;
            
        }


        private void UpdateTransition(GameTime gameTime)
        {
            if(_isTransitioningToNight)
            {
                _normalizedScreenColor -= (float)gameTime.ElapsedGameTime.TotalSeconds / TRANSITION_DURATION;
                
                if(_normalizedScreenColor < 0)
                    _normalizedScreenColor = 0;

                if(IsNight)
                {
                    InvertTextures();
                }
            }

            else if(_isTransitioningToDay)
            {
                _normalizedScreenColor += (float)gameTime.ElapsedGameTime.TotalSeconds / TRANSITION_DURATION;

                if(_normalizedScreenColor > 1)
                    _normalizedScreenColor = 1;

                if(!IsNight)
                {
                    InvertTextures();
                }
                
            }

        }

        private void InvertTextures()
        {
            if(IsNight)
            {
                _spriteSheet.SetData(_invertedTextureData);
            }

            if(!IsNight)
            {
                _spriteSheet.SetData(_textureData);
            }
        }

        private bool TransitionToNightTime()
        {
            if(IsNight || _isTransitioningToNight)
                return false;

            _nightTimeStartScore = _scoreBoard.DisplayScore;
            _isTransitioningToDay = false;
            _isTransitioningToNight = true;
            _normalizedScreenColor = 1f;
            NightCount++;

            return true;
        }

        private bool TransitionToDayTime()
        {
            if(!IsNight || _isTransitioningToDay)
                return false;

            _isTransitioningToDay = true;
            _isTransitioningToNight = false;
            _normalizedScreenColor = 0f;

            return true;
        }

        private void HandleCloudSpawning()
        {
            //gives a collection of all objects in the cloud class in the entity manager
            IEnumerable<Cloud> clouds = _entityManager.GetEntitiesOfType<Cloud>();

            //activates if the furthest cloud from the right of the screen surpasses the target cloud distance for spawning the next cloud 
            if(clouds.Count() <= 0 || (TrexRunnerGame.WINDOW_WIDTH - clouds.Max(cloud => cloud.Position.X)) >= _targetCloudDistance)
            {
                _targetCloudDistance = _random.Next(CLOUD_MIN_DISTANCE, CLOUD_MAX_DISTANCE + 1);
                int posY = _random.Next(CLOUD_MIN_POS_Y, CLOUD_MAX_POS_Y + 1);

                Cloud cloud = new Cloud(_spriteSheet, _trex, new Vector2(TrexRunnerGame.WINDOW_WIDTH, posY));
                cloud.DrawOrder = CLOUD_DRAW_ORDER;

                _entityManager.AddEntity(cloud);

            }

        }

        private void HandleStarSpawning()
        {
            IEnumerable<Star> stars = _entityManager.GetEntitiesOfType<Star>();

            if(stars.Count() <= 0 || (TrexRunnerGame.WINDOW_WIDTH - stars.Max(star => star.Position.X)) >= _targetStarDistance)
            {
                _targetStarDistance = _random.Next(STAR_MIN_DISTANCE, STAR_MAX_DISTANCE + 1);
                int posY = _random.Next(STAR_MIN_POS_Y, STAR_MAX_POS_Y +1);

                Star star = new Star(this, _spriteSheet, _trex, new Vector2(TrexRunnerGame.WINDOW_WIDTH, posY));
                star.DrawOrder = STAR_DRAW_ORDER;

                _entityManager.AddEntity(star);

            }
        }

    }

}