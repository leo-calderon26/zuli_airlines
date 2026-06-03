namespace zuli_Data
{
    public record PassengerCheckInfo(
        int Index,
        string FirstName,
        string FirstLastName,
        string SecondLastName,
        string BirthDate,
        string PassportCountry
    );
}