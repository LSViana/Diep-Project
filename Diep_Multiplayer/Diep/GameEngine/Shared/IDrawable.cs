using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public interface IDrawable
    {
        RectangleF Bounds { get; set; }
        void Draw(Graphics g);
    }
}
