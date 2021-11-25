using MakeTheCheck.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MakeTheCheck.Helpers
{
    public partial class TableHelper : Form
    {
        public TableHelper()
        {
            InitializeComponent();
            listBox1.ValueMember = "ID";
            listBox1.DisplayMember = "Number";
        }

        private void ReloadList()
        {
            listBox1.Items.Clear();
            List<Table> tables = DBUtils.GetAllTables();
            tables.ForEach(t => listBox1.Items.Add(t));
            listBox1.Refresh();
        }

        private void TableHelper_Load(object sender, EventArgs e)
        {
            ReloadList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int number = Convert.ToInt32(numericUpDown1.Value);
            DBUtils.AddTable(number);
            ReloadList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                int id = ((Table)listBox1.SelectedItem).ID;
                List<Order> orders = DBUtils.GetOrdersByTableID(id);
                if (orders != null && orders.Count > 0)
                {
                    DialogResult result = MessageBox.Show("There is an order at this table. Delete it anyway?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        DBUtils.DeleteTableByID(id);
                        ReloadList();
                    }
                }
                else
                {
                    DBUtils.DeleteTableByID(id);
                    ReloadList();
                }
            }
        }
    }
}
