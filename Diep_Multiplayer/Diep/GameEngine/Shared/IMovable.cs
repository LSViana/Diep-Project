using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public interface IMovable : IDrawable, IGameObject
    {
        void Move(Vector2D vector);
        Single Angle { get; set; }
        Vector2D MovementVector { get; set; }
        Single Speed { get; set; }
        RectangleF DrawingBounds { get; }
    }
}
