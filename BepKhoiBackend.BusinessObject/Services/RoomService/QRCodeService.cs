using QRCoder;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

public class QRCodeService
{
    private readonly CloudinaryService _cloudinaryService;

    public QRCodeService(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    public async Task<string> GenerateAndUploadQRCodeAsync(string data)
    {
        try
        {
            // Tạo QR Code
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrBytes = qrCode.GetGraphic(20);

            // Lưu vào file tạm
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"QR_{Guid.NewGuid()}.png");
            await File.WriteAllBytesAsync(tempFilePath, qrBytes);

            // Upload lên Cloudinary
            string qrCodeUrl = await _cloudinaryService.UploadImageAsync(tempFilePath);

            // Xóa file tạm sau khi upload thành công
            File.Delete(tempFilePath);

            return qrCodeUrl;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to generate and upload QR code: {ex.Message}");
        }
    }

    public async Task DeleteQRCodeFromCloudinaryAsync(string qrCodeUrl)
    {
        await _cloudinaryService.DeleteImageAsync(qrCodeUrl);
    }

}
