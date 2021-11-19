using MakeTheCheck.Models;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MakeTheCheck.Helpers
{
    public partial class SelectImageHelper : Form
    {
        public object SelectedItem { get; set; }
        public bool cancelled = true;
        public SelectImageHelper()
        {
            InitializeComponent();
        }

        private void SelectImageHelper_Load(object sender, EventArgs e)
        {
            SelectedItem = null;
            foreach(ImageModel model in DBUtils.GetAllImages())
            {
                MyPictureBox pb = new MyPictureBox();
                pb.Location = new Point(0, 0);
                pb.Size = new Size(200, 300);
                pb.TabStop = false;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Item = model;
                pb.Image = model.Img;
                pb.BorderStyle = BorderStyle.FixedSingle;

                pb.Click += Pb_Click;
                pb.DoubleClick += Pb_DoubleClick;

                panel.Controls.Add(pb);
            }
        }

        private void Pb_DoubleClick(object sender, EventArgs e)
        {
            SelectedItem = ((MyPictureBox)sender).Item;
            cancelled = false;
            this.Close();
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            SelectedItem = ((MyPictureBox)sender).Item;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\Images"; //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Filter = "PNG File (*.png)|*.png|JPG File|*.jpg;*.JPG;*.jpeg";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                Image orimage = Image.FromFile(filePath);
                ImageModel img = DBUtils.GetImageByImage(orimage);
                if (img.Img == null) {
                    DBUtils.AddImage(orimage);
                    SelectedItem = DBUtils.GetImageByImage(orimage);
                    cancelled = false;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("This image already exists in database.");
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null)
            {
                cancelled = false;
                this.Close();
            }
        }
    }
}
