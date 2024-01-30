using Admin_Web_API.Data;
using Admin_Web_API.Models;
using System;

public class LoginRepository : Repository<Login>
{
    private readonly McbaContext _mcbaContext;

    public LoginRepository(McbaContext mcbaContext)
    {
        _mcbaContext = mcbaContext;
    }

    public void Add(Login entity)
    {
        _mcbaContext.Logins.Add(entity);
        _mcbaContext.SaveChanges();
    }

    public IEnumerable<Login> GetAll()
    {
        return _mcbaContext.Logins.ToList();
    }

    public Login GetById(int id)
    {
        return _mcbaContext.Logins.FirstOrDefault(x => x.CustomerID == id);
    }

    public void Update(Login updatedLogin)
    {
        Login existingLogin = GetById(updatedLogin.CustomerID);
        if (existingLogin != null)
        {
            existingLogin.locked = updatedLogin.locked;
            _mcbaContext.SaveChanges();
        }
    }
}

