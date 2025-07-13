using GateWise.Core.DTOs;

namespace GateWise.Core.Interfaces;

public interface ILabAccessService
{
    Task<string> RequestLabAccessAsync(string userId, int labId, AccessLogCreateDto dto);
}