using System.Drawing;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TrexRunner.Entities
{
    public interface ICollidable
    {
         
        Rectangle CollisionBox { get; }

    }
}