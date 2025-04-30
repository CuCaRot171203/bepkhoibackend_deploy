using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.LoginService.Interface
{
    public interface IOtpService
    {
        string GenerateOtp(string email);
        bool VerifyOtp(string email, string otp);
        void RemoveOtp(string email);
    }


}
