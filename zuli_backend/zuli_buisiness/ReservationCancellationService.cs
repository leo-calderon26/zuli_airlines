using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using zuli_Business.DTO;
using zuli_Business.DTO.Cancellation;
using zuli_Business.Interface;
using zuli_Data.Enums;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class ReservationCancellationService : IReservationCancellationService
    {
        private const int CANCELLATION_TOKEN_EXPIRATION_HOURS = 24;

        private readonly IReservationCancellationRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ReservationCancellationService(
            IReservationCancellationRepository repository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _repository = repository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<BasicResponseDTO> RequestCancellationAsync(
            RequestCancellationRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.ReservationCode) ||
                request.ReservationCode.Trim().Length != 8)
            {
                throw new ZuliValidationException(
                    "reservationCode",
                    "El código de reserva debe tener exactamente 8 caracteres."
                );
            }

            string reservationCode = request.ReservationCode.Trim();

            var cancellationInfo = await _repository.GetCancellationInfoAsync(
                reservationCode
            );

            if (cancellationInfo == null)
            {
                throw new ZuliNotFoundException(
                    "No se encontró la reserva."
                );
            }

            var (
                buyerEmail,
                buyerName,
                reservationStatusId,
                _
            ) = cancellationInfo.Value;

            if (reservationStatusId == (int)ReservationStatus.Cancelled)
            {
                return new BasicResponseDTO
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "La reserva ya se encuentra cancelada."
                };
            }

            if (reservationStatusId != (int)ReservationStatus.Active)
            {
                throw new ZuliValidationException(
                    "reservationCode",
                    "La reserva no se encuentra disponible para cancelación."
                );
            }

            if (string.IsNullOrWhiteSpace(buyerEmail))
            {
                throw new ZuliValidationException(
                    "buyerEmail",
                    "No fue posible solicitar la cancelación porque el comprador no tiene un correo registrado."
                );
            }

            string token = GenerateSecureToken();
            string tokenHash = HashToken(token);

            DateTime requestedAt = DateTime.UtcNow;
            DateTime expiresAt = requestedAt.AddHours(
                CANCELLATION_TOKEN_EXPIRATION_HOURS
            );

            int affectedRows = await _repository.SetCancellationTokenAsync(
                reservationCode,
                tokenHash,
                requestedAt,
                expiresAt
            );

            if (affectedRows != 1)
            {
                throw new ZuliValidationException(
                    "reservationCode",
                    "No fue posible solicitar la cancelación de la reserva."
                );
            }

            string confirmationLink = BuildCancellationLink(token);

            await _emailService.SendCancellationRequestEmailAsync(
                buyerEmail,
                buyerName ?? string.Empty,
                confirmationLink
            );

            return new BasicResponseDTO
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Se enviaron instrucciones al correo del comprador para completar la cancelación."
            };
        }

        public async Task<BasicResponseDTO> ConfirmCancellationAsync(
            ConfirmCancellationRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                throw new ZuliValidationException(
                    "token",
                    "El token de cancelación es requerido."
                );
            }

            string tokenHash = HashToken(request.Token);

            CancellationConfirmationResult result =
                await _repository.ConfirmCancellationAsync(tokenHash);

            if (result != CancellationConfirmationResult.Confirmed)
            {
                throw new ZuliValidationException(
                    "token",
                    "La cancelación no puede procesarse. El enlace es inválido, expiró o ya fue utilizado."
                );
            }

            return new BasicResponseDTO
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "La reserva fue cancelada correctamente."
            };
        }

        private string BuildCancellationLink(string token)
        {
            string frontendBaseUrl = _configuration["Frontend:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "No se ha configurado la URL del frontend."
                );

            return $"{frontendBaseUrl}/cancelar-reserva?token={Uri.EscapeDataString(token)}";
        }

        private static string GenerateSecureToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private static string HashToken(string token)
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
            byte[] hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}