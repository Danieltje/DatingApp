using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        // inject the interface we get from AutoMapper inside here
        public UsersController(IUserRepository userRepository, IMapper mapper,
            IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // api/users
        /* We have set-up an AutoMapper and Member/Photo Dto so will apply those here now */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // returning a member Dto directly from our repository
            return await _userRepository.GetMemberAsync(username);
        }

        // We use an Http PUT method to update a resource in our server
        // The client has all the data related to the entity we want to update
        // We don't need to return the object back from our API
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            /* Find the claim that matches the name identifier. That's the claim we give the user in their token
               This should give us the username from the token

               In section 11 we made the GetUsername method to encapsulate the code
            */
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            // this saves us manually mapping between our updatedto and user object
            _mapper.Map(memberUpdateDto, user);

            // here the user object gets flagged as being updated by EF
            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }

        // Using a Http POST method because we are creating a new resource
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // Getting our user, we're eagerly loading them with the Get..Async method
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // We get our result back from the photoService
            var result = await _photoService.AddPhotoAsync(file);

            // Check and see the result; if we get an Error we're returning a Bad Request
            if (result.Error != null) return BadRequest(result.Error.Message);

            // If no error, create a new Photo
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // Is it the first photo the user is uploading? Make it IsMain photo true
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            // We Add the photo
            user.Photos.Add(photo);

            // Returning the photo
            if (await _userRepository.SaveAllAsync())
            // We don't want to return here immediately, we need to wrap this in something
            {
               // Now we will return a 201, that's the Created response
                return CreatedAtRoute("GetUser", new {Username = user.UserName} ,_mapper.Map<PhotoDto>(photo));
               
            }
                
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // We have access to the Photos because we do eager loading inside the GetUserByUsernameAsync
            // We already went into the database in the userRepository, here we already have the users so we search in it
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");


            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            // Turn off the currentMain photo
            if (currentMain != null) currentMain.IsMain = false;

            // Turn on the new Main photo we want to set in this method
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to set main photo");
        }
    }
}