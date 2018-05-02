using System;
using System.Collections.Generic;
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

        #region Events
        public event KeyEventHandler KeyPress;
        public event KeyEventHandler KeyUp;
        #endregion

        public MessageFilter()
        {
            // Initializing Dictionary of Pressed Keys with all of them not pressed (false)
            KeysDictionary = new Dictionary<Keys, Boolean>();
            foreach (var key in Enum.GetValues(typeof(Keys)).Cast<Keys>())
            {
                KeysDictionary[key] = false;
            }
        }

        public Dictionary<Keys, bool> KeysDictionary { get; private set; }

        public bool IsPressed(Keys key)
        {
            return KeysDictionary[key];
        }

        public bool PreFilterMessage(ref Message m)
        {
            var key = (Keys)m.WParam;
            Boolean keyPressed = false;
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
            if(keyPressed)
            {
                KeyPress?.Invoke(this, new KeyEventArgs(key));
            }
            else
            {
                KeyUp?.Invoke(this, new KeyEventArgs(key));
            }
            return false;
        }
    }
}
