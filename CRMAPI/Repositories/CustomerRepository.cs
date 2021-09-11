using CRMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMAPI.Repositories
{
    public class CustomerRepository : IRepository<Customer, CustomerDTO>
    {
        public Task<bool> Add(Customer entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Customer entity)
        {
            throw new NotImplementedException();
        }

        public Task<Customer> Get(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<CustomerDTO> GetDto(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Customer entityToUpdate, Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}
