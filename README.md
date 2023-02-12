As I understand it, you have three requirements:

- When the project starts, attach a click event to all `TextBox` instances already created in the `Form` designer.
- When a new text box is created (programmatically or by user interaction) attach the click event to the new textbox.
- Implement this functionality _without_ making a custom class. 

This answer shows one way to meet these three objectives.

***
**Utilility**

First, make a utility that can iterate all of the controls in the Form, but also all the controls of its child controls.

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

***
**Attach existing**

Using this utility, initialize any textboxes added in design mode to route to the click handler.

[![design-time-click][2]][2]

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
            .
            .
            .
        }

        // Show the name of the clicked button.
        private void onAnyClickTextBox(object sender, EventArgs e)
        {
            if(sender is Control control)
            {
                textBox1.Text = $"Clicked: {control.Name}";
            }
        }
    }

***
**Attach new**

Iterate a second time to attach the `ControlAdded` event to every control. This way, new `TextBox` instances can be detected in order to attach the `Click` event.

[![runtime click][1]][1]

    public MainForm()
    {
        .
        .
        .
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
    }

***
**Testing**

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

  [1]: https://i.stack.imgur.com/VdLeU.png
  [2]: https://i.stack.imgur.com/LMNdC.png