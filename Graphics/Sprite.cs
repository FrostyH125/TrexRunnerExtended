using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TrexRunner.Graphics
{
    public class Sprite
    {

      public Texture2D Texture { get; set; }

      public int X { get; set; }
      public int Y { get; set; }
    
      public int Width { get; set; }
      public int Height { get; set; }

      public Color TintColor { get; set; } = Color.White;

      public Sprite(Texture2D texture, int x, int y, int width, int height)
      {
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
      }

      public void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.Vector2 position)
      {

        spriteBatch.Draw(Texture, position, new Rectangle(X, Y, Width, Height), TintColor);
          
      }

      //use this override when you want to add a rotation parameter to your sprite method
      //https://stackoverflow.com/questions/20518218/how-to-make-an-object-move-towards-another-object-in-c-sharp-xna
      //The link may or may not be enough to enable you to add this feature
      //since youll be looking at this at the same time youre trying to get something to go towards another object (like the trex)
      //try
      //position = (target.position - position) / position * speed
      //if it doesnt work just play around with the idea
      public void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.Vector2 position, float rotation)
      {
        spriteBatch.Draw(Texture, position, new Rectangle(X, Y, Width, Height), TintColor, rotation, new Microsoft.Xna.Framework.Vector2(0, 0), 1f, SpriteEffects.None, 1f);
      }
    }
}