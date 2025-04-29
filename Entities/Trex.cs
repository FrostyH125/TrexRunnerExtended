using System;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata;
using System.Windows.Forms.VisualStyles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TrexRunner.Graphics;

namespace TrexRunner.Entities
{
    public class Trex : IGameEntity, ICollidable
    {
        //Jumping
        private const float Gravity = 1600f;
        private const float JUMP_START_VELOCITY = -480f;
        private const float CANCEL_JUMP_VELOCITY = -100f;
        private const float MIN_JUMP_HEIGHT = 40f;
        private float _verticalVelocity;
        private float _startPosY;
        private float _dropVelocity;
        private const float DROP_VELOCITY = 600f;

        //Idle Sprite texture location
        private const int TREX_IDLE_BACKGROUND_SPRITE_POS_X = 40;
        private const int TREX_IDLE_BACKGROUND_SPRITE_POS_Y = 0;

        //default sprite texture location & dimensions
        public const int TREX_DEFAULT_SPRITE_POS_X = 848;
        public const int TREX_DEFAULT_SPRITE_POS_Y = 0;
        public const int TREX_DEFAULT_SPRITE_WIDTH = 44;
        public const int TREX_DEFAULT_SPRITE_HEIGHT = 52;

        //blink animation values
        private const float BLINK_ANIMATION_RANDOM_MIN = 2f;
        private const float BLINK_ANIMATION_RANDOM_MAX = 10f;
        private const float BLINK_ANIMATION_EYE_CLOSE_TIME = 0.5f;
        private Random _random;

        //running animation
        private const int TREX_RUNNING_SPRITE_ONE_POS_X = TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_WIDTH * 2;
        private const int TREX_RUNNING_SPRITE_ONE_POS_Y = 0;
        private const float RUN_ANIMATION_FRAME_LENGTH = 0.1f;

        //hurt animation
        private const float TREX_HURT_SPRITE_FLASH_LENGTH = 0.15f;
        public float _hurtAnimationElapsedTime { get; private set; }
        public float _hurtAnimationLength = 0.9f;
        private const int HURT_BLANK_SPRITE_POS_X = -1;
        private const int HURT_BLANK_SPRITE_POS_Y = -1;
        private const int HURT_BLANK_SPRITE_WIDTH = 1;
        private const int HURT_BLANK_SPRITE_HEIGHT = 1;
        
        public const float START_SPEED = 280f;
        public const float MAX_SPEED = 900f;

        private const float ACCELERATION_PPS_PER_SECOND = 3f;

        //ducking animation
        private const int TREX_DUCKING_SPRITE_WIDTH = 59;
        private const int TREX_DUCKING_SPRITE_POS_X = TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_WIDTH * 6;
        private const int TREX_DUCKING_SPRITE_POS_Y = 0;

        private const int TREX_DEAD_SPRITE_POS_X = 1068;
        private const int TREX_DEAD_SPRITE_POS_Y = 0;

        private const int TREX_DEAD_CROUCHING_SPRITE_POS_X = 1112;
        private const int TREX_DEAD_CROUCHING_SPRITE_POS_Y = 69;

        private const int COLLISION_BOX_INSET = 3;
        private const int DUCK_COLLISION_REDUCTION = 20;

        //trex sprites for state changes
        private Sprite _idleBackgroundSprite;
        private Sprite _idleSprite;
        private Sprite _idleBlinkSprite;
        private Sprite _deadSprite;
        private Sprite _deadCrouchingSprite;
        private Sprite _hurtBlankSprite;

        //sounds
        private SoundEffect _jumpSound;
        private SoundEffect _hitSound;

        //trex animations
        private SpriteAnimation _blinkAnimation;
        private SpriteAnimation _runAnimation;
        private SpriteAnimation _duckAnimation;
        private SpriteAnimation _hurtAnimation;


        public event EventHandler JumpComplete;
        public event EventHandler Died;

        public bool IsHurt {get; set;}


        //stores current state of trex
        public TrexState State  { get; private set; }

        //ORIGINAL
        public TrexState PreviousState { get; private set; }

        //stores current position of trex
        public Vector2 Position { get; set; }

        public TrexRunnerGame Game;

        public int DrawOrder { get; set; }

        public bool IsAlive { get; private set; }

        public float Speed { get; private set; }

        public int Health = 3;

        public Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle
                (
                    (int)Math.Round(Position.X),
                    (int)Math.Round(Position.Y),
                    TREX_DEFAULT_SPRITE_WIDTH,
                    TREX_DEFAULT_SPRITE_HEIGHT
                );
                box.Inflate(-COLLISION_BOX_INSET, -COLLISION_BOX_INSET); 

                if(State == TrexState.Ducking)
                {
                    box.Y += DUCK_COLLISION_REDUCTION;
                    box.Height -=DUCK_COLLISION_REDUCTION;
                }               
                return box;
            }
        }


        public Trex(Texture2D spriteSheet, Vector2 position, SoundEffect jumpSound, SoundEffect hitSound, TrexRunnerGame game)
        {
            
            Position = position;
            Game = game;
            _idleBackgroundSprite = new Sprite(spriteSheet, TREX_IDLE_BACKGROUND_SPRITE_POS_X, TREX_IDLE_BACKGROUND_SPRITE_POS_Y, TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            State = TrexState.Idle;

            _jumpSound = jumpSound;
            _hitSound = hitSound;

            _random = new Random();


            _idleSprite = new Sprite(spriteSheet, TREX_DEFAULT_SPRITE_POS_X, TREX_DEFAULT_SPRITE_POS_Y, TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            _idleBlinkSprite = new Sprite(spriteSheet, TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_POS_Y, TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            _deadSprite = new Sprite(spriteSheet, TREX_DEAD_SPRITE_POS_X, TREX_DEAD_SPRITE_POS_Y, TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            _deadCrouchingSprite = new Sprite(spriteSheet, TREX_DEAD_CROUCHING_SPRITE_POS_X, TREX_DEAD_CROUCHING_SPRITE_POS_Y, TREX_DUCKING_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT);
            _hurtBlankSprite = new Sprite(spriteSheet, HURT_BLANK_SPRITE_POS_X, HURT_BLANK_SPRITE_POS_Y, HURT_BLANK_SPRITE_WIDTH, HURT_BLANK_SPRITE_HEIGHT);

            _blinkAnimation = new SpriteAnimation();

            CreateBlinkAnimation();
            _blinkAnimation.Play();

            _startPosY = position.Y;

            _runAnimation = new SpriteAnimation();
            _runAnimation.AddFrame(new Sprite(spriteSheet, TREX_RUNNING_SPRITE_ONE_POS_X, TREX_RUNNING_SPRITE_ONE_POS_Y, TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT), 0);
            _runAnimation.AddFrame(new Sprite(spriteSheet, TREX_RUNNING_SPRITE_ONE_POS_X + TREX_DEFAULT_SPRITE_WIDTH, TREX_RUNNING_SPRITE_ONE_POS_Y, TREX_DEFAULT_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT), RUN_ANIMATION_FRAME_LENGTH);
            _runAnimation.AddFrame(_runAnimation[0].Sprite, RUN_ANIMATION_FRAME_LENGTH * 2);
            _runAnimation.Play();

            _duckAnimation = new SpriteAnimation();
            _duckAnimation.AddFrame(new Sprite(spriteSheet, TREX_DUCKING_SPRITE_POS_X, TREX_DUCKING_SPRITE_POS_Y, TREX_DUCKING_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT), 0);
            _duckAnimation.AddFrame(new Sprite(spriteSheet, TREX_DUCKING_SPRITE_POS_X + TREX_DUCKING_SPRITE_WIDTH, TREX_DUCKING_SPRITE_POS_Y, TREX_DUCKING_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_HEIGHT), RUN_ANIMATION_FRAME_LENGTH);
            _duckAnimation.AddFrame(_duckAnimation[0].Sprite, RUN_ANIMATION_FRAME_LENGTH * 2);
            _duckAnimation.Play();

            _hurtAnimation = SpriteAnimation.CreateSimpleAlternatingAnimation(spriteSheet, _deadSprite, _hurtBlankSprite, TREX_HURT_SPRITE_FLASH_LENGTH, 6);
            _hurtAnimation.Play();

            IsAlive = true;
        }

        public void Initialize()
        {
            
            Speed = START_SPEED;
            State = TrexState.Running;
            IsAlive = true;
            Position = new Vector2(Position.X, _startPosY);

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {   
            if (IsAlive)
            {
                if (!IsHurt)
                {
                    if (State == TrexState.Idle)
                    {
                        _idleBackgroundSprite.Draw(spriteBatch, Position);
                        _blinkAnimation.Draw(spriteBatch, Position);
                    }

                    else if (State == TrexState.Jumping || State == TrexState.Falling)
                    {

                        _idleSprite.Draw(spriteBatch, Position);

                    }
                    else if (State == TrexState.Running)
                    {

                        _runAnimation.Draw(spriteBatch, Position);

                    }
                    else if (State == TrexState.Ducking)
                    {

                        _duckAnimation.Draw(spriteBatch, Position);

                    }
                }
                if (IsHurt)
                {

                    _hurtAnimation.Draw(spriteBatch, Position);           
                
                }

                PreviousState = State;

            }

            else if (!IsAlive && PreviousState != TrexState.Ducking)
            {
                _deadSprite.Draw(spriteBatch, Position);
            }

            else if (!IsAlive && PreviousState == TrexState.Ducking)
            {
                _deadCrouchingSprite.Draw(spriteBatch, Position);
            }                     

            
        }

        //this method is called in the main game loop
        //it calls other update functions in other classes that function in their own way
        public void Update(GameTime gameTime)
        {
            if(State == TrexState.Idle && !IsHurt)
            {
                //if the blink animation is not playing while idle, it will remake a new one with a new random number and play it
                if(!_blinkAnimation.IsPlaying)
                {
                    CreateBlinkAnimation();
                    _blinkAnimation.Play();
                }

                _blinkAnimation.Update(gameTime);

            }

            //checks if the trex is jumping or falling, if it is, applies the vertical velocity
            else if(State == TrexState.Jumping || State == TrexState.Falling)
            {
                
                //_verticalVelocity is usually zero unless BeginJump() or ContinueJump() is called
                //adds the drop velocity (usually zero unless Drop() is called)
                Position = new Vector2(Position.X, Position.Y + _verticalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds + _dropVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

                //applies gravity to vertical velocity
                _verticalVelocity += Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //sets state or falling if _verticalVelocity is greater than or equal to zero (meaning its going downward)
                if (_verticalVelocity >= 0)
                    State = TrexState.Falling;

                //sets position to start position and velocity to zero if the trex goes lower than the start position
                if(Position.Y >=_startPosY)
                {

                    Position = new Vector2(Position.X, _startPosY);
                    _verticalVelocity = 0;
                    State = TrexState.Running;

                    OnJumpComplete();

                }
                    

            }

            //continues the timer for the run animation
            else if (State == TrexState.Running && !IsHurt)
            {

                _runAnimation.Update(gameTime);

            }

            //continues the timer for the duck animatiion
            else if (State == TrexState.Ducking && !IsHurt)
            {

                _duckAnimation.Update(gameTime);

            }


            if (IsHurt)
            {
                _hurtAnimationElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _hurtAnimation.Update(gameTime);

                if(_hurtAnimationElapsedTime >= _hurtAnimationLength)
                {
                    IsHurt = false;
                    _hurtAnimationElapsedTime = 0;
                }

            }

            if(State != TrexState.Idle)
                Speed += ACCELERATION_PPS_PER_SECOND * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(Speed > MAX_SPEED)
                Speed = MAX_SPEED;

            if(Health <= 0)
            {
                Die();
            }


            //resets the drop velocity every frame, only to be changed when calling Drop()
            _dropVelocity = 0;
        }


        private void CreateBlinkAnimation()
        {
            
            //clears the previous blink animation frames (this is to reassign frames as the animation involves random timestamps)
            _blinkAnimation.Clear();
            _blinkAnimation.ShouldLoop = false;
            
            //Assigns a random number between 2 and 10 to _blinkTimeStamp
            double _blinkTimeStamp = BLINK_ANIMATION_RANDOM_MIN + _random.NextDouble() * (BLINK_ANIMATION_RANDOM_MAX - BLINK_ANIMATION_RANDOM_MIN);

            //Adds SpriteAnimationFrames to the SpriteAnimation created in this function
            _blinkAnimation.AddFrame(_idleSprite, 0);
            _blinkAnimation.AddFrame(_idleBlinkSprite, (float)_blinkTimeStamp);

            //Acts as a dummy frame to reset the animation and let us know when it ends
            _blinkAnimation.AddFrame(_idleSprite, (float)_blinkTimeStamp + BLINK_ANIMATION_EYE_CLOSE_TIME);


        }

        public bool BeginJump()
        {

            if(State == TrexState.Jumping || State == TrexState.Falling)
                return false;

            _jumpSound.Play();

            State = TrexState.Jumping;
            
            _verticalVelocity = JUMP_START_VELOCITY;
            
            return true;

        }

        public bool CancelJump()
        {
            
            //cannot cancel jump if not jumping or if the min jump height is more than the height you're at
            if(State != TrexState.Jumping || (_startPosY - Position.Y) < MIN_JUMP_HEIGHT)
                return false;

            //smooths out the transition for the jump cancel
            _verticalVelocity = _verticalVelocity < CANCEL_JUMP_VELOCITY ? CANCEL_JUMP_VELOCITY : 0;

            return true;
        }

        public bool Duck()
        {
            if(State == TrexState.Jumping || State == TrexState.Falling)
                return false;

            State = TrexState.Ducking;

            return true;


        }

        public bool GetUp()
        {

            if(State != TrexState.Ducking)
                return false;

            State = TrexState.Running;
            return true;

        }

        public bool Drop()
        {

            if (State != TrexState.Falling && State != TrexState.Jumping)
                return false;

            //sets the drop velocity so the trex falls faster
            State = TrexState.Falling;
            _dropVelocity = DROP_VELOCITY;

            return true;
        }

        //Raises the event, which the "follower" in the game class will be notified of and can use to call its own method
        protected virtual void OnJumpComplete()
        {

            EventHandler handler = JumpComplete;
            handler?.Invoke(this, EventArgs.Empty);

        }

        protected virtual void OnDied()
        {
            EventHandler handler = Died;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public bool Die()
        {
            if (!IsAlive)
                return false;

            State = TrexState.Idle;
            Speed = 0;
            IsAlive = false;

            OnDied();

            return true;
        }

        public void LoseHealth()
        {
            Health -= 1;
            IsHurt = true;
            _hitSound.Play();
            Game.shakeScreen = true;
        }

        public void GainHealth()
        {
            Health += 1;
        }
    }
}
