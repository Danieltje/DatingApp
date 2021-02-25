using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }


        // when trying to get the not-found endpoint it returns a 404 Not Found error
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            // We try to find something we know doesn't exist to get an exception
            // There's not a user with id -1
            var thing = _context.Users.Find(-1);

            if (thing == null) return NotFound();

            return Ok(thing);
        }


        // when trying to execute a get request to this endpoint we get a NullReferenceException
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
                // the find method attempts to find an entity with the given parameter -1
                // if it's found its attached to the context and is returned
                // if nothing is found then null is returned
                var thing = _context.Users.Find(-1);

                // thingToReturn will be null. When we try to execute a method on null, we
                // will get a NullReferenceException. We're converting it to a string so the
                // controllermethod will need to return an actionresult of type string
                var thingToReturn = thing.ToString();

                return thingToReturn;
        }


        // trying to execute a get to this endpoint will return 400 Bad Request with the string attached
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
}