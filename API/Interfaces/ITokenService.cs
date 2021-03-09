using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    // an interface summarized is a contract between itself and any class that implements it
    // this "contract" states that any class that implements this interface will implement the properties, methods, or events in this TokenService interface
    // an interface cannot contain implementation logic. It can only contain the signatures of the functionality the interface provides

    // in c# we always prefix interface with a capital I
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}