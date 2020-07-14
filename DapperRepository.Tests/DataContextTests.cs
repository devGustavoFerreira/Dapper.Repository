using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperRepository.Tests
{
    [TestClass]
    public class DataContextTests
    {
        #region Context
        [TestMethod]
        public void InstanceCreation()
        {
            DataContext context = new DataContext();

            Assert.AreNotEqual(context, null);

            context.Dispose();
        }

        [TestMethod]
        public void GetConnection()
        {
            DataContext context = new DataContext();
            var connection = context.Connection;

            Assert.AreEqual(connection.State, System.Data.ConnectionState.Open);

            context.Dispose();
        }

        [TestMethod]
        public void BeginTransaction()
        {
            DataContext context = new DataContext();
            var transaction = context.BeginTransaction();

            Assert.AreEqual(transaction.Connection.State, System.Data.ConnectionState.Open);

            context.Dispose();
        }
        #endregion

        #region Sync Methods
        [TestMethod]
        public void Insert()
        {
            DataContext context = new DataContext();

            var product = new Product();
            context.Insert(product);

            Assert.AreNotEqual(product.Id, 0);

            context.Dispose();
        }

        [TestMethod]
        public void InsertBulk()
        {
            DataContext context = new DataContext();

            var products = new List<Product> { new Product(), new Product(), new Product(), new Product(), new Product() };
            int rowCount = context.InsertBulk(products);

            Assert.AreNotEqual(rowCount, 0);

            context.Dispose();
        }

        [TestMethod]
        public void Update()
        {
            DataContext context = new DataContext();

            var product = context.Find<Product>(expression: null);
            product.UpdatedDate = System.DateTime.Now;
            var rowsAffected = context.Update(product);

            Assert.AreNotEqual(rowsAffected, 0);

            context.Dispose();
        }

        [TestMethod]
        public void Delete()
        {
            DataContext context = new DataContext();

            var product = context.Find<Product>(expression: null);
            var rowsAffected = context.Delete(product);

            Assert.AreNotEqual(rowsAffected, 0);

            context.Dispose();
        }

        [TestMethod]
        public void DeleteBulk()
        {
            DataContext context = new DataContext();

            var products = context.FindAll<Product>(p => p.Id > 0).Take(3);
            int rowCount = context.DeleteBulk(products);

            Assert.AreNotEqual(rowCount, 0);

            context.Dispose();
        }

        [TestMethod]
        public void Find()
        {
            DataContext context = new DataContext();

            var product = context.Find<Product>(x => x.Id > 0);

            Assert.AreNotEqual(product, null);
        }
        #endregion

        #region Async Methods
        [TestMethod]
        public async Task InsertAsync()
        {
            DataContext context = new DataContext();

            var product = new Product();
            await context.InsertAsync(product);

            Assert.AreNotEqual(product.Id, 0);

            context.Dispose();
        }

        [TestMethod]
        public async Task InsertBulkAsync()
        {
            DataContext context = new DataContext();

            var products = new List<Product> { new Product(), new Product(), new Product() };
            int rowCount = await context.InsertBulkAsync(products);

            Assert.AreNotEqual(rowCount, 0);

            context.Dispose();
        }

        [TestMethod]
        public async Task UpdateAsync()
        {
            DataContext context = new DataContext();

            var product = context.Find<Product>(null);
            product.UpdatedDate = DateTime.Now;
            var rowsAffected = await context.UpdateAsync(product);

            Assert.AreNotEqual(rowsAffected, 0);

            context.Dispose();
        }

        [TestMethod]
        public async Task DeleteAsync()
        {
            DataContext context = new DataContext();

            var product = context.Find<Product>(null);
            var rowsAffected = await context.DeleteAsync(product);

            Assert.AreNotEqual(rowsAffected, 0);

            context.Dispose();
        }

        public async Task DeleteBulkAsync()
        {
            DataContext context = new DataContext();

            var products = new List<Product> { new Product(), new Product(), new Product() };
            int rowCount = await context.DeleteBulkAsync(products);

            Assert.AreNotEqual(rowCount, 0);

            context.Dispose();
        }
        #endregion
    }
}