namespace zuli_Business.Validation.Strategies
{
    public interface IValidationStrategy<T>
    {
        Task ExecuteAsync(T dto, ValidationContext context);
    }
}
