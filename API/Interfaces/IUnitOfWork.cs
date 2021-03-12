using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
         // We want to return our Repositories.
         IUserRepository UserRepository { get; }
         IMessageRepository MessageRepository { get; }
         ILikesRepository LikesRepository { get; }

         Task<bool> Complete();

        // A Helper method that helps us with seeing if EF has been tracking any changes.
         bool HasChanges();

    }
}