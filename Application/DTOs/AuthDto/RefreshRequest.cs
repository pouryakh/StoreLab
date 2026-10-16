using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthDto
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
