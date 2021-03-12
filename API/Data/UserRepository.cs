using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            /* On this stage this query is a IQueryable. This is an expression tree that is gonna go to
               EF. EF will actually build it up as an expression tree. Then (before) when we execute the ToListAsync()
               it will actually execute the query

               This is a list we're only ever going to read from. Not doing anything else with these en
             */
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            // Let's say a user is looking for someone between 20 and 30. MaxAge would be 30. MinAge would be 20.
            // maxDob is f.e. anyone born in year 2000 or below. minDob will be 1990 as is 30 years old now.
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            // Implementing a sorting operation. We want to sort the query by Created and for default LastActive
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper
                .ConfigurationProvider).AsNoTracking(), 
                    userParams.PageNumber, userParams.PageSize);
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

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .Select(x => x.Gender)
                .FirstOrDefaultAsync();
        }

        // method to return all of our users
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
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