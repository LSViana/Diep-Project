using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public struct Vector2D
    {
        public static readonly Vector2D Empty = new Vector2D(0, 0);

        public Vector2D(Single X, Single Y)
        {
            if (X >= -1 && X <= 1)
                this.X = X;
            else
                this.X = 0;
            if (Y >= -1 && Y <= 1)
                this.Y = Y;
            else
                this.Y = 0;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public override string ToString()
        {
            return $"[X: {X}, Y: {Y}]";
        }
    }
}
