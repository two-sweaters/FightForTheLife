using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fight_for_The_Life.Domain;

namespace Fight_for_The_Life.Views
{
    public partial class MainForm : Form
    {
        private readonly TableLayoutPanel layoutTable = new TableLayoutPanel();
        private int gameTimeInMilliseconds;
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
            BackgroundImage = Properties.Resources.MainMenuBackground;

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
            BackgroundImage = Properties.Resources.FirstEnemies;

            AddButton("- Выход -", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => MainMenuInitialization());
            AddButton("Следующая страница ->", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => EnemiesSecondPageInitialization());
        }

        private void EnemiesSecondPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Properties.Resources.EnemiesSecond;

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
            BackgroundImage = Properties.Resources.Background;
            var scoreLabel = new Label
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Font = new Font("Segoe Print", 80, FontStyle.Bold, GraphicsUnit.World)
            };
            layoutTable.Controls.Add(scoreLabel);

            game = new Game();
            var timer = new Timer();
            timer.Interval = 1;
            timer.Tick += (sender, args) => gameTimeInMilliseconds++;
            timer.Start();

            Paint += (sender, args) =>
            {
                scoreLabel.Text = "Score: " + game.GetScore(gameTimeInMilliseconds / 1000);
                var indent = (int) (Game.FieldHeight * 0.26993006993);
                var coreModel = game.Sperm.Core.GetModel();
                args.Graphics.FillRectangle(Brushes.GhostWhite, new Rectangle(new Point(game.Sperm.Location.X, game.Sperm.Location.Y + indent), game.Sperm.Model.Size));
                args.Graphics.FillRectangle(Brushes.GhostWhite, new Rectangle(new Point(coreModel.X, coreModel.Y + indent), coreModel.Size));
                Invalidate();
            };

            KeyDown += (sender, args) =>
            {
                if ((args.KeyCode & Keys.Up) != Keys.Up)
                {
                    game.Sperm.MoveUp();
                    Invalidate();
                }

                if (((args.KeyCode & Keys.Down) != Keys.Down))
                {
                    game.Sperm.MoveDown();
                    Invalidate();
                }

                if ((args.KeyData & Keys.Space) != 0)
                {
                    game.Sperm.Core.Shot(game.GetVelocityInPixelsPerSecond(gameTimeInMilliseconds / 1000));
                    Invalidate();
                }

                if ((args.KeyCode & Keys.Escape) != Keys.Escape)
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
            var button = new Label();
            button.Anchor = anchor;
            button.Text = text;
            button.AutoSize = true;
            button.Cursor = Cursors.Hand;
            button.ForeColor = color;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Dock = DockStyle.None;
            button.Font = new Font("Segoe Print", size, FontStyle.Bold, GraphicsUnit.World);
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
