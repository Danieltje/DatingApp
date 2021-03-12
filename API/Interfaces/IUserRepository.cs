using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    // In this interface we provide the signatures of the methods that we will support
    // for the Entity this is related to. The Repository will be the concrete methods and
    // the Interface is for the signatures

    // In this case; what are we gonna let the users do?
    public interface IUserRepository
    {
         void Update(AppUser user);
         Task<IEnumerable<AppUser>> GetUsersAsync();
         Task<AppUser> GetUserByIdAsync(int id);
         Task<AppUser> GetUserByUsernameAsync(string username);
         Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
         Task<MemberDto> GetMemberAsync(string username);
         Task<string> GetUserGender(string username);
    }
}