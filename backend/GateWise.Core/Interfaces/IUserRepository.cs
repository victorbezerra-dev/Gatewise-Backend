using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string id);
    Task<User?> GetByEmailOrRegistrationAsync(string email, string registrationNumber);

}
