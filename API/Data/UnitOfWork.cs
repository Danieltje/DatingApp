using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    /* We're going to be creating instances of the repositories.
       We're gonna pass it what it would have it in it's constructor.

       Before we implemented the UnitOfWork we used DI to get DataContext etc.
       Now we're gonna be creating new instances of the repositories via the UOW.
     */

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);

        /* Saving all the changes EF has tracked in one go, and making sure there are changes by specifying > 0 */
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // If EF has something (to change), this will return true.
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}