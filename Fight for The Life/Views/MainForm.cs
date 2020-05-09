using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fight_for_The_Life.Domain;
using Fight_for_The_Life.Domain.Enemies;
using Fight_for_The_Life.Properties;

namespace Fight_for_The_Life.Views
{
    public partial class MainForm : Form
    {
        private readonly TableLayoutPanel layoutTable = new TableLayoutPanel();
        private readonly Timer timer = new Timer(){ Interval = 50 };
        private Game game;
        private KeyEventHandler onKeyDown;
        private PaintEventHandler gameDrawing;
        public MainForm()
        {
            Size = new Size(1920, 1080);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Resources.GameIcon;
            WindowState = FormWindowState.Maximized;
            Text = "Fight for The Life: Natural Selection";
            Controls.Add(layoutTable);
            BackgroundImageLayout = ImageLayout.Stretch;

            layoutTable.ColumnCount = 3;
            layoutTable.RowCount = 6;
            layoutTable.Dock = DockStyle.Fill;
            for (var i = 0; i < layoutTable.ColumnCount; i++)
                layoutTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            for (var i = 0; i < layoutTable.RowCount; i++)
                layoutTable.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));

            timer.Tick += (sender, args) =>
            {
                game.IncreaseGameTimeInSeconds(50 / 1000d);
                game.UpdateGame();
                if (game.IsGameOver)
                    GameOver();
                Invalidate();
            };

            MainMenuInitialization();
        }

        private void MainMenuInitialization(int textSizeS = 24, int textSizeL = 54)
        {
            Paint -= gameDrawing;
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = Resources.MainMenuBackground;

            AddButton("- Выход -", 24, Color.White, 1, 5, 
                AnchorStyles.None, (sender, args) => Application.Exit());
            AddButton("Играть", 54, Color.Black, 0, 4, 
                AnchorStyles.Right, (sender, args) => ControlMenuInitialization());
            AddButton("Противники", 54, Color.Black, 2, 4,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
            Invalidate();
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
            layoutTable.BackColor = Color.Transparent;
            BackgroundImage = null;
            var font = new Font("Segoe Print", 80, FontStyle.Bold, GraphicsUnit.World);

            game = new Game();
            timer.Start();

            var image = new Bitmap(Resources.Background, Width, Height);

            var spermImage = new Bitmap(Resources.MainSperm, Resources.MainSperm.Width * Width / 1920, 
                Resources.MainSperm.Height * Height / 1080);

            var coreImage = new Bitmap(Resources.Core, Resources.Core.Width * Width / 1920, 
                Resources.Core.Height * Height / 1080);

            var bloodImage = new Bitmap(Resources.Blood, Resources.Blood.Width * Width / 1920, 
                Resources.Blood.Height * Height / 1080);

            var pillImage = new Bitmap(Resources.Pill, Resources.Pill.Width * Width / 1920,
                Resources.Pill.Height * Height / 1080);

            var intrauterineDeviceImage = new Bitmap(Resources.Spiral, 
                Resources.Spiral.Width * Width / 1920, 
                Resources.Spiral.Height * Height / 1080);

            var otherSpermImage = new Bitmap(Resources.Sperm, Resources.Sperm.Width * Width / 1920,
                Resources.Sperm.Height * Height / 1080);

            gameDrawing = (sender, args) =>
            {
                var text = "Score: " + game.GetScore();
                var indent = (int) (Game.FieldHeight * 0.26993006993);
                var coreModel = game.Sperm.Core.GetModel();
                args.Graphics.DrawImage(image, 0, 0);
                args.Graphics.DrawString(text, font, new SolidBrush(Color.White), new PointF(0, 0));
                args.Graphics.DrawImage(coreImage, new Point(coreModel.X + coreModel.Width - coreImage.Width,
                    coreModel.Location.Y - (coreImage.Height - coreModel.Height) / 2 + indent));
                args.Graphics.DrawImage(spermImage, new Point(game.Sperm.Model.Width - spermImage.Width,
                    game.Sperm.Location.Y - (spermImage.Height - game.Sperm.Model.Height) / 2 + indent));
                DrawEnemies(args, bloodImage, intrauterineDeviceImage, pillImage, otherSpermImage, indent);
            };
            Paint += gameDrawing;

            onKeyDown = (sender, args) => ControlInGame(args);
            KeyDown += onKeyDown;
        }

        private void DrawEnemies(PaintEventArgs args, Bitmap bloodImage, Bitmap intrauterineDeviceImage, 
            Bitmap pillImage, Bitmap otherSpermImage, int indent)
        {
            foreach (var enemy in game.LivingEnemies)
            {
                var enemyModel = enemy.GetModel();
                if (enemy is Blood)
                    args.Graphics.DrawImage(bloodImage, new Point(
                        enemyModel.X + enemyModel.Width - bloodImage.Width,
                        enemyModel.Location.Y - (bloodImage.Height - enemyModel.Height) / 2 + indent));

                if (enemy is IntrauterineDevice)
                    args.Graphics.DrawImage(intrauterineDeviceImage, new Point(
                        enemyModel.X,
                        enemyModel.Location.Y - (intrauterineDeviceImage.Height - enemyModel.Height) / 2 + indent));

                if (enemy is BirthControl)
                    args.Graphics.DrawImage(pillImage, new Point(
                        enemyModel.X,
                        enemyModel.Location.Y - (pillImage.Height - enemyModel.Height) / 2 + indent));

                if (enemy is OtherSperm)
                    args.Graphics.DrawImage(otherSpermImage, new Point(
                        enemyModel.X,
                        enemyModel.Location.Y - (otherSpermImage.Height - enemyModel.Height) / 2 + indent));
            }
        }

        private void GameOver()
        {
            timer.Stop();
            KeyDown -= onKeyDown;
            layoutTable.BackColor = Color.FromArgb(128, 0, 0, 0);

            AddButton("Esc - выход в меню", 24, Color.White, 0, 5, 
                AnchorStyles.Left, (sender, args) =>
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    MainMenuInitialization();
                });

            AddButton("Enter - новая игра", 24, Color.White, 2, 5, 
                AnchorStyles.None, (sender, args) =>
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    StartGame();
                });

            AddButton("Game Over", 80, Color.White, 1, 1, AnchorStyles.None);

            onKeyDown = (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    MainMenuInitialization();
                }

                if (args.KeyCode == Keys.Enter)
                {
                    KeyDown -= onKeyDown;
                    Paint -= gameDrawing;
                    StartGame();
                }
            };
            KeyDown += onKeyDown;
        }

        private void PauseGame()
        {
            KeyDown -= onKeyDown;
            onKeyDown = (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    ResumeGame();
            };
            KeyDown += onKeyDown;
            layoutTable.BackColor = Color.FromArgb(128, 0, 0, 0);

            AddButton("- В меню -", 60, Color.White, 1, 2, 
                AnchorStyles.None, (sender, args) =>
                {
                    Paint -= gameDrawing;
                    KeyDown -= onKeyDown;
                    MainMenuInitialization();
                });

            AddButton("- Выход -", 60, Color.White, 1, 3, 
                AnchorStyles.None, (sender, args) => Application.Exit());



            timer.Stop();
        }

        private void ResumeGame()
        {
            layoutTable.Controls.Clear();
            layoutTable.BackColor = Color.Transparent;
            KeyDown -= onKeyDown;
            onKeyDown = (sender, args) => ControlInGame(args);
            KeyDown += onKeyDown;
            timer.Start();
        }

        private void AddButton(string text, int size, Color color, 
            int column, int row, AnchorStyles anchor, EventHandler actionOnClick=null)
        {
            var button = new Label
            {
                Anchor = anchor,
                Text = text,
                BackColor = Color.Transparent,
                AutoSize = true,
                Cursor = Cursors.Hand,
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Font = new Font("Segoe Print",(int) (size * Width / 1920d), FontStyle.Bold, GraphicsUnit.World)
            };
            button.Click += actionOnClick;
            layoutTable.Controls.Add(button, column, row);
        }

        private void ControlInGame(KeyEventArgs args)
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
                game.Sperm.Core.Shot(game.GetVelocityInPixelsPerSecond());
            }

            if (args.KeyCode == Keys.Escape)
            {
                PauseGame();
            }
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
