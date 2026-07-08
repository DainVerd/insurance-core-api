using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/policies")]
[ApiVersion("1.0")]
public class PolicyController : Controller
{
    private readonly IPolicyService _policyService;
    public PolicyController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreatePolicy([FromBody] CreatePolicyRequest request)
    {
        var result = _policyService.CreatePolicy(request);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetPolicyById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided Policy id is default value!");

        var result = _policyService.GetById(id);

        return Ok(result);
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ActivatePolicyById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided Policy id is default value!");

        _policyService.ActivatePolicy(id);

        return Ok();
    }
}
