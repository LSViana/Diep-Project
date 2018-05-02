using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep.GameEngine.Shared
{
    public class MessageFilter : IMessageFilter
    {
        public const int WM_KEYDOWN = 0x0100;

        public const int WM_KEYUP = 0x0101;

        public const int WM_LBUTTONDOWN = 0x0201;

        public const int WM_LBUTTONUP = 0x0202;

        public const int WM_RBUTTONDOWN = 0x0204;
        
        public const int WM_RBUTTONUP = 0x0205;

        public const int WM_MOUSEMOVE = 0x0200;

        public MessageFilter()
        {
            // Initializing Dictionary of Pressed Keys with all of them not pressed (false)
            KeysDictionary = new Dictionary<Keys, Boolean>();
            foreach (var key in Enum.GetValues(typeof(Keys)).Cast<Keys>())
            {
                KeysDictionary[key] = false;
            }
        }

        public Point MousePosition { get; set; }

        public virtual Dictionary<Keys, bool> KeysDictionary { get; private set; }

        public virtual bool IsPressed(Keys key)
        {
            return KeysDictionary[key];
        }

        public virtual bool PreFilterMessage(ref Message m)
        {
            var key = (Keys)m.WParam;
            if (m.Msg == WM_KEYDOWN)
            {
                return KeysDictionary[key] = true;
            }
            else if (m.Msg == WM_KEYUP)
            {
                return !(KeysDictionary[key] = false);
            }
            else if (m.Msg == WM_LBUTTONDOWN)
            {
                key = Keys.LButton;
                return KeysDictionary[key] = true;
            }
            else if (m.Msg == WM_LBUTTONUP)
            {
                key = Keys.LButton;
                return !(KeysDictionary[key] = false);
            }
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                key = Keys.RButton;
                return KeysDictionary[key] = true;
            }
            else if (m.Msg == WM_RBUTTONUP)
            {
                key = Keys.RButton;
                return !(KeysDictionary[key] = false);
            }
            else if (m.Msg == WM_MOUSEMOVE)
            {
                var y = (int)((m.LParam.ToInt32() & 0xFFFF0000) >> 16);
                var x = (int)((m.LParam.ToInt32() & 0x0000FFFF));
                MousePosition = new Point(x, y);
                return true;
            }
            return false;
        }
    }
}
