using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Controls
{
    public class Label : Control
    {
        public Label(GameScreen Screen, string Text, Font Font, Color BackColor, Color ForeColor) : base(Screen, Text, Font)
        {
            this.BackColor = BackColor;
            this.ForeColor = ForeColor;
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(BackBrush, Bounds);
            g.DrawString(Text, Font, ForeBrush, Bounds, Format);
        }
    }
}
