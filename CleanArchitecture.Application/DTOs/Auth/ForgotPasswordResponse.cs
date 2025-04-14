using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class ForgotPasswordResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
