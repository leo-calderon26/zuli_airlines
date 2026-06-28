using System.Data;
using Dapper;
using zuli_Data;
using zuli_Data.Enums;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class ReservationCancellationRepository : DapperRepository, IReservationCancellationRepository
    {
        public ReservationCancellationRepository(DapperContext context)
            : base(context)
        {
        }

        public async Task<(string? BuyerEmail, string? BuyerName, int ReservationStatusId, int ReservationId)?>
            GetCancellationInfoAsync(string reservationCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                const string sql = @"
                    SELECT TOP (1)
                        personEmail.Email AS BuyerEmail,
                        CONCAT(
                            person.FirstName,
                            ' ',
                            person.FirstLastName
                        ) AS BuyerName,
                        reservation.ReservationStatusId,
                        reservation.ReservationId
                    FROM dbo.Reservation reservation
                    INNER JOIN dbo.Buyer buyer
                        ON reservation.BuyerId = buyer.BuyerId
                    INNER JOIN dbo.Person person
                        ON buyer.PersonId = person.PersonId
                    LEFT JOIN dbo.PersonEmail personEmail
                        ON personEmail.PersonId = person.PersonId
                    WHERE reservation.ReservationCode = @ReservationCode;";

                var row = await connection.QueryFirstOrDefaultAsync(
                    sql,
                    new
                    {
                        ReservationCode = reservationCode
                    }
                );

                if (row == null)
                {
                    return ((string? BuyerEmail, string? BuyerName, int ReservationStatusId, int ReservationId)?)null;
                }

                return (
                    (string?)row.BuyerEmail,
                    (string?)row.BuyerName,
                    (int)row.ReservationStatusId,
                    (int)row.ReservationId
                );
            });
        }

        public async Task<int> SetCancellationTokenAsync(
            string reservationCode,
            string tokenHash,
            DateTime requestedAt,
            DateTime expiresAt)
        {
            return await WithConnectionAsync(async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    "dbo.sp_RequestReservationCancellation",
                    new
                    {
                        ReservationCode = reservationCode,
                        TokenHash = tokenHash,
                        RequestedAt = requestedAt,
                        ExpiresAt = expiresAt
                    },
                    commandType: CommandType.StoredProcedure
                );
            });
        }

        public async Task<CancellationConfirmationResult> ConfirmCancellationAsync(
            string tokenHash)
        {
            return await WithConnectionAsync(async connection =>
            {
                int result = await connection.QueryFirstOrDefaultAsync<int>(
                    "dbo.sp_ConfirmReservationCancellation",
                    new
                    {
                        TokenHash = tokenHash
                    },
                    commandType: CommandType.StoredProcedure
                );

                return (CancellationConfirmationResult)result;
            });
        }
    }
}