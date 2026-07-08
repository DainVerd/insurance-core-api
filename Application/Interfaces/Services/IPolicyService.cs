using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IPolicyService
{
    Guid CreatePolicy(CreatePolicyRequest request);
    PolicyDto GetById(Guid id);
    void ActivatePolicy(Guid id);
}
