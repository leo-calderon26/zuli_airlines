using QRCoder;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] Generate(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            return qrCode.GetGraphic(5);
        }
    }
}