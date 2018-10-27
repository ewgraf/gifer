using System;
using System.Windows.Forms;

namespace gifer {
    public class MoveFormWithControlsHandler {
        private readonly Form _form;

        private int _x;
        private int _y;
        private bool _moving;

        public MoveFormWithControlsHandler(Form form, Control[] controls) {
            if (form == null) {
                throw new ArgumentNullException(nameof(form));
            }
            if (controls == null) {
                throw new ArgumentNullException(nameof(controls));
            }
            
            _form = form;

            foreach (Control c in controls) {
                c.MouseDown += (s, a) => OnMouseDown(a);
                c.MouseMove += (s, a) => OnMouseMove(a);
                c.MouseUp += (s, a) => OnMouseUp();
            }
        }

        public void OnMouseDown(MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) {
                return;
            }
            _moving = true;
            _x = e.X;
            _y = e.Y;
        }

        public void OnMouseMove(MouseEventArgs e) {
            if (!_moving) {
                return;
            }
            _form.Top = e.Y + _form.Top - _y;
            _form.Left = e.X + _form.Left - _x;
        }

        public void OnMouseUp() {
            _moving = false;
        }
    }
}
