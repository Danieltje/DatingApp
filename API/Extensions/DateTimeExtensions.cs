using System;

namespace API.Extensions
{
    // we always make an extension method static
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dob)
        {
            // writing the logic to calculate someone's age
            var today = DateTime.Today;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}