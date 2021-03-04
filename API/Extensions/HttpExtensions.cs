using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    // Remember because it's an Extension method we give it a static property
    public static class HttpExtensions
    {
        // Not returning anything because we just add the header to our response/request
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
        {
            // Creating a new paginationHeader. The parameters need to be in this exact order.
            // We made this class in the Helper map
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // We dont need to say X-Pagination anymore, but we do need to serialize it as Json. It takes a key and a string value
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));

            // Because we're adding a custom header, we need to add a Cors() header to make this header available
            // The Access.. part has to be spelled like that. Pagination also needs to match the name you specified above
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}