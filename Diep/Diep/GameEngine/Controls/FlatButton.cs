using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Controls
{
    public class FlatButton : Control
    {
        private Pen forePen;

        public FlatButton(GameScreen Screen, String Text, Font Font, Color BackColor, Color ForeColor, Boolean Border) : base(Screen, Text)
        {
            this.BackColor = BackColor;
            this.ForeColor = ForeColor;
            this.Border = Border;
            this.Font = Font;
            // Standard Property Values
            forePen = new Pen(ForeColor);
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                forePen = new Pen(value);
            }
        }

        public override bool ReadOnly { get => true; set => value = true; }

        public override Color ForeColor { get => base.ForeColor; set => base.ForeColor = value; }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            // Base Drawing
            g.FillRectangle(BackBrush, Bounds);
            // Mouse Down Effect
            if (Pressed)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb((Int32)(BackColor.R * .7f), (Int32)(BackColor.G * .7f), (Int32)(BackColor.B * .7f))), Bounds);
            }
            // Hover Effect
            else if (Hovered)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb((Int32)(BackColor.R * .8f), (Int32)(BackColor.G * .8f), (Int32)(BackColor.B * .8f))), Bounds);
            }
            // Border Drawing
            if (Border)
            {
                g.DrawRectangle(forePen, Rectangle.Round(Bounds));
            }
            // Text Drawing
            g.DrawString(Text, Font, ForeBrush, Bounds, Format);
        }

        public bool Border { get; }
    }
}
