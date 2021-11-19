using MakeTheCheck.Helpers;
using MakeTheCheck.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MakeTheCheck
{
    public partial class AddToTable : Form
    {
        float total = 0;
        public AddToTable()
        {
            InitializeComponent();
            Panel.Enabled = false;
            billBox.View = View.Details;
            billBox.GridLines = true;
            billBox.CheckBoxes = false;
            billBox.FullRowSelect = true;
            billBox.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            billBox.Columns[0].Width = 213;
            billBox.Columns[1].Width = 90;
            billBox.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach(Product product in DBUtils.GetAllProducts())
            {
                MyButton btn = new MyButton();
                btn.TitleText = product.Name;
                btn.DescText = product.Description;
                btn.PriceText = product.Price.ToString() + "₺";
                btn.Name = product.ID.ToString();
                btn.Item = product;
                btn.Size = new Size(700, 300);
                btn.Location = new Point(0, 0);
                btn.ImageAlign = ContentAlignment.MiddleLeft;
                btn.TextAlign = ContentAlignment.TopRight;
                btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                btn.Image = Utils.ResizeBitmap((Bitmap)(DBUtils.GetImageByID(product.ProductImage)).Img, 200, 300);
                btn.Click += Btn_Click;
                Panel.Controls.Add(btn);
            }

            comboBox1.DataSource = DBUtils.GetAllTables();
            comboBox1.ValueMember = "ID";
            comboBox1.DisplayMember = "Number";
            comboBox1.Refresh();

            if(comboBox1.Items.Count > 0)
            {
                ReloadList();
                Panel.Enabled = true;
            }
        }

        private void CreateListViewItem(ListView view, OrderWithProduct obj)
        {
            ListViewItem item = new ListViewItem(" " + obj.ProductName);
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, obj.ProductPrice.ToString()));
            item.Tag = obj;
            view.Items.Add(item);
        }

        private void ReloadList()
        {
            total = 0;
            billBox.Items.Clear();
            List<Order> orders = DBUtils.GetOrdersByTableID(((Table)comboBox1.SelectedItem).ID);
            List<OrderWithProduct> orderswithproducts = new List<OrderWithProduct>();
            
            orders.ForEach(o => 
            { 
                Product product = DBUtils.GetProductByID(o.ProductID);
                orderswithproducts.Add(new OrderWithProduct(o, product));
                total += product.Price;
            });
            orderswithproducts.ForEach(o => CreateListViewItem(billBox, o));
            billBox.Refresh();
            total = (float)Math.Round(total, 2, MidpointRounding.ToEven);
            totalLabel.Text = total.ToString() + " ₺";
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DBUtils.AddOrder(((Product)((MyButton)sender).Item).ID, ((Table)comboBox1.SelectedItem).ID);
                ReloadList();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (billBox.SelectedItems.Count > 0)
            {
                float selectedTotal = 0;
                List<int> orderIDs = new List<int>();
                foreach(ListViewItem item in billBox.SelectedItems)
                {
                    OrderWithProduct order = (OrderWithProduct)item.Tag;
                    selectedTotal += order.ProductPrice;
                    orderIDs.Add(order.OrderID);
                }
                selectedTotal = (float)Math.Round(selectedTotal, 2, MidpointRounding.ToEven);
                Bill helper = new Bill(orderIDs, selectedTotal);
                helper.ShowDialog();
                if (!helper.cancelled) { ReloadList(); }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (billBox.Items.Count > 0)
            {
                List<int> orderIDs = new List<int>();
                foreach (ListViewItem item in billBox.Items)
                {
                    OrderWithProduct order = (OrderWithProduct)item.Tag;
                    orderIDs.Add(order.OrderID);
                }
                Bill helper = new Bill(orderIDs, (float)Math.Round(total / Convert.ToInt32(numericUpDown1.Value), 2, MidpointRounding.ToEven));
                helper.ShowDialog();
                if (!helper.cancelled) { ReloadList(); }
                numericUpDown1.Value = 2;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (billBox.Items.Count > 0)
            {
                List<int> orderIDs = new List<int>();
                foreach(ListViewItem item in billBox.Items)
                {
                    OrderWithProduct order = (OrderWithProduct)item.Tag;
                    orderIDs.Add(order.OrderID);
                }
                Bill helper = new Bill(orderIDs, total);
                helper.ShowDialog();
                if (!helper.cancelled) { ReloadList(); }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (billBox.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in billBox.SelectedItems)
                {
                    DBUtils.DeleteOrderByID(((OrderWithProduct)item.Tag).OrderID);
                }
                ReloadList();
            }
        }
    }
}
