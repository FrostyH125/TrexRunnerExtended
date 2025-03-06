using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrexRunner.Graphics
{
    public class SpriteAnimation
    {
        private List<SpriteAnimationFrame> _frames = new List<SpriteAnimationFrame>();

        //returns the frame at that index in the list
        public SpriteAnimationFrame this[int index]
        {
            get
            {
                return GetFrame(index);
            }
        }

        public int FrameCount => _frames.Count;

        //sorts all the frames by timestamps less than or equal to playback progress
        //orders them by timestamp
        //returns the final one
        public SpriteAnimationFrame CurrentFrame
        {
            get
            {

                return _frames
                .Where(f => f.TimeStamp <= PlaybackProgress)
                .OrderBy(f => f.TimeStamp)
                .LastOrDefault();

            }
        }

        //returns the frame with the largest timestamp
        public float Duration
        {
            get
            {

                if (!_frames.Any())
                    return 0;

                return _frames.Max(f => f.TimeStamp);

            }
        }

        public bool IsPlaying { get; private set; }

        public float PlaybackProgress { get; private set; }

        public bool ShouldLoop { get; set; } = true;

        //adds a SpriteAnimationFrame (frame) to the _frames list for this SpriteAnimation
        //assigns to the frame the sprite included with the invoking of this method, and the timestamp
        
        public void AddFrame(Sprite sprite, float timeStamp)
        {
            SpriteAnimationFrame frame = new SpriteAnimationFrame(sprite, timeStamp);

            _frames.Add(frame);
        }
        

        //adds to playback progress the elapsed seconds
        //if the playback progress is greated than the duration variable
        //if ShouldLoop = true it subtracts the duration from the playback progress
        //effectively setting it back to 0 to reset
        //else it calls the stop method and sets IsPlaying to false
        public void Update(GameTime gameTime)
        {

            if(IsPlaying)
            {

                PlaybackProgress += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(PlaybackProgress > Duration)
                    {
                        if(ShouldLoop)
                            PlaybackProgress -= Duration; 
                        else
                            Stop();
                    }
                    //could you replace this to -= 0 instead?
                
            }

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {

            SpriteAnimationFrame frame = CurrentFrame;

            if(frame != null)
                frame.Sprite.Draw(spriteBatch, position);
            

        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            PlaybackProgress = 0;
        }

        public SpriteAnimationFrame GetFrame(int index)
        {
            if (index < 0 || index >= _frames.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "The frame with index" + index + "does not exist in this animation.");
            
            return _frames[index];
        }

        public void Clear()
        {

            Stop();
            _frames.Clear();

        }

        //Automatically makes an animation based on these parameters, not needing to create a bunch of individual sprites within the class
        //Can be invoked from the class itself, does not require an object
        public static SpriteAnimation CreateSimpleAnimation(Texture2D texture, Point startPos, int width, int height, Point offset, int frameCount, float framelength)
        {
            if(texture == null)
                throw new ArgumentNullException(nameof(texture));

            //defines a sprite animation
            SpriteAnimation anim = new SpriteAnimation();

            //
            for(int i = 0; i < frameCount; i++)
            {
                //creates a sprite with the parameters, adjusting accordingly as iterated over
                Sprite sprite = new Sprite(texture, startPos.X + i * offset.X, startPos.Y + i * offset.Y, width, height);
                //adds frame to the sprite animation, adjusting framelength as iterated over
                anim.AddFrame(sprite, framelength * i);

                //adds dummy frame at the end
                if(i == frameCount - 1)
                    anim.AddFrame(sprite, framelength * (i + 1));
            }

            return anim;

        }

    }

}