using Application.DTOs;
using Application.Interfaces.Services;
using Asp.Versioning;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/claims")]
[ApiVersion("1.0")]
public class ClaimController : Controller
{
    private readonly IClaimService _claimService;
    public ClaimController(IClaimService claimService)
    {
        _claimService = claimService;
    }
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult CreateClaim([FromBody] CreateClaimRequest request)
    {
        var result = _claimService.CreateClaim(request);

        return Ok(result);
    }
}
