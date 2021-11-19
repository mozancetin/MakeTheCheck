using System;
using System.Windows.Forms;

namespace MakeTheCheck.Helpers
{
    public partial class AddTypeHelper : Form
    {
        public AddTypeHelper()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("New Type Name space can't left blank!");
                return;
            }

            if (DBUtils.GetProductTypeByName(textBox1.Text) == null)
            {
                DBUtils.AddType(textBox1.Text);
                MessageBox.Show("New type successfully added.");
                this.Close();
            }
            else
            {
                MessageBox.Show("This type name already exists.");
            }
        }
    }
}
