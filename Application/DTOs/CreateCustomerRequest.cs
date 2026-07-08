using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs;

public class CreateCustomerRequest
{
    public string FullName { get; init; } = string.Empty;
}
