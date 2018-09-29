using System.Collections.Generic;
using System.Windows.Forms;

namespace gifer {
    public static class ControlExtensions {
        public static Control[] ToArray(this Control.ControlCollection controls) {
            var list = new List<Control>();
            foreach (Control c in controls) {
                list.Add(c);
            }
            return list.ToArray();
        }
    }
}
