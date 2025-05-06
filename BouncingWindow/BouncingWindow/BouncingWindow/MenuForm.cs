using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BouncingWindow
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Size = new Size(140, 80);
            this.BackColor = Color.White;
            this.TopMost = true;

            InitMenuButtons();
        }

        private void InitMenuButtons()
        {
            Button inventoryButton = new Button
            {
                Text = "Open Inventory",
                Dock = DockStyle.Top,
                Height = 40
            };

            inventoryButton.Click += (s, e) =>
            {
                MessageBox.Show("Inventory clicked!");
                this.Hide(); // Close the menu after click
            };

            this.Controls.Add(inventoryButton);
        }
    }
}
