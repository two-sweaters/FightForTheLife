using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fight_for_The_Life.Domain;
using Fight_for_The_Life;
using Fight_for_The_Life.Properties;

namespace Fight_for_The_Life.Views
{
    public partial class MainForm : Form
    {
        private readonly TableLayoutPanel layoutTable = new TableLayoutPanel();
        private int gameTimeInMilliseconds;
        private int timeAfterShotInMilliseconds;
        private Game game;

        public MainForm()
        {
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            Icon = (Icon) resources.GetObject("$this.Icon");
            WindowState = FormWindowState.Maximized;
            Text = "Fight for The Life: Natural Selection";
            Controls.Add(layoutTable);
            BackgroundImageLayout = ImageLayout.Stretch;

            layoutTable.BackColor = Color.Transparent;
            layoutTable.ColumnCount = 3;
            layoutTable.RowCount = 6;
            layoutTable.Dock = DockStyle.Fill;
            for (var i = 0; i < layoutTable.ColumnCount; i++)
                layoutTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            for (var i = 0; i < layoutTable.RowCount; i++)
                layoutTable.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));

            MainMenuInitialization();
        }

        private void MainMenuInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.MainMenuBackground;

            AddButton("- Выход -", 24, Color.White, 1, 5, 
                AnchorStyles.None, (sender, args) => Application.Exit());
            AddButton("Играть", 54, Color.Black, 0, 4, 
                AnchorStyles.Right, (sender, args) => ControlMenuInitialization());
            AddButton("Противники", 54, Color.Black, 2, 4,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
        }

        private void EnemiesFirstPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.FirstEnemies;

            AddButton("- Выход -", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => MainMenuInitialization());
            AddButton("Следующая страница ->", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => EnemiesSecondPageInitialization());
        }

        private void EnemiesSecondPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Resources.EnemiesSecond;

            AddButton("- Выход -", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => MainMenuInitialization());
            AddButton("<- Предыдущая страница", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
        }
        private void ControlMenuInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Properties.Resources.Control;

            AddButton("- К игре -", 24, Color.White, 1, 5,
                AnchorStyles.None, (sender, args) => StartGame());
        }

        private void StartGame()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = null;
            var font = new Font("Segoe Print", 80, FontStyle.Bold, GraphicsUnit.World);

            game = new Game();
            var timer = new Timer {Interval = 50};
            timer.Tick += (sender, args) =>
            {
                gameTimeInMilliseconds += 50;
                Invalidate();
            };
            timer.Start();
            var image = new Bitmap(Resources.Background, Width, Height);
            var spermImage = new Bitmap(Resources.MainSperm, Resources.MainSperm.Width * Width / 1920, 
                Resources.MainSperm.Height * Height / 1080);
            var coreImage = new Bitmap(Resources.Core, Resources.Core.Width * Width / 1920, Resources.Core.Height * Height / 1080);
            Paint += (sender, args) =>
            {
                var text = "Score: " + game.GetScore(gameTimeInMilliseconds / 1000d);
                var indent = (int) (Game.FieldHeight * 0.26993006993);
                var coreModel = game.Sperm.Core.GetModel(timeAfterShotInMilliseconds / 1000d);
                args.Graphics.DrawImage(image, 0, 0);
                args.Graphics.DrawString(text, font, new SolidBrush(Color.White), new PointF(0, 0));
                args.Graphics.DrawImage(spermImage, new Point(game.Sperm.Model.Width - spermImage.Width, 
                    game.Sperm.Location.Y - (spermImage.Height - game.Sperm.Model.Height) / 2 + indent));
                args.Graphics.DrawImage(coreImage, new Point(coreModel.X + coreModel.Width - coreImage.Width,
                    coreModel.Location.Y - (coreImage.Height - coreModel.Height) / 2 + indent));
            };
            KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Up)
                {
                    game.Sperm.MoveUp();
                }

                if (args.KeyCode == Keys.Down)
                {
                    game.Sperm.MoveDown();
                }

                if (args.KeyCode == Keys.Space)
                {
                    game.Sperm.Core.Shot(game.GetVelocityInPixelsPerSecond(gameTimeInMilliseconds / 1000d));
                    timer.Tick += (o, eventArgs) => timeAfterShotInMilliseconds += 50;
                }

                if (args.KeyCode == Keys.Escape)
                {
                    PauseGame();
                }
            };
        }
    


        private void PauseGame()
        {

        }

        private void AddButton(string text, int size, Color color, 
            int column, int row, AnchorStyles anchor, EventHandler actionOnClick=null)
        {
            var button = new Label
            {
                Anchor = anchor,
                Text = text,
                AutoSize = true,
                Cursor = Cursors.Hand,
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Font = new Font("Segoe Print", size, FontStyle.Bold, GraphicsUnit.World)
            };
            button.Click += actionOnClick;
            layoutTable.Controls.Add(button, column, row);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }
    }
}
