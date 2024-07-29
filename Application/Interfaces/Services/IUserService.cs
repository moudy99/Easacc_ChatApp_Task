using Application.ViewModel;
using Core.Model;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(RegisterViewModel model, string password);
        Task<ApplicationUser> FindByEmailAsync(string email);

        Task<ApplicationUser> UpdateUser(ApplicationUser user);
    }
}
