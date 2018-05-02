using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public static class Extensions
    {
        public static Random Random = new Random();

        /// <summary>
        /// Method used to get the angle between tho PointF structures
        /// </summary>
        /// <param name="this">The first PointF structure</param>
        /// <param name="other">The second PointF structure</param>
        /// <param name="until2PI">If true, the result is inside the range [0, 2 * Math.PI], otherwise, it goest from [-Math.PI, Math.PI]</param>
        /// <returns>The angle, in radians, between the two PointF structures</returns>
        public static Single GetAngle(this PointF @this, PointF other, Boolean until2PI = true)
        {
            var offsetY = other.Y - @this.Y;
            var offsetX = other.X - @this.X;
            var hyp = Math.Sqrt(Math.Pow(offsetX, 2) + Math.Pow(offsetY, 2));
            if(until2PI)
                return (Single)(offsetY < 0 ? Math.PI * 2 - Math.Acos(offsetX / hyp) : Math.Acos(offsetX / hyp));
            else
                return (Single)(offsetY < 0 ? -Math.Acos(offsetX / hyp) : Math.Acos(offsetX / hyp));
        }

        /// <summary>
        /// Returns the PointF structure corresponding to the center of the RectangleF
        /// </summary>
        /// <param name="this">The RectangleF to be used at calculus</param>
        /// <returns>The PointF structure that is at center of RectangleF</returns>
        public static PointF GetCenter(this RectangleF @this)
        {
            return new PointF(@this.X + @this.Width / 2, @this.Y + @this.Height / 2);
        }

        /// <summary>
        /// Converts an angle in radians into an angle in degrees
        /// </summary>
        /// <param name="this">The angle in radians</param>
        /// <returns>The angle in degrees</returns>
        public static Single ToDegree(this Single @this)
        {
            return (Single)(180 / Math.PI * @this);
        }

        public static PointF Move(this PointF @this, Single distance, Vector2D vector)
        {
            var offsetX = distance * vector.X;
            var offsetY = distance * vector.Y;
            @this.X += offsetX;
            @this.Y += offsetY;
            return @this;
        }

        public static Vector2D GetVectorFromAngle(this Single angle)
        {
            return new Vector2D((Single)Math.Cos(angle), (Single)Math.Sin(angle));
        }

        public static Single GetAngleFromVector(this Vector2D vector)
        {
            var initial = PointF.Empty;
            var next = new PointF(vector.X, vector.Y);
            return initial.GetAngle(next);
        }

        public static PointF GetSquareBorder(this PointF @this, Single ShapeAngle, Single Angle, Single SquareDimension)
        {
            var distance = (Single)(Math.Abs(Math.Cos(Angle * 2)) * (-(Math.Sqrt(2) / 2 - .5)) + Math.Sqrt(2) / 2) * SquareDimension;
            return @this.Move(distance, GetVectorFromAngle(Angle));
        }

        public static Single GetDistance(this PointF @this, PointF destiny)
        {
            return (Single)(Math.Sqrt(Math.Pow(@this.X - destiny.X, 2) + Math.Pow(@this.Y - destiny.Y, 2)));
        }
    }
}
