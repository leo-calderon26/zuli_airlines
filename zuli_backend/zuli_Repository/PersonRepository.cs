using System.Data;
using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class PersonRepository : DapperRepository, IPersonRepository
    {
        public PersonRepository(DapperContext context) : base(context)
        {
        }

        public async Task<PersonEntity?> GetPersonByEmail(string email)
        {
            return await WithConnectionAsync(async (connection) =>
            {
                var sql = @"
                    SELECT p.PersonId, p.FirstName, p.FirstLastName, p.SecondLastName, p.BirthDate, p.Gender
                    FROM Person p
                    INNER JOIN PersonEmail pe ON p.PersonId = pe.PersonId
                    WHERE pe.Email = @email";
                return await connection.QuerySingleOrDefaultAsync<PersonEntity>(sql, new { email });
            });
        }

        public async Task<List<PersonBulkResult>> CreatePersonBulk(
            List<PersonEntity> persons,
            List<PassportEntity> passports,
            BuyerEntity buyer)
        {
            return await WithConnectionAsync(async (connection) =>
            {
                var table = new DataTable();
                table.Columns.Add("RowIndex", typeof(int));
                table.Columns.Add("FirstName", typeof(string));
                table.Columns.Add("FirstLastName", typeof(string));
                table.Columns.Add("SecondLastName", typeof(string));
                table.Columns.Add("BirthDate", typeof(string));
                table.Columns.Add("Gender", typeof(string));
                table.Columns.Add("Email", typeof(string));
                table.Columns.Add("PassportCountry", typeof(string));
                table.Columns.Add("PassportDueDate", typeof(DateTime));
                table.Columns.Add("IsBuyer", typeof(bool));
                table.Columns.Add("Phone", typeof(string));

                for (var i = 0; i < persons.Count; i++)
                {
                    var passport = passports[i];
                    table.Rows.Add(
                        i,
                        persons[i].FirstName,
                        persons[i].FirstLastName,
                        persons[i].SecondLastName,
                        persons[i].BirthDate,
                        persons[i].Gender,
                        persons[i].Email ?? (object)DBNull.Value,
                        passport.PassportCountry,
                        passport.DueDate,
                        false,
                        DBNull.Value
                    );

                }
                if (buyer != null)
                {
                    table.Rows.Add(
                        persons.Count,
                        buyer.FirstName,
                        buyer.FirstLastName,
                        buyer.SecondLastName,
                        buyer.BirthDate,
                        "",
                        buyer.Email ?? (object)DBNull.Value,
                        "",
                        DBNull.Value,
                        true,
                        buyer.Phone ?? (object)DBNull.Value
                    );
                }
                var parameters = new DynamicParameters();
                parameters.Add("Persons", table.AsTableValuedParameter("dbo.PersonBulkType"));

            var result = await connection.QueryAsync<PersonBulkResult>(
                "dbo.sp_UpsertPersonBulk",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
            });
        }
    }
}
