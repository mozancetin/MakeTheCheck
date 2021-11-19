using MakeTheCheck.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MakeTheCheck.Helpers
{
    public partial class AddProductHelper : Form
    {
        private bool imageSelected = false;
        private ImageModel imgObject = new ImageModel();
        public bool isNew;
        private Dictionary<string, object> Params = new Dictionary<string, object>();
        public AddProductHelper(bool isNew = true, Dictionary<string, object> Parameters = null)
        {
            InitializeComponent();
            this.isNew = isNew;
            if (!isNew) 
            {
                button1.Text = "Update";
                this.Params = Parameters;
                imgObject = (ImageModel)Params["imgObject"];
                imageSelected = true;
                ReloadWithParams();
            }
        }

        private void ReloadWithParams()
        {
            textBox1.Text = ((Product)Params["productObject"]).Name;
            textBox4.Text = ((Product)Params["productObject"]).Description;
            numericUpDown1.Value = (decimal)((Product)Params["productObject"]).Price;
        }

        private void imgButton_Click(object sender, EventArgs e)
        {
            SelectImageHelper helper = new SelectImageHelper();
            this.Hide();
            helper.ShowDialog();
            if (!helper.cancelled)
            {
                imgObject = (ImageModel)helper.SelectedItem;
                pictureBox1.Image = imgObject.Img;
                imageSelected = true;
            }
            this.Show();
        }

        private void AddProductHelper_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(DBUtils.GetAllTypes().ToArray());
            comboBox1.DisplayMember = "TypeName";
            comboBox1.ValueMember = "ID";
            comboBox1.SelectedIndex = 0;
            if (isNew)
            {
                pictureBox1.Image = Utils.ResizeBitmap(new Bitmap("../../Images/default.jpg"), 200, 300);
            }
            else
            {
                pictureBox1.Image = Utils.ResizeBitmap((Bitmap)imgObject.Img, 200, 300);
                string typeName = ((TypeModel)Params["typeObject"]).TypeName;
                foreach(TypeModel cbItem in comboBox1.Items)
                {
                    if(cbItem.TypeName == typeName)
                    {
                        comboBox1.SelectedItem = cbItem;
                        break;
                    }
                }
            }
            comboBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Name property can't left blank!");
                    return;
                }

                if (isNew)
                {
                    if (imageSelected)
                    {
                        DBUtils.AddProduct(textBox1.Text, DBUtils.GetProductTypeByName(comboBox1.Text).ID, (float)numericUpDown1.Value, textBox4.Text == null ? null : textBox4.Text, imgObject.ID);
                    }
                    else
                    {
                        DBUtils.AddProduct(textBox1.Text, DBUtils.GetProductTypeByName(comboBox1.Text).ID, (float)numericUpDown1.Value, textBox4.Text == null ? null : textBox4.Text, 0);
                    }
                    MessageBox.Show("Product successfully added!");
                }
                else
                {
                    if (imageSelected)
                    {
                        DBUtils.UpdateProduct(((Product)Params["productObject"]).ID, textBox1.Text, DBUtils.GetProductTypeByName(comboBox1.Text).ID, (float)numericUpDown1.Value, textBox4.Text == null ? null : textBox4.Text, imgObject.ID);
                    }
                    else
                    {
                        DBUtils.UpdateProduct(((Product)Params["productObject"]).ID, textBox1.Text, DBUtils.GetProductTypeByName(comboBox1.Text).ID, (float)numericUpDown1.Value, textBox4.Text == null ? null : textBox4.Text, 0);
                    }
                    MessageBox.Show("Product successfully updated!");
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddTypeHelper helper = new AddTypeHelper();
            Hide();
            helper.ShowDialog();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(DBUtils.GetAllTypes().ToArray());
            comboBox1.DisplayMember = "TypeName";
            comboBox1.ValueMember = "ID";
            comboBox1.SelectedIndex = 0;
            comboBox1.Refresh();
            Show();
        }
    }
}
