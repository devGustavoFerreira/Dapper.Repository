using DapperRepository.Providers;
using DapperRepository.Providers.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DapperRepository.Tests
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void InsertQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.InsertQuery("Product", new Product());
            
            Assert.AreEqual(commandText, "INSERT INTO [Product]([Product].[Name], [Product].[Price], [Product].[UpdatedDate], [Product].[CreatedDate]) OUTPUT INSERTED.Id VALUES(@Name, @Price, @UpdatedDate, @CreatedDate)");
        }

        [TestMethod]
        public void InsertBulkQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.InsertBulkQuery("Product", new List<Product> { new Product(), new Product(), new Product() });

            Assert.AreEqual(commandText, "INSERT INTO [Product]([Product].[Name], [Product].[Price], [Product].[UpdatedDate], [Product].[CreatedDate]) VALUES (@Name1, @Price1, @UpdatedDate1, @CreatedDate1)\r\nINSERT INTO [Product]([Product].[Name], [Product].[Price], [Product].[UpdatedDate], [Product].[CreatedDate]) VALUES (@Name2, @Price2, @UpdatedDate2, @CreatedDate2)\r\nINSERT INTO [Product]([Product].[Name], [Product].[Price], [Product].[UpdatedDate], [Product].[CreatedDate]) VALUES (@Name3, @Price3, @UpdatedDate3, @CreatedDate3)\r\n");
        }

        [TestMethod]
        public void UpdateQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.UpdateQuery("Product", new Product());

            Assert.AreEqual(commandText, "UPDATE [Product] SET [Product].[Name] = @Name, [Product].[Price] = @Price, [Product].[UpdatedDate] = @UpdatedDate, [Product].[CreatedDate] = @CreatedDate WHERE [Product].[Id] = @Id");
        }

        [TestMethod]
        public void UpdateBulkQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.UpdateBulkQuery("Product", new List<Product> { new Product(), new Product(), new Product() });

            Assert.AreEqual(commandText, "UPDATE [Product] SET [Product].[Name] = @Name1, [Product].[Price] = @Price1, [Product].[UpdatedDate] = @UpdatedDate1, [Product].[CreatedDate] = @CreatedDate1 WHERE [Product].[Id] = @Id\r\nUPDATE [Product] SET [Product].[Name] = @Name2, [Product].[Price] = @Price2, [Product].[UpdatedDate] = @UpdatedDate2, [Product].[CreatedDate] = @CreatedDate2 WHERE [Product].[Id] = @Id\r\nUPDATE [Product] SET [Product].[Name] = @Name3, [Product].[Price] = @Price3, [Product].[UpdatedDate] = @UpdatedDate3, [Product].[CreatedDate] = @CreatedDate3 WHERE [Product].[Id] = @Id\r\n");
        }

        [TestMethod]
        public void DeleteQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.DeleteQuery("Product");

            Assert.AreEqual(commandText, "DELETE FROM [Product] WHERE [Product].[Id] = @Id");
        }

        [TestMethod]
        public void DeleteBulkQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.DeleteBulkQuery("Product");

            Assert.AreEqual(commandText, "DELETE FROM [Product] WHERE [Product].[Id] IN(@Ids)");
        }

        [TestMethod]
        public void SelectFirstQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.SelectFirstQuery<Product>(x => x.Id > 0, "Product");

            Assert.AreEqual(commandText, "SELECT TOP(1)\r\n[Product].[Name],\r\n[Product].[Price],\r\n[Product].[UpdatedDate],\r\n[Product].[CreatedDate],\r\n[Product].[Id] FROM [Product] WHERE [Product].[Id] > @Id");
        }

        [TestMethod]
        public void SelectQuery()
        {
            IProvider provider = new SqlServerProvider();
            string commandText = provider.SelectQuery<Product>(p => p.Id > 0, "Product");

            Assert.AreEqual(commandText, "SELECT\r\n [Product].[Name],\r\n[Product].[Price],\r\n[Product].[UpdatedDate],\r\n[Product].[CreatedDate],\r\n[Product].[Id] FROM [Product] WHERE [Product].[Id] > @Id");
        }
    }
}