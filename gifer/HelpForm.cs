using System;
using System.Windows.Forms;

namespace gifer
{
    public partial class HelpForm : Form
    {
        public bool ShowHelpAtStartup { get; private set; }
        public bool OpenInStandalone  { get; private set; }

        public HelpForm(bool show, bool standalone)
        {
            InitializeComponent();

            this.checkBoxShowHelpAtStartup.Checked = show;
            this.checkBoxOpenInStandalone.Checked = standalone;
        }

        private void buttonContinue_Click(object s, EventArgs e) => this.Close();

        private void HelpForm_FormClosing(object s, FormClosingEventArgs e)
        {
            ShowHelpAtStartup = this.checkBoxShowHelpAtStartup.Checked;
            OpenInStandalone  = this.checkBoxOpenInStandalone .Checked;
        }

        private void HelpForm_KeyDown(object s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {

        }
    }
}
