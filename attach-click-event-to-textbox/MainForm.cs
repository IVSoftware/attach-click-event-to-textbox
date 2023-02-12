using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace attach_click_event_to_textbox
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            IterateControlTree((control) =>
            {
                // Attach click handlers to the textboxes
                // already added in the Forms designer.
                if (control is TextBoxBase) control.Click += onAnyClickTextBox;
            });

            IterateControlTree((control) =>
            {
                control.ControlAdded += (sender, e) =>
                { 
                    // Get notified when any control collection is changed.
                    if(e.Control is TextBoxBase textbox)
                    {
                        textbox.Click += onAnyClickTextBox;
                    }
                };
            });

            buttonNew.Click += onClickNew;
        }

        void IterateControlTree(Action<Control> action, Control control = null)
        {
            if (control == null)
            {
                control = this;
            }
            action(control);
            foreach (Control child in control.Controls)
            {
                IterateControlTree(action, child);
            }
        }

        private void onAnyClickTextBox(object sender, EventArgs e)
        {
            if(sender is Control control)
            {
                textBox1.Text = $"Clicked: {control.Name}";
            }
        }

        // FOR TESTING PURPOSES
        int _id = 1;
        private void onClickNew(object sender, EventArgs e)
        {
            flowLayoutPanel.Controls.Add(new TextBox
            {
                Name = $"dynamicTextBox{_id}",
                PlaceholderText = $"TextBox{_id}",
            });
            _id++;
        }
    }
}
