using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class EmailConfirmationResponse
    {
        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }
    }
}
