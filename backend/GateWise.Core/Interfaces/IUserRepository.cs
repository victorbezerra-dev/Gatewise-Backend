using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<User?> GetByEmailOrRegistrationAsync(string email, int registrationNumber);

}
