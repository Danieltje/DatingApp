using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    /* When we're adding (Exception)middleware to our .NET API, we need to do certain things
       - Add a constructor for this
       - A request delegate; what's coming up next in the middleware pipeline
       - Use the ILogger so we can still log out our Exception into the terminal
       - We give it a type of ExceptionMiddleware; our class
       - Want to check what environment we're running in; production/development? Use IHostEnvironment
    */
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        // give the middleware it's required method
        // we give it HttpContext because this is happening in the context of a HTTP request
        public async Task InvokeAsync(HttpContext context) 
        {
            try
            {
                // this piece of middleware lives at the very top
                // We pass the context on to this async task and pass it on to the next piece of middleware
                // anything below this that will all pass _next with the context. If any of them gets an exception it will
                // pass it around till they reach something that can handle the Exception

                // Because this middleware will be at the top of this "passing around", we're gonna catch the Exception
                await _next(context);
            }
            catch (Exception ex)
            {
                // log the error. If we don't do this the Exception is going to be silent/will not show
                _logger.LogError(ex, ex.Message);

                // write out the Exception to our response. Effectively will be a 500 exception
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                // create a response
                var response = _env.IsDevelopment()

                    // what to do when this is development mode
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())

                    // what to do when this NOT in development mode (production mode)
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                // sent back response in JSON
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};   

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}