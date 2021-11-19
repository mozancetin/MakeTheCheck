using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Data.SQLite;
using MakeTheCheck.Models;
using System.Drawing;

namespace MakeTheCheck
{
    public static class DBUtils
    {
        public static string databasePath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\Data\allData.db"; // On Release: Environment.CurrentDirectory + @"\Data\allData.db".
        public static void CreateTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Products(ID INTEGER PRIMARY KEY AUTOINCREMENT, ProductName TEXT NOT NULL, ProductType INTEGER NOT NULL, Price FLOAT NOT NULL, Description TEXT, ProductImage INT NOT NULL, FOREIGN KEY (ProductType) REFERENCES Types (ID), FOREIGN KEY (ProductImage) REFERENCES Images (ID))";
                    command.ExecuteNonQuery();

                    var command2 = connection.CreateCommand();
                    command2.CommandText = "CREATE TABLE IF NOT EXISTS Tables(ID INTEGER PRIMARY KEY AUTOINCREMENT, TABLE_NUMBER INTEGER NOT NULL)";
                    command2.ExecuteNonQuery();

                    var command3 = connection.CreateCommand();
                    command3.CommandText = "CREATE TABLE IF NOT EXISTS Orders(ID INTEGER PRIMARY KEY AUTOINCREMENT, ProductID INTEGER NOT NULL, TableID INTEGER NOT NULL, FOREIGN KEY (ProductID) REFERENCES Products (ID), FOREIGN KEY (TableID) REFERENCES Tables (ID))";
                    command3.ExecuteNonQuery();

                    var command4 = connection.CreateCommand();
                    command4.CommandText = "CREATE TABLE IF NOT EXISTS Types(ID INTEGER PRIMARY KEY AUTOINCREMENT, Type TEXT NOT NULL)";
                    command4.ExecuteNonQuery();

                    var command5 = connection.CreateCommand();
                    command5.CommandText = "CREATE TABLE IF NOT EXISTS Images(ID INTEGER PRIMARY KEY AUTOINCREMENT, Image BLOB NOT NULL)";
                    command5.ExecuteNonQuery();

                    AddType("Default", 0);
                    AddImage(new Bitmap("../../Images/default.jpg"), 0); // Change path on release.
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            if (imageIn == null) { return null; }
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null) { return null; }
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        public static TypeModel GetProductTypeByID(int typeid)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Type FROM Types WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", typeid);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        while (reader.Read())
                        {
                            return new TypeModel() { ID = reader.GetInt32(0), TypeName = reader.GetString(1) };
                        }
                    }
                    return null;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static Product GetProductByID(int id)
        {
            try
            {
                Product product = new Product();
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, ProductName, ProductType, Price, Description, ProductImage FROM Products WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TypeModel productType = GetProductTypeByID(reader.GetInt32(2));
                            if (string.IsNullOrWhiteSpace(productType.TypeName)) { return null; }
                            product = new Product()
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Type = productType.TypeName,
                                Price = reader.GetFloat(3),
                                Description = reader.GetString(4),
                                ProductImage = reader.GetInt32(5)
                            };
                        }
                    }
                    return product;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static ImageModel GetImageByID(int id)
        {
            ImageModel model = new ImageModel();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Image FROM Images WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using(var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        model = new ImageModel() { ID = reader.GetInt32(0), Img = ByteArrayToImage((byte[])reader[1]) };
                    }
                    return model;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static ImageModel GetImageByImage(Image img)
        {
            ImageModel model = new ImageModel() { Img = null };
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Image FROM Images WHERE Image = @img";
                    command.Parameters.AddWithValue("@img", ImageToByteArray(img));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            model = new ImageModel() { ID = reader.GetInt32(0), Img = img };
                        }
                    }
                    return model;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static List<ImageModel> GetAllImages()
        {
            List<ImageModel> allImages = new List<ImageModel>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Image FROM Images";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allImages.Add(new ImageModel() { ID = reader.GetInt32(0), Img = ByteArrayToImage((byte[])reader[1]) });
                        }
                    }
                }
                return allImages;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static List<Table> GetAllTables()
        {
            List<Table> allTables = new List<Table>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, TABLE_NUMBER FROM Tables";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allTables.Add(new Table() { ID = reader.GetInt32(0), Number = reader.GetInt32(1) });
                        }
                    }
                }
                return allTables;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static void AddOrder(int ProductID, int TableID)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Orders (ProductID, TableID) VALUES(@pid, @tid)";
                    command.Parameters.AddWithValue("@pid", ProductID);
                    command.Parameters.AddWithValue("@tid", TableID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static List<Order> GetOrdersByTableID(int id)
        {
            List<Order> orderList = new List<Order>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, ProductID, TableID FROM Orders WHERE TableID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderList.Add(new Order() { ID = reader.GetInt32(0), ProductID = reader.GetInt32(1), TableID = reader.GetInt32(2) });
                        }
                    }
                }
                return orderList;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static void DeleteOrderByID(int id)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Orders WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static bool IsTableNumberExists(int TableNumber)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, TABLE_NUMBER FROM Tables WHERE TABLE_NUMBER = @number";
                    command.Parameters.AddWithValue("@number", TableNumber);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return true;
            }
        }

        public static void AddTable(int TableNumber)
        {
            if (IsTableNumberExists(TableNumber))
            {
                MessageBox.Show("This table number is already exists.");
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Tables (TABLE_NUMBER) VALUES(@number)";
                    command.Parameters.AddWithValue("@number", TableNumber);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static void DeleteTableByID(int id)
        {
            List<Order> orders = GetOrdersByTableID(id);
            if (orders != null)
            {
                if (orders.Count > 0)
                {
                    MessageBox.Show("There is an order at this table. You cannot delete it.");
                    return;
                }
            }
            orders.Clear();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Tables WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static void AddImage(Image img, int id = -1)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    if (id != -1)
                    {
                        command.CommandText = "INSERT OR REPLACE INTO Images (ID, Image) VALUES(@id, @img)";
                        command.Parameters.AddWithValue("@id", id);
                    }
                    else
                    {
                        command.CommandText = "INSERT INTO Images (Image) VALUES(@img)";
                    }
                    
                    command.Parameters.AddWithValue("@img", ImageToByteArray(img));
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static TypeModel GetProductTypeByName(string typename)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Type FROM Types WHERE Type = @type";
                    command.Parameters.AddWithValue("@type", typename);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new TypeModel() { ID = reader.GetInt32(0), TypeName = reader.GetString(1) };
                        }
                    }
                    return null;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return null;
            }
        }

        public static void AddProduct(string name, int typeID, float price, string desc, int imgID)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Products (ProductName, ProductType, Price, Description, ProductImage) VALUES (@name, @type, @price, @desc, @img)";
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@type", typeID);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@desc", desc);
                    command.Parameters.AddWithValue("@img", imgID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static void DeleteProduct(int id)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Products WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static void AddType(string name, int id = -1)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    if (id != -1)
                    {
                        command.CommandText = "INSERT OR REPLACE INTO Types (ID, Type) VALUES(@id, @type)";
                        command.Parameters.AddWithValue("@id", id);
                    }
                    else
                    {
                        command.CommandText = "INSERT INTO Types (Type) VALUES(@type)";
                    }
                    command.Parameters.AddWithValue("@type", name);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        public static TypeModel GetTypeByID(int id)
        {
            TypeModel returnType = new TypeModel();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Type FROM Types WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnType = new TypeModel()
                            {
                                ID = reader.GetInt32(0),
                                TypeName = reader.GetString(1)
                            };
                            break;
                        }
                    }
                    return returnType;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return new TypeModel();
            }
        }

        public static List<TypeModel> GetAllTypes()
        {
            List<TypeModel> types = new List<TypeModel>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Type FROM Types";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            types.Add(new TypeModel()
                            {
                                ID = reader.GetInt32(0),
                                TypeName = reader.GetString(1)
                            });
                        }
                    }
                    return types;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return new List<TypeModel>();
            }
        }

        public static List<Product> GetAllProducts()
        {
            try
            {
                List<Product> products = new List<Product>();
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, ProductName, ProductType, Price, Description, ProductImage FROM Products";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TypeModel productType = GetProductTypeByID(reader.GetInt32(2));
                            if (string.IsNullOrWhiteSpace(productType.TypeName)) { continue; }
                            products.Add(new Product()
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Type = productType.TypeName,
                                Price = reader.GetFloat(3),
                                Description = reader.GetString(4),
                                ProductImage = reader.GetInt32(5)
                            });
                        }
                    }
                    return products;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return new List<Product>();
            }
        }

        public static void UpdateProduct(int ProductID, string name, int typeID, float price, string desc, int imgID)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Products SET ProductName = @name, ProductType = @type, Price = @price, Description = @desc, ProductImage = @img WHERE ID = @id";
                    command.Parameters.AddWithValue("@id", ProductID);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@type", typeID);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@desc", desc);
                    command.Parameters.AddWithValue("@img", imgID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }
    }
}
