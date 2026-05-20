using zuli_Business.DTO;

namespace zuli_Business.Validation.Strategies
{
    public class FlightRouteBusinessIdStrategy : IValidationStrategy<FlightRouteDTO>
    {
        public Task ExecuteAsync(FlightRouteDTO dto, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(dto.businessId))
                context.AddError(FlightRouteAtributes.BUSINESS_ID,
                    "Es necesario ingresar el Id de negocio");
            return Task.CompletedTask;
        }
    }
}
