using System;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace TrexRunner.Entities
{
    public class ScoreBoard : IGameEntity
    {

        private const int TEXTURE_COORDS_NUMBER_X = 655;
        private const int TEXTURE_COORDS_NUMBER_Y = 0;

        private const int TEXTURE_COORDS_NUMBER_WIDTH = 10;
        private const int TEXTURE_COORDS_NUMBER_HEIGHT = 13;

        private const byte NUMBER_DIGITS_TO_DRAW = 5;

        private const int TEXTURE_COORDS_HI_X = 755;
        private const int TEXTURE_COORDS_HI_Y = 0;
        private const int TEXTURE_COORDS_HI_WIDTH = 20;
        private const int TEXTURE_COORDS_HI_HEIGHT = 13;

        private const int HI_TEXT_MARGIN = 28;

        private const int SCORE_MARGIN = 70;
        private const float SCORE_INCREMENT_MULTIPLIER = 0.025f; //0.025f
        private Texture2D _texture;
        private Trex _trex;
        
        private bool _isPlayingFlashAnimation;
        public bool _isActive;
        private float _flashAnimationTime;

        private SoundEffect _scoreSFX;
        private double _score;
        private const float FLASH_ANIMATION_FRAME_LENGTH = 0.333f;
        private const int FLASH_ANIMATION_FLASH_COUNT = 4;

        private const int MAX_SCORE = 99_999;

        //sets score to higher value of the two values in Math.Max()
        //sets score to lower value of the two values in Math.Min()
        public double Score {
             get => _score; 
             set => _score = Math.Max(0, Math.Min(value, MAX_SCORE)); }

        public int DisplayScore => (int)Math.Floor(Score);

        public int HighScore { get; set; }

        public bool HasHighScore => HighScore > 0;

        public int DrawOrder => 100;

        public Vector2 Position { get; set; }

        public ScoreBoard(Texture2D texture, Vector2 position, Trex trex, SoundEffect scoreSFX)
        {
            _trex = trex;
            _texture = texture;
            Position = position;
            _scoreSFX = scoreSFX;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_isActive)
                return;

            if(HasHighScore)
            {
                //draws the HI symbol
                spriteBatch.Draw(_texture, new Vector2(Position.X - HI_TEXT_MARGIN, Position.Y), new Rectangle(TEXTURE_COORDS_HI_X, TEXTURE_COORDS_HI_Y, TEXTURE_COORDS_HI_WIDTH, TEXTURE_COORDS_HI_HEIGHT), Color.White);
                //draws the actual highscore
                
                    DrawScore(spriteBatch, HighScore, Position.X);

            }

            //MATH: the division being cast to an integer basically returns the current frame of the flash animation
            //say the time is 2.1 seconds into the flash animation, dividing that by the frame length (say if it was .2 seconds), 
            //that would return 10.5 (cast to integer returns 10), which if thinking logically, would be the number
            //of frame you should be on if each frame lasts 0.2 seconds, and youve been running for 2.1 seconds
            //
            //The modulo operation, division by 2, basically just checks if the result of the previous operation
            //(the current frame) is even or odd (if its the 4th frame, the division by 2 would not have a remainder,
            //making it even, if the result was 3, the result would have a remainder, making it odd)
            //
            //The score will be drawn either if the flash animation is NOT playing OR if the animation IS playing
            //AND the frame is odd
            if(!_isPlayingFlashAnimation || ((int)(_flashAnimationTime / FLASH_ANIMATION_FRAME_LENGTH) % 2 != 0))
            {
                //While the flash animation is not playing, the score is just the displayscore,
                //while the flash animation is playing, the score shown on screen is the display score
                //rounded down to the nearest 100
                int score = !_isPlayingFlashAnimation ? DisplayScore : (DisplayScore / 100 * 100);
                DrawScore(spriteBatch, score, Position.X + SCORE_MARGIN);
            }

        }

        private void DrawScore(SpriteBatch spriteBatch, int score, float startPosX)
        {
            //gets the individual digits of the DisplayScore, forming an array
            int[] scoreDigits = SplitDigits(score);

            //gets current X component of position
            float posX = startPosX;

            //iterates over each digit in the array
            foreach (int digit in scoreDigits)
            {
                //gets the area within the texture that should be rendered for each digit
                Rectangle textureCoords = GetDigitTextureBounds(digit);

                //defines where it should be rendered on the screen
                Vector2 screenPos = new Vector2(posX, Position.Y);

                //draws it to the screen using these variables (it now knows which part of the spritesheet to draw AND where to draw it)
                spriteBatch.Draw(_texture, screenPos, textureCoords, Color.White);

                //adds width of the number to the x position as to draw the next number to the right of the last number
                posX += TEXTURE_COORDS_NUMBER_WIDTH;
            }
        }


        public void Update(GameTime gameTime)
        {
            //first defines this
            int oldScore = DisplayScore;

            //then updates the score (making Display Score != oldScore)
            Score += _trex.Speed * SCORE_INCREMENT_MULTIPLIER * gameTime.ElapsedGameTime.TotalSeconds;

            //Since int division always crops off the remainder, this will only be true and run if the 
            //DisplayScore passes a number that can be evenly divided by 100 (every 100 points)
            if(!_isPlayingFlashAnimation && (DisplayScore / 100 != oldScore / 100))
            {
                _isPlayingFlashAnimation = true;
                _flashAnimationTime = 0;
                //TODO: in own game, set parameters to Play(1,1,1) it sounds nice
                _scoreSFX.Play(0.3f, 0, 0);
            }

            if (_isPlayingFlashAnimation)
            {
                _flashAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //returns the time for frame length times the flash count, which is how long the animation should last,
                //giving 4 frames no matter what numbers we plug in here
                if(_flashAnimationTime >= FLASH_ANIMATION_FRAME_LENGTH * FLASH_ANIMATION_FLASH_COUNT * 2)
                {
                    _isPlayingFlashAnimation = false;
                }
            }
        }

        private int[] SplitDigits(int input)
        {

            //casts the input of numbers to a string, ands pads the left of the string with 0, up to 5 digits
            string inputStr = input.ToString().PadLeft(NUMBER_DIGITS_TO_DRAW, '0');

            //defines an integer array with the length of the input string
            int[] result = new int[inputStr.Length];

            //iterates over each element of the array 
            for (int i = 0; i < result.Length; i++)
            {
                //casts the char value of each element of the string to a double by getting the numeric value of the "char" and then casting it to an integer
                result[i] = (int)char.GetNumericValue(inputStr[i]);
            }
            //returns the resulting array of numbers when it is done
            return result;

        }

        private Rectangle GetDigitTextureBounds(int digit)
        {

            if (digit < 0 || digit > 9)
                throw new ArgumentOutOfRangeException("digit", "The value of digit must be between 0 and 9");

            int posX = TEXTURE_COORDS_NUMBER_X + digit * TEXTURE_COORDS_NUMBER_WIDTH;
            int posY = TEXTURE_COORDS_NUMBER_Y;

            return new Rectangle(posX, posY, TEXTURE_COORDS_NUMBER_WIDTH, TEXTURE_COORDS_NUMBER_HEIGHT);

        }

    }
}