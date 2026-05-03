using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    // Esto son tag pasa saber donde esta el error
    public static class AircraftAtributes
    {
        public const string ECONOMYROWS = "EconomyRows";
        public const string SEATINGECONOMY = "SeatingRowsEconomy";
        public const string FIRSTROWS = "FirstRows";
        public const string SEATINGFIRST = "SeatingRowsFirst";
        public const string MODEL = "Model";
        public const string WEIGHT = "Weight";
        public const string BAGGAGECAPACITY = "BaggageCapacity";
    }
    public class AircraftValidator
    {
        public void ValidateAircraftInfo(AircraftDTO aircraft)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                {AircraftAtributes.ECONOMYROWS, new List<string>() },
                {AircraftAtributes.SEATINGECONOMY, new List<string>() },
                {AircraftAtributes.FIRSTROWS, new List<string>() },
                {AircraftAtributes.SEATINGFIRST, new List<string>() },
                {AircraftAtributes.MODEL, new List<string>() },
                {AircraftAtributes.WEIGHT, new List<string>() },
                {AircraftAtributes.BAGGAGECAPACITY, new List<string>() },
            };

            var IsEmptyInfo = false;
            //Validar si el modelo no tiene espacios en blanco
            if (string.IsNullOrWhiteSpace(aircraft.model))
            {
                errorInfo[AircraftAtributes.MODEL].Add("El nombre del modelo no puede traer espacios en blanco");
                IsEmptyInfo = true;
            }
            //Validar que el modelo solo pueda traer numeros y letras
            
            if (!MiscValidor.ContainsNumbersAndChars(aircraft.model) && !string.IsNullOrWhiteSpace(aircraft.model))
            {
                errorInfo[AircraftAtributes.MODEL].Add("El nombre del modelo no puede tener caractes especiales");
                IsEmptyInfo = true;
            }
            if (aircraft.numberEconomyClassRows < 0)
            {
                errorInfo[AircraftAtributes.ECONOMYROWS].Add("La cantidad de filas de clase economica tiene que ser un numero positivo");
                IsEmptyInfo = true;
            }
            if (aircraft.numberSeatingRowsEconomy < 0) {
                errorInfo[AircraftAtributes.SEATINGECONOMY].Add("La cantidad de asientos por fila de la clase economica tiene que ser un numero positivo");
                IsEmptyInfo = true;
            }
            if (aircraft.numberFirstClassRows < 0)
            {
                errorInfo[AircraftAtributes.FIRSTROWS].Add("La cantidad de filas de primera clase tiene que ser un numero positivo");
                IsEmptyInfo = true;
            }
            if (aircraft.numberSeatingRowsFirst < 0)
            {
                errorInfo[AircraftAtributes.SEATINGFIRST].Add("La cantidad de asientos por fila de primera clase tiene que ser un numero positivo");
                IsEmptyInfo = true;
            }
            // Validar que el peso sea un valor numerico
            if (aircraft.weight < 0)
            {
                errorInfo[AircraftAtributes.WEIGHT].Add("El peso tiene que ser un numero positivo");
                IsEmptyInfo = true;
            }
            if (aircraft.baggageCapacity < 0)
            {
                errorInfo[AircraftAtributes.BAGGAGECAPACITY].Add("La capacidad de equipaje tiene que ser un numero positivo");
                IsEmptyInfo = true;
            }
            if (IsEmptyInfo)
            {
                throw new ZuliValidationException(errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value));
            }
            else
            {
                //Validar que el modelo no puede medir mas de 15 chars
                if (aircraft.model.Length >= 16)
                {
                    errorInfo[AircraftAtributes.MODEL].Add("El nombre del modelo no puede medir más de 15 caracteres");
                    IsEmptyInfo = true;
                }
                if (TotalSeats(aircraft) > 1000)
                {
                    errorInfo[AircraftAtributes.MODEL].Add("La cantidad de asientos no puede ser mayor que a 1000 asientos");
                    IsEmptyInfo = true;
                }
                if (aircraft.baggageCapacity > (aircraft.weight * 0.30m))
                {
                    errorInfo[AircraftAtributes.BAGGAGECAPACITY].Add("La capacidad de equipaje no puede ser mayor al 30% del peso de la aeronave");
                    IsEmptyInfo = true;
                }
                if (aircraft.baggageCapacity < (aircraft.weight * 0.30m))
                {
                    errorInfo[AircraftAtributes.BAGGAGECAPACITY].Add("La capacidad de equipaje no puede ser menor al 30% del peso de la aeronave");
                    IsEmptyInfo = true;
                }
                if (errorInfo.Any(x => x.Value.Count > 0))
                    throw new ZuliValidationException(errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value));
            }
        }
        public int TotalSeats(AircraftDTO aircraft)
        {
            var totalFirst = aircraft.numberSeatingRowsFirst * aircraft.numberFirstClassRows;
            var totalEconomy = aircraft.numberEconomyClassRows * aircraft.numberSeatingRowsEconomy;
            return totalEconomy + totalFirst;
        }
    }
}
