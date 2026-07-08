using Application.DTOs;
using Application.Interfaces.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

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
}
