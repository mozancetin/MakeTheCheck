namespace MakeTheCheck.Models
{
    public class OrderWithProduct
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int TableID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public float ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public int ProductImage { get; set; }

        public OrderWithProduct(Order order, Product pro)
        {
            this.OrderID = order.ID;
            this.ProductID = order.ProductID;
            this.TableID = order.TableID;
            this.ProductName = pro.Name;
            this.ProductType = pro.Type;
            this.ProductPrice = pro.Price;
            this.ProductDescription = pro.Description;
            this.ProductImage = pro.ProductImage;
        }
    }
}
