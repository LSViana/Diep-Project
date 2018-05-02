using System;
using System.Collections.Generic;
using System.Drawing;

namespace Diep.GameEngine.Shared
{
    public class GraphicsSupplier
    {
        public GraphicsSupplier()
        {
            var teamColors = Enum.GetValues(typeof(TeamColor));
            Brushes = new SolidBrush[teamColors.Length * 256];
            Pens = new Pen[256];
            // Pens
            for (byte i = 0; i <= Byte.MaxValue; i++)
            {
                Pens[i] = new Pen(Color.FromArgb((Byte)(i * GameController.PenAlpha / 255f), Color.Black), GameController.PenWidth);
                //
                if (i == Byte.MaxValue)
                    break;
            }
            // Brushes
            foreach (TeamColor teamColor in teamColors)
            {
                var color = GetColor(teamColor);
                for (byte i = 0; i <= Byte.MaxValue; i++)
                {
                    Brushes[(Int32)teamColor * 256 + i] = new SolidBrush(Color.FromArgb(i, color));
                    //
                    if (i == Byte.MaxValue)
                        break;
                }
            }
        }

        private Color GetColor(TeamColor teamColor)
        {
            switch (teamColor)
            {
                case TeamColor.Red:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0x0000FF));
                case TeamColor.DeepSkyBlue:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0xFFBF00));
                case TeamColor.Mustard:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0x69E8FF));
                case TeamColor.OrangeRed:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0x0045FF));
                case TeamColor.Purple:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0xE22B8A));
                case TeamColor.White:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0xFFFFFF));
                case TeamColor.HotPink:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0xB469FF));
                case TeamColor.IndianRed:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0x7776FC));
                case TeamColor.LightGreen:
                    return Color.FromArgb(255, ColorTranslator.FromWin32(0x80FF80));
                case TeamColor.DimGray:
                    return Color.FromArgb(255, Color.DimGray);
                case TeamColor.DarkGray:
                    return Color.DarkGray;
            }
            return Color.Empty;
        }

        public SolidBrush[] Brushes { get; set; }

        public Pen[] Pens { get; set; }

        public SolidBrush GetSolidBrush(TeamColor color, Byte alpha)
        {
            return Brushes[(Int32)color * 256 + alpha];
        }

        public Pen GetPen(Byte alpha)
        {
            return Pens[alpha];
        }
    }
}
