using FluentValidation;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Validation
{

    public static class AircraftAtributes
    {
        public const string BUSINESSID = "businessId";
        public const string ECONOMYROWS = "numberEconomyClassRows";
        public const string SEATINGECONOMY = "numberSeatingRowsEconomy";
        public const string FIRSTROWS = "numberFirstClassRows";
        public const string SEATINGFIRST = "numberSeatingRowsFirst";
        public const string MODEL = "model";
        public const string WEIGHT = "weight";
        public const string BAGGAGECAPACITY = "baggageCapacity";
    }
    public class AircraftValidator : AbstractValidator<AircraftDTO>
    {
        public AircraftValidator()
        {

            RuleFor(x => x.businessId)
                .NotEmpty().WithMessage("Es necesario ingresar el Id de negocio")
                .OverridePropertyName(AircraftAtributes.BUSINESSID);

            RuleFor(x => x.model)
                .NotEmpty().WithMessage("El nombre del modelo no puede traer espacios en blanco")
                .OverridePropertyName(AircraftAtributes.MODEL);

            RuleFor(x => x.model)
                .Must(MiscValidor.ContainsNumbersAndChars)
                .When(x => !string.IsNullOrWhiteSpace(x.model))
                .WithMessage("El nombre del modelo no puede tener caractes especiales")
                .OverridePropertyName(AircraftAtributes.MODEL);

            RuleFor(x => x.weight)
                .GreaterThanOrEqualTo(0m).WithMessage("El peso tiene que ser un numero positivo")
                .Must((dto, weight, context) =>
                {
                    var original = GetOriginalAircraft(context);
                    return original == null || (decimal)weight >= original.weight;
                })
                .WithMessage("El peso soportado solo puede ser igual o mayor al valor original")
                .OverridePropertyName(AircraftAtributes.WEIGHT);

            RuleFor(x => x.numberEconomyClassRows)
                .GreaterThanOrEqualTo((short)0).WithMessage("La cantidad de filas de clase economica tiene que ser un numero positivo")
                .Must((AircraftDTO dto, short rows, ValidationContext<AircraftDTO> context) =>
                {
                    var original = GetOriginalAircraft(context);
                    return original == null || (int)rows >= original.numberEconomyClassRows;
                })
                .WithMessage("La cantidad de filas de clase económica solo se puede aumentar")
                .OverridePropertyName(AircraftAtributes.ECONOMYROWS);

            RuleFor(x => x.numberSeatingRowsEconomy)
                .GreaterThanOrEqualTo((short)0).WithMessage("La cantidad de asientos por fila de la clase economica tiene que ser un numero positivo")
                .Must((AircraftDTO dto, short seats, ValidationContext<AircraftDTO> context) =>
                {
                    var original = GetOriginalAircraft(context);
                    return original == null || (int)seats >= original.numberSeatingRowsEconomy;
                })
                .WithMessage("La cantidad de asientos por fila económica solo se puede aumentar")
                .OverridePropertyName(AircraftAtributes.SEATINGECONOMY);

            RuleFor(x => x.numberFirstClassRows)
                .GreaterThanOrEqualTo((short)0).WithMessage("La cantidad de filas de primera clase tiene que ser un numero positivo")
                .Must((AircraftDTO dto, short rows, ValidationContext<AircraftDTO> context) =>
                {
                    var original = GetOriginalAircraft(context);
                    return original == null || (int)rows >= original.numberFirstClassRows;
                })
                .WithMessage("La cantidad de filas de primera clase solo se puede aumentar")
                .OverridePropertyName(AircraftAtributes.FIRSTROWS);

            RuleFor(x => x.numberSeatingRowsFirst)
                .GreaterThanOrEqualTo((short)0).WithMessage("La cantidad de asientos por fila de primera clase tiene que ser un numero positivo")
                .Must((AircraftDTO dto, short seats, ValidationContext<AircraftDTO> context) =>
                {
                    var original = GetOriginalAircraft(context);
                    return original == null || (int)seats >= original.numberSeatingRowsFirst;
                })
                .WithMessage("La cantidad de asientos por fila de primera clase solo se puede aumentar")
                .OverridePropertyName(AircraftAtributes.SEATINGFIRST);

            RuleFor(x => x.baggageCapacity)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("La capacidad de equipaje tiene que ser un numero positivo")
                .OverridePropertyName(AircraftAtributes.BAGGAGECAPACITY);

            RuleFor(x => x.model)
                .MaximumLength(15)
                .When(x => !string.IsNullOrWhiteSpace(x.model))
                .WithMessage("El nombre del modelo no puede medir más de 15 caracteres")
                .OverridePropertyName(AircraftAtributes.MODEL);

            RuleFor(x => x)
                .Must(HasValidSeatCount)
                .WithMessage("La cantidad de asientos no puede ser mayor que a 1000 asientos")
                .OverridePropertyName(AircraftAtributes.MODEL);

            RuleFor(x => x)
                .Must(HasValidBaggageCapacity)
                .WithMessage("La capacidad de equipaje no puede ser mayor al 30% del peso de la aeronave")
                .OverridePropertyName(AircraftAtributes.BAGGAGECAPACITY);
        }

        private AircraftEntity? GetOriginalAircraft(ValidationContext<AircraftDTO> context)
        {
            if (context.RootContextData.TryGetValue("OriginalAircraft", out var original))
            {
                return original as AircraftEntity;
            }
            return null;
        }

        private static bool HasValidSeatCount(AircraftDTO aircraft)
        {
            return TotalSeats(aircraft) <= 1000;
        }

        private static bool HasValidBaggageCapacity(AircraftDTO aircraft)
        {
            return aircraft.baggageCapacity == aircraft.weight * 0.30m;
        }

        private static int TotalSeats(AircraftDTO aircraft)
        {
            var totalFirst = aircraft.numberSeatingRowsFirst * aircraft.numberFirstClassRows;
            var totalEconomy = aircraft.numberEconomyClassRows * aircraft.numberSeatingRowsEconomy;
            return totalEconomy + totalFirst;
        }
    }
}
