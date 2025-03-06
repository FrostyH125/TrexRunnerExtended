using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrexRunner.Extensions
{
    public static class Texture2DExt
    {
        public static Texture2D InvertColors(this Texture2D texture, Color? excludeColor = null)
        {
            if(texture is null)
                throw new ArgumentNullException(nameof(texture));
            
            Texture2D result = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);

            Color[] pixelData = new Color[texture.Width * texture.Height];

            texture.GetData(pixelData);

            //If exclude color has a value and the pixel in question has that color, then return the pixel color as it is, otherwise, return new color for the pixel
            Color[] invertedPixelData = pixelData.Select(p => excludeColor.HasValue && p == excludeColor ? p : new Color(255 - p.R, 255 - p.G, 255 - p.B, p.A)).ToArray();

            result.SetData(invertedPixelData);

            return result;
        }
    }
}