using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
        /* We here want to return a list of users. We are querying both the users and likes table, 
            because we want to see if there are likes associated with the users involved
            We're joining these inside our query and letting EF work out the join query that needs to be made
            Gonna select the properties we need in the LikeDto
        */
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            // This will give us the list of users that have liked the currently logged in user.
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto 
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers,
                likesParams.PageNumber, likesParams.PageSize);
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