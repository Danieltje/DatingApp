using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        // Information to get a specific user like; The user who initiated the like, and the one who got liked
         Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

         // Just get the AppUser by giving the userId
         Task<AppUser> GetUserWithLikes(int userId);

         // Get the likes for specific user
         Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
    }
}