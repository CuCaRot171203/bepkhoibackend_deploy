using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using System;
using System.Collections.Generic;

public class OtpService : IOtpService
{
    private static readonly Dictionary<string, string> _otpStore = new Dictionary<string, string>();
    private readonly Random _random = new Random();

    public string GenerateOtp(string email)
    {
        var otp = _random.Next(100000, 999999).ToString(); // Tạo OTP 6 số
        _otpStore[email] = otp; // Lưu OTP vào dictionary
        return otp;
    }

    public bool VerifyOtp(string email, string otp)
    {
        if (_otpStore.TryGetValue(email, out var storedOtp) && storedOtp == otp)
        {
            RemoveOtp(email); // Xóa OTP sau khi xác minh thành công
            return true;
        }
        return false;
    }

    public void RemoveOtp(string email)
    {
        _otpStore.Remove(email); // Xóa OTP khỏi dictionary sau khi xác minh thành công
    }
}
