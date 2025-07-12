using GateWise.Core.DTOs;

namespace GateWise.Core.Interfaces;

public interface IAccessLabService
{
    Task<string> RequestLabAccessAsync	(string userId, AccessLogCreateDto dto);
}