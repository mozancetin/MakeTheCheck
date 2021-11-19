using MakeTheCheck.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace MakeTheCheck.Helpers
{
    public partial class ManageProductHelper : Form
    {
        public ManageProductHelper()
        {
            InitializeComponent();
        }

        private void ReloadList()
        {
            listBox1.Items.Clear();
            DBUtils.GetAllProducts().ForEach(item => listBox1.Items.Add(item));
            Refresh();
        }

        private void ManageProductHelper_Load(object sender, EventArgs e)
        {
            listBox1.ValueMember = "ID";
            listBox1.DisplayMember = "Name";
            ReloadList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddProductHelper helper = new AddProductHelper();
            this.Hide();
            helper.ShowDialog();
            ReloadList();
            this.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count == 1)
            {
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Product item = (Product)listBox1.SelectedItem;
                Params["productObject"] = item;
                Params["imgObject"] = DBUtils.GetImageByID(item.ProductImage);
                Params["typeObject"] = DBUtils.GetProductTypeByName(item.Type);

                AddProductHelper helper = new AddProductHelper(false, Params);
                this.Hide();
                helper.ShowDialog();
                ReloadList();
                this.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show($"Do you really want to delete {listBox1.SelectedItems.Count} products?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.No) { return; }

                foreach(Product item in listBox1.SelectedItems)
                {
                    DBUtils.DeleteProduct(item.ID);
                }
                ReloadList();
            }
        }
    }
}
