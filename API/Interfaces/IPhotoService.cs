using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces
{
    public interface IPhotoService
    {
         Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

         // every photo/image that gets uploaded to Cloudinary gets a publicId, so to delete one you need to
         // know which Id you need to target for the specific photo
         Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}