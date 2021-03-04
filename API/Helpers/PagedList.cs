using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    /* Making this class generic; it can take any type of Entity, but those types are wrapped in a List
       The type gets swapped in compile time depending on what we use. PagedList is of type List, we inherit that from List
     */
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;

            // For example; if we have a total count of 10 items, and we have a pagesize of 5, we have 2 pages that get calculated
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;

            // Add the items into this class, so when we create a new instance of this. We do that in the CreateAsync method
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // Creating a static method that we can call from anywhere.
        // This is going to receive our IQueryable/query, where we work out the pagination information

        // A static method that creates a new instance of this class which we then return with all the calculated properties
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber,
            int pageSize)
        {
            // This makes a database call and is unavoidable
            var count = await source.CountAsync();

            // If you are on page number 1 for instance. 1-1 = 0. Means we will Skip(0) records, and take 5 for the pageSize
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}