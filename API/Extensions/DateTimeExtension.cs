namespace API.Extensions;

/// <summary>
/// Extension methods for date and time calculations
/// </summary>
public static class DateTimeExtension
{
    /// <summary>
    /// Calculates the age based on a date of birth
    /// </summary>
    /// <param name="dob">The date of birth to calculate age from</param>
    /// <returns>The calculated age as an integer</returns>
    public static int CalculateAge(this DateOnly dob)
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        int age = currentDate.Year - dob.Year;
        if (dob>currentDate.AddYears(-age))
        {
            --age;
        }
        return age;
    }
}