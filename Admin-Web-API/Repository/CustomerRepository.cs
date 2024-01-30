using Admin_Web_API.Data;
using Admin_Web_API.Models;
using System;

public class CustomerRepository : Repository<Customer>
{
    private readonly McbaContext _mcbaContext;

    public CustomerRepository(McbaContext mcbaContext)
    {
        _mcbaContext = mcbaContext;
    }

    public void Add(Customer entity)
    {
        _mcbaContext.Customers.Add(entity);
        _mcbaContext.SaveChanges();
    }

    public IEnumerable<Customer> GetAll()
    {
        return _mcbaContext.Customers.ToList();
    }

    public Customer GetById(int id)
    {
        return _mcbaContext.Customers.Find(id);
    }

    public void Update(Customer updatedCustomer)
    {
        Customer existingCustomer = GetById(updatedCustomer.CustomerID);
        if (existingCustomer != null)
        {
            existingCustomer.Name = updatedCustomer.Name;
            existingCustomer.TFN = updatedCustomer.TFN;
            existingCustomer.Address = updatedCustomer.Address;
            existingCustomer.City = updatedCustomer.City;
            existingCustomer.PostCode = updatedCustomer.PostCode;
            existingCustomer.State = updatedCustomer.State;
            existingCustomer.Mobile = updatedCustomer.Mobile;
            _mcbaContext.SaveChanges();
        }
    }
}

