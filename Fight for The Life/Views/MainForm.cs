using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fight_for_The_Life.Views
{
    public partial class MainForm : Form
    {
        private readonly TableLayoutPanel layoutTable = new TableLayoutPanel();
        public MainForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            Icon = (Icon) resources.GetObject("$this.Icon");
            WindowState = FormWindowState.Maximized;
            Text = "Fight for The Life: Natural Selection";
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(layoutTable);

            layoutTable.BackColor = Color.FromArgb(0, 0, 0, 0);
            layoutTable.ColumnCount = 3;
            layoutTable.RowCount = 6;
            layoutTable.Dock = DockStyle.Fill;
            for (var i = 0; i < layoutTable.ColumnCount; i++)
                layoutTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            for(var i = 0; i < layoutTable.RowCount; i++)
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
                AnchorStyles.Right, (sender, args) => Application.Exit());
            AddButton("Противники", 54, Color.Black, 2, 4,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
        }

        private void EnemiesFirstPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Properties.Resources.FirstEnemies;

            AddButton("- Выход -", 24, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => MainMenuInitialization());
            AddButton("Следующая страница ->", 20, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => EnemiesSecondPageInitialization());
        }

        private void EnemiesSecondPageInitialization()
        {
            layoutTable.Controls.Clear();
            BackgroundImage = Properties.Resources.EnemiesSecond;

            AddButton("- Выход -", 24, Color.White, 2, 5,
                AnchorStyles.Right, (sender, args) => MainMenuInitialization());
            AddButton("<- Предыдущая страница", 20, Color.White, 0, 5,
                AnchorStyles.Left, (sender, args) => EnemiesFirstPageInitialization());
        }

        private void AddButton(string text, int size, Color color, 
            int column, int row, AnchorStyles anchor, EventHandler actionOnClick)
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
    }
}
