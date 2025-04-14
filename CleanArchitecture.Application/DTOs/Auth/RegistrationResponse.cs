using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class RegistrationResponse
    {
        public bool IsSuccessful { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string ConfirmationLink { get; set; }
        public string ConfirmationToken { get; set; }
        public string ConfirmationLinkWithoutCreds { get; set; }
    }
}
