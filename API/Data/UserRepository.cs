using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    // this is going to implement/use our IUserRepository interface
    public class UserRepository : IUserRepository
    {
        // we need a constructor bcus it will access the dbcontext
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {

            return await _context.Users
                .Where(x => x.UserName == username)
                // automapper queryable extensions
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // we return the user with the ID we try to use in the parameter
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // we return the user with the username we try to use in the parameter
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            // this will give us a circular reference exception; it's endless as course instructor shows
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        // method to return all of our users
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        // we return a bool to say if our changes have been saved
        // we want to save when something has changed; a value greater than 0
        // the SaveChanges method returns an integer for the number of changes made
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        /* We're "updating" the user here. Not changing anything in the database, but we mark this
           Entity (AppUser) that it has been modified. 

        */
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}