using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public interface ITouchable
    {
        Single Power { get; set; }
        Single Impact { get; set; }
        Single Health { get; set; }
        Boolean VerifyCollision(ITouchable other);
        void Collide(ITouchable other);
        RectangleF CollisionBounds { get; }
        Boolean Active { get; set; }
    }
}
