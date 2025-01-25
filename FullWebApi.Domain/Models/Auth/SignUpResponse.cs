using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullWebApi.Domain.Models.Auth;

    public class SignUpResponse
    {
        public string Token { get; set; } = string.Empty;
    }

