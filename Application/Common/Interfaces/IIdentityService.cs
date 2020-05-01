using System.Threading.Tasks;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
  public interface IIdentityService
  {
    Task<string> GetUsernameAsync(string userId);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);
  }
}