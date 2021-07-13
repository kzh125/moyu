using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace moyu
{
    public class InactiveMonitor
    {
        public event EventHandler OnInactive;
        private int _inactiveSeconds;
        private Timer _timer;
        private MouseHook _mouse = new MouseHook();
        private KeyboardHook _keyboard = new KeyboardHook();
        private DateTime _activityTime = DateTime.Now;
        private bool _isActive;

        public InactiveMonitor(int seconds)
        {
            _inactiveSeconds = seconds;

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(this.timer_Tick);
            _timer.Enabled = false;

            _mouse.OnMouseActivity += new MouseEventHandler(mouse_OnMouseActivity);
            _mouse.Start();

            _keyboard.KeyDownEvent += new KeyEventHandler(keyboard_OnKeyDown);
            _keyboard.Start();
        }

        public void Start()
        {
            _timer.Enabled = true;
        }

        void mouse_OnMouseActivity(object sender, MouseEventArgs e)
        {
            _activityTime = DateTime.Now;
            _isActive = true;
        }

        void keyboard_OnKeyDown(object sender, KeyEventArgs e)
        {
            _activityTime = DateTime.Now;
            _isActive = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!_isActive)
            {
                return;
            }

            if ((DateTime.Now - _activityTime).TotalSeconds > _inactiveSeconds)
            {
                _isActive = false;
                if (OnInactive != null)
                {
                    OnInactive(this, null);
                }
            }
        }
    }
}
