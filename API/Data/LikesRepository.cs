using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        // Find an individual like
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            // sourceUserId and likedUserId make up our primary key in this particular entity
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        // Depending on which list they're looking for, we'll add some if statements to return slightly different
        // We can see what the sourceUser has liked, and who liked the sourceUser depending on the input
        public Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            throw new System.NotImplementedException();
        }

        // Get the list of users that this user has liked
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}