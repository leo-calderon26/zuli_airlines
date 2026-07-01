namespace zuli_Business.Interface
{
    public interface IQrCodeService
    {
        byte[] Generate(string data);
    }
}
