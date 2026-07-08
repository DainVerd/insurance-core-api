using Application.DTOs;
using Application.Interfaces.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult GetClaim([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided Customer id is default value!");

        var result = _claimService.GetById(id);

        return Ok(result);
    }
}
