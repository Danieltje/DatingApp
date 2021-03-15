using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /* Controller instead of BaseApiController. Controller is a base class for an MVC controller with view support.
       Our Angular app is the view for our application. We're gonna tell this FallbackController what file to serve.
       We'll tell the API what to do with any route it doesn't understand. In theory we send the user to our index.html.
       That's where our Angular app is served from, and ng knows what to do with those routes here.
     */
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
            // Returning the physical file; index.html; if our API doesn't know what to do with a route it falls back to this controller.
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}