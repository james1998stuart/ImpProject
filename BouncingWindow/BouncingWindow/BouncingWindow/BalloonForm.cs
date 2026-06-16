using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using BouncingWindow;

namespace BouncingWindow
{
    public partial class BalloonForm : Form
    {
        private Button menuButton;
        private MenuForm menuForm;

        private PictureBox balloon;
        private System.Windows.Forms.Timer balloonTimer;
        private System.Windows.Forms.Timer spawnTimer;

        private float driftAngle = 0f;
        private Random random = new Random();

        private int balloonSpeed;
        private int balloonBaseY;
        private int balloonDriftAmount;

        public BalloonForm()//Constructor
        {
            InitializeComponent();

            

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0); // Top of screen
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, 150);

            //this.BackColor = Color.LightBlue; //testing remove in future

            InitBalloon();
            InitMenuButton();


            this.Resize += (s, e) => PositionMenuButton();
        }

        private async void OnBalloonPopped()
        {
            var drops = await DropFetcher.GetDropsAsync(); // Get cached or fetched drop list
            var selectedDrop = DropFetcher.GetRandomDrop(drops); // Pick a drop based on rarity

            if (selectedDrop != null)
            {
                int qty = selectedDrop.GetRandomQuantity(); // Get realistic quantity from range or single value

                // ADD TO INVENTORY
                Inventory.AddItem(selectedDrop.Item, qty);

                MessageBox.Show($" You got: {selectedDrop.Item} x{qty}", "Balloon Drop!");
            }
            else
            {
                MessageBox.Show(" The balloon was empty ", "Whoops");
            }
        }

        private void InitMenuButton()//Initializes menu button
        {
            menuButton = new Button
            {
                Size = new Size(40, 40),
                BackColor = Color.SlateGray,
                FlatStyle = FlatStyle.Flat,
                Text = "≡",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            menuButton.FlatAppearance.BorderSize = 0;

            // Add it first, then position it based on the form size
            this.Controls.Add(menuButton);
            PositionMenuButton();

            menuButton.Click += MenuButton_Click;
        }

        private void PositionMenuButton()//Sets position of menu
        {
            int margin = 10;

            // Use this.ClientSize, NOT this.Size
            int x = this.ClientSize.Width - menuButton.Width - margin;
            int y = this.ClientSize.Height - menuButton.Height - margin;

            menuButton.Location = new Point(10, 10);
        }


        private void InitBalloon()
        {
            try
            {
                balloon = new PictureBox
                {
                    Image = Image.FromFile("Assets/Balloon.png"),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(100, 150),
                    BackColor = Color.Transparent,
                    Location = new Point(-100, 20),
                    Visible = false // Do NOT show instantly
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Balloon image failed to load: " + ex.Message);
                return;
            }

            this.Controls.Add(balloon);
            balloon.BringToFront();

            balloonTimer = new System.Windows.Forms.Timer();
            balloonTimer.Interval = 30;
            balloonTimer.Tick += BalloonTimer_Tick;

            spawnTimer = new System.Windows.Forms.Timer();
            spawnTimer.Tick += SpawnTimer_Tick;

            balloon.Click += Balloon_Click;

            // Random first spawn after launching program
            ScheduleNextBalloonSpawn();
        }

        private void ScheduleNextBalloonSpawn()
        {
            balloonTimer.Stop();
            balloon.Visible = false;

            // Random delay between 5 and 20 seconds
            spawnTimer.Interval = random.Next(1000, 5001);
            spawnTimer.Start();
        }

        private void SpawnTimer_Tick(object sender, EventArgs e)
        {
            spawnTimer.Stop();

            // Random movement settings each time
            balloonSpeed = random.Next(1, 5);          // 1 to 4 pixels per tick
            balloonBaseY = random.Next(10, 60);        // random height near top
            balloonDriftAmount = random.Next(5, 25);   // random wave strength

            driftAngle = (float)(random.NextDouble() * Math.PI * 2);

            balloon.Left = -balloon.Width;
            balloon.Top = balloonBaseY;
            balloon.Visible = true;

            balloonTimer.Start();
        }

        private void BalloonTimer_Tick(object sender, EventArgs e)
        {
            balloon.Left += balloonSpeed;

            driftAngle += 0.1f;

            int yDrift = (int)(balloonDriftAmount * Math.Sin(driftAngle));
            balloon.Top = balloonBaseY + yDrift;

            // If balloon leaves the screen, do NOT instantly reset.
            // Hide it and wait for a random future spawn.
            if (balloon.Left > this.Width)
            {
                ScheduleNextBalloonSpawn();
            }
        }

        private void Balloon_Click(object sender, EventArgs e)
        {
            balloonTimer.Stop();

            System.Media.SoundPlayer player = new System.Media.SoundPlayer("Assets/imp_bite.wav");
            player.Play();

            balloon.Visible = false;

            OnBalloonPopped();

            // Random respawn instead of fixed 2 seconds
            ScheduleNextBalloonSpawn();
        }



        private void MenuButton_Click(object sender, EventArgs e)//handles what happens when you click the menu button
        {
            if (menuForm == null || menuForm.IsDisposed)
            {
                menuForm = new MenuForm();
                menuForm.StartPosition = FormStartPosition.Manual;
                menuForm.Location = new Point(
                    Screen.PrimaryScreen.WorkingArea.Left,
                    Screen.PrimaryScreen.WorkingArea.Top
                );
                menuForm.Show();
            }
            else
            {
                menuForm.Visible = !menuForm.Visible;
            }
        }
    }
}