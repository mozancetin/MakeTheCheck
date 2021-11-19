using MakeTheCheck.Helpers;
using System;
using System.Windows.Forms;

namespace MakeTheCheck
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
            DBUtils.CreateTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddToTable attWindow = new AddToTable();
            this.Hide();
            attWindow.ShowDialog();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ManageProductHelper helper = new ManageProductHelper();
            Hide();
            helper.ShowDialog();
            Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TableHelper helper = new TableHelper();
            Hide();
            helper.ShowDialog();
            Show();
        }
    }
}
