using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    // Esto son tag pasa saber donde esta el error
    public static class AircraftAtributes
    {
        public const string BUSINESSID = "BusinessId";
        public const string ECONOMYROWS = "EconomyRows";
        public const string SEATINGECONOMY = "SeatingRowsEconomy";
        public const string FIRSTROWS = "FirstRows";
        public const string SEATINGFIRST = "SeatingRowsFirst";
        public const string MODEL = "Model";
        public const string WEIGHT = "Weight";
        public const string BAGGAGECAPACITY = "BaggageCapacity";
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

            RuleFor(x => x.numberEconomyClassRows)
                .GreaterThanOrEqualTo((short)0)
                .WithMessage("La cantidad de filas de clase economica tiene que ser un numero positivo")
                .OverridePropertyName(AircraftAtributes.ECONOMYROWS);

            RuleFor(x => x.numberSeatingRowsEconomy)
                .GreaterThanOrEqualTo((short)0)
                .WithMessage("La cantidad de asientos por fila de la clase economica tiene que ser un numero positivo")
                .OverridePropertyName(AircraftAtributes.SEATINGECONOMY);

            RuleFor(x => x.numberFirstClassRows)
                .GreaterThanOrEqualTo((short)0)
                .WithMessage("La cantidad de filas de primera clase tiene que ser un numero positivo")
                .OverridePropertyName(AircraftAtributes.FIRSTROWS);

            RuleFor(x => x.numberSeatingRowsFirst)
                .GreaterThanOrEqualTo((short)0)
                .WithMessage("La cantidad de asientos por fila de primera clase tiene que ser un numero positivo")
                .OverridePropertyName(AircraftAtributes.SEATINGFIRST);

            RuleFor(x => x.weight)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("El peso tiene que ser un numero positivo")
                .OverridePropertyName(AircraftAtributes.WEIGHT);

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
