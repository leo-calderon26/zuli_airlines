using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DapperContext _context;

        public PersonRepository(DapperContext context) => _context = context;

        public async Task<int> CreatePerson(PersonEntity person)
        {
            using var connection = _context.CreateConnection();
            var sql = "dbo.sp_UpsertPerson";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                person.FirstName,
                person.FirstLastName,
                person.SecondLastName,
                person.BirthDate,
                person.Gender,
                person.Email
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task CreatePassport(PassportEntity passport)
        {
            using var connection = _context.CreateConnection();
            var checkSql = @"
                SELECT COUNT(1) FROM Passport
                WHERE PassengerId = @PassengerId AND PassportCountry = @PassportCountry;";

            var exists = await connection.ExecuteScalarAsync<int>(checkSql, new
            {
                passport.PassengerId,
                passport.PassportCountry
            });

            if (exists > 0) return;

            var sql = @"
                INSERT INTO Passport (PassengerId, DueDate, PassportCountry)
                VALUES (@PassengerId, @DueDate, @PassportCountry);";

            await connection.ExecuteAsync(sql, new
            {
                passport.PassengerId,
                passport.DueDate,
                passport.PassportCountry
            });
        }

        public async Task<PersonEntity?> GetPersonByEmail(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT p.PersonId, p.FirstName, p.FirstLastName, p.SecondLastName, p.BirthDate, p.Gender
                FROM Person p
                INNER JOIN PersonEmail pe ON p.PersonId = pe.PersonId
                WHERE pe.Email = @email";
            return await connection.QuerySingleOrDefaultAsync<PersonEntity>(sql, new { email });
        }
    }
}
