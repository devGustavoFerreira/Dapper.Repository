using DapperRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demonstration
{
    class Application
    {
        IRepository<Customers> _repository;
        public Application(IRepository<Customers> repository)
        {
            _repository = repository;
        }
        public void Run()
        {
            //Create
            var customersToInsert = new List<Customers>(4) { new Customers() { CompanyName = "Company 1" }, new Customers() { CompanyName = "Company 2" }, new Customers() { CompanyName = "Company 3" }, new Customers() { CompanyName = "Company 4" } };
            _repository.InsertBulk(customersToInsert);

            //Read
            var customerFound = _repository.Find(c => c.Id == 2);
            var customersToUpdate = _repository.FindAll(c => c.CompanyName == "Company 3");
            var customersToDelete = _repository.FindAll(c => c.CompanyName == "Company 2");

            //Update
            foreach (var customerToUpdate in customersToUpdate)
            {
                customerToUpdate.City = "Montes Claros";
                customerToUpdate.Region = "Minas Gerais";
                _repository.Update(customerToUpdate);
            }
            //Delete
            foreach (var customerToDelete in customersToDelete)
                _repository.Delete(customerToDelete);

        }
    }
}
