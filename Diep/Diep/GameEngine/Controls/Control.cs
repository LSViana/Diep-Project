using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep.GameEngine.Controls
{
    public class Control : IGameObject
    {
        public event EventHandler MouseDown;
        public event EventHandler MouseUp;

        private const int letterCooldown = 100;
        protected GameScreen screen;
        private Brush backBrush;
        private Brush foreBrush;
        private Color backColor;
        private Color foreColor;
        private Font font;

        public Control(GameScreen Screen, String Text) : this(Screen, Text, new Font("Segoe UI", 9))
        {
            // Simplest Constructor
        }

        public Control(GameScreen Screen, String Text, Font Font)
        {
            this.Screen = Screen;
            this.Text = Text;
            // Standard Property Values
            font = Font;
            Format = new StringFormat();
        }

        public StringFormat Format { get; protected set; }

        public virtual Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                backBrush = new SolidBrush(value);
            }
        }

        public virtual Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                foreBrush = new SolidBrush(value);
            }
        }

        public Boolean MouseDownOnEnter { get; set; }

        public Font Font { get { return font; } protected set { font = value; } }

        protected Brush BackBrush => backBrush;

        protected Brush ForeBrush => foreBrush;

        public RectangleF Bounds { get; set; }

        public PointF Location
        {
            get { return Bounds.Location; }
            set { Bounds = new RectangleF(value, Bounds.Size); }
        }

        public SizeF Size
        {
            get { return Bounds.Size; }
            set { Bounds = new RectangleF(Bounds.Location, value); }
        }

        public Boolean Hovered { get; protected set; }

        public String Text { get; set; }

        public GameScreen Screen
        {
            get { return screen; }
            set
            {
                screen = value;
                if (!Screen.Controls.Contains(this))
                    Screen.Controls.Add(this);
            }
        }

        public bool Pressed { get; private set; }

        public DateTime LastKeyPressed { get; private set; }

        public virtual bool ReadOnly { get; set; }

        public int MaxLength { get; set; }

        public virtual void Draw(Graphics g)
        {
            // Standard Implementation
        }

        public virtual void Step(IEnumerable<object> data)
        {
            foreach (var item in data)
            {
                if (item is Point mouseLocation)
                {
                    var mouseRect = new RectangleF(mouseLocation, new Size(1, 1));
                    if (mouseRect.IntersectsWith(Bounds))
                    {
                        Hovered = true;
                    }
                    else
                    {
                        if (Hovered)
                            Hovered = false;
                    }
                }

                // Verifying Mouse
                if (Screen.MessageFilter.IsPressed(Keys.LButton))
                {
                    if (Hovered)
                    {
                        MouseDown?.Invoke(this, EventArgs.Empty);
                        Pressed = true;
                    }
                }
                if (ReadOnly)
                    return;
                // Verifying Keyboard
                if (Screen.MessageFilter.IsPressed(Keys.Back))
                {
                    if (Text.Length == 0 || (LastKeyPressed > DateTime.MinValue && (DateTime.Now - LastKeyPressed).TotalMilliseconds < letterCooldown))
                        return;
                    Text = Text.Substring(0, Text.Length - 1);
                    LastKeyPressed = DateTime.Now;
                }
                else if (Screen.MessageFilter.IsPressed(Keys.Enter))
                {
                    if (MouseDownOnEnter)
                        MouseDown?.Invoke(this, EventArgs.Empty);
                }
                else if (Text.Length > MaxLength)
                    return;
                else
                {
                    var pressedTextKeys = Screen.MessageFilter.KeysDictionary
                        .ToArray()
                        .Where(a => Char.IsLetterOrDigit((char)a.Key) || Char.IsWhiteSpace((char)a.Key))
                        .Select(a => a.Key);
                    if (pressedTextKeys.Any())
                    {
                        foreach (var key in pressedTextKeys)
                        {
                            if (LastKeyPressed > DateTime.MinValue && (DateTime.Now - LastKeyPressed).TotalMilliseconds < letterCooldown)
                                return;
                            var upper = (System.Windows.Forms.Control.ModifierKeys & (Keys.Shift | Keys.CapsLock)) != 0;
                            var letter = (char)Encoding.ASCII.GetBytes(new char[] { (char)key })[0];
                            if (!upper)
                                letter = Char.ToLower(letter);
                            Text += letter;
                            LastKeyPressed = DateTime.Now;
                        }
                    }
                }
                if (!data.Any(a => a is Keys && (Keys)a == Keys.LButton))
                {
                    if (Pressed)
                    {
                        MouseUp?.Invoke(this, EventArgs.Empty);
                        Pressed = false;
                    }
                }
            }
        }
    }
}
