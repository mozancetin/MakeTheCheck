using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MakeTheCheck.Helpers
{
    public partial class Bill : Form
    {
        List<int> orderIDs;
        public bool cancelled = true;
        public Bill(List<int> orderIDs, float cost)
        {
            InitializeComponent();
            this.orderIDs = orderIDs;
            label1.Text = cost.ToString() + " ₺";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(int orderID in orderIDs)
            {
                DBUtils.DeleteOrderByID(orderID);
            }
            cancelled = false;
            this.Close();
        }
    }
}
