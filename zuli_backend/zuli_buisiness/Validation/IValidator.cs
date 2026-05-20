namespace zuli_Business.Validation
{
    public interface IValidator<T>
    {
        Task ValidateAsync(T dto);
    }
}
