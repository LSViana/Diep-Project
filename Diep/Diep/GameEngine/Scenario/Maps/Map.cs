using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Maps
{
    public class Map : IGameObject
    {
        private SolidBrush backBrush;
        private Pen backPen;

        public SolidBrush BackBrush => backBrush;
        public Pen BackPen => backPen;

        public Map(GameScreen Screen, Color BackColor, Color GridColor, Int32 SquareSize)
        {
            this.Screen = Screen;
            backBrush = new SolidBrush(BackColor);
            backPen = new Pen(GridColor);
            this.BackColor = BackColor;
            this.GridColor = GridColor;
            this.SquareSize = SquareSize;
            // Standard Property Values
            MapOutLimit = 100f;
        }

        public Single MapOutLimit { get; set; }
        public RectangleF Bounds { get; set; }
        public GameScreen Screen { get; }
        public Color BackColor { get { return backBrush.Color; } set { backBrush = new SolidBrush(value); } }
        public Color GridColor { get { return backPen.Color; } set { backPen = new Pen(value); } }
        public int SquareSize { get; }

        public void Draw(Graphics g)
        {
            var drawingBounds = Bounds;
            var drawArea = Screen.DrawArea;
            drawingBounds.Offset(-Screen.CameraPoint.X, -Screen.CameraPoint.Y);
            drawingBounds.Intersect(Screen.ScreenBounds);

            // Drawing Background
            g.FillRectangle(BackBrush, drawingBounds);
            // Drawing Grid
            //Int32 offsetX = (Int32)(drawingBounds.X > 0 ? drawingBounds.X : -Screen.CameraPoint.X % SquareSize + SquareSize),
            //    offsetY = (Int32)(drawingBounds.Y > 0 ? drawingBounds.Y : -Screen.CameraPoint.Y % SquareSize + SquareSize);
            //Int32 offsetX = (Int32)(drawingBounds.X > 0 ? drawingBounds.X : -Screen.CameraPoint.X % SquareSize + SquareSize + drawingBounds.X),
            //    offsetY = (Int32)(drawingBounds.Y > 0 ? drawingBounds.Y : -Screen.CameraPoint.Y % SquareSize + SquareSize + drawingBounds.Y);
            ////
            //var horizontalGrid = (Int32)drawingBounds.Width / SquareSize;
            //var verticalGrid = (Int32)drawingBounds.Height / SquareSize;
            //for (int x = 0; x < horizontalGrid; x++)
            //{
            //    g.DrawLine(BackPen, new Point(offsetX, (Int32)drawingBounds.Y), new Point(offsetX, (Int32)(drawingBounds.Y + drawingBounds.Height)));
            //    offsetX += SquareSize;
            //}
            //for (int y = 0; y < verticalGrid; y++)
            //{
            //    g.DrawLine(BackPen, new Point((Int32)drawingBounds.X, offsetY), new Point((Int32)(drawingBounds.X + drawingBounds.Width), offsetY));
            //    offsetY += SquareSize;
            //}
            //Tests Purpose
            //g.DrawString(drawingBounds.Size.ToString(), new Font("Segoe UI", 14), new SolidBrush(Color.Black), drawingBounds.Location);
        }

        public void Step(IEnumerable<object> data)
        {
            //
        }
    }
}
