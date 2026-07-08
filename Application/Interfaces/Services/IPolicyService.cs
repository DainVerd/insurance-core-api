using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IPolicyService
{
    Guid CreatePolicy(CreatePolicyRequest request);
    PolicyDto GetById(Guid id);
}
