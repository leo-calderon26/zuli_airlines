using FluentValidation.Results;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public static class FluentValidationExtensions
    {
        public static void ThrowIfInvalid(this ValidationResult result)
        {
            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key, 
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );

                throw new ZuliValidationException(errors);
            }
        }
    }
}