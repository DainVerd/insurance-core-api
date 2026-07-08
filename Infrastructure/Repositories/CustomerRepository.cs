using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class CustomerRepository : BaseInMemoryRepository<Customer>, ICustomerRepository
{
}
