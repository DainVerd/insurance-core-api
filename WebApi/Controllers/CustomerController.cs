using Application.DTOs;
using Application.Interfaces.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/customers")]
[ApiVersion("1.0")]
public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        var id = _customerService.CreateCustomer(request);

        return CreatedAtAction(nameof(CreateCustomer), new { id, version = "1.0" }, id);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult GetCustomer([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided Customer id has default value!");

        var result = _customerService.GetCustomerById(id);

        return Ok(result);
    }
}
