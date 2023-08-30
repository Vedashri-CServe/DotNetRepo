﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Auth
{
    public interface IAccessTokenProvider
    {
        TokenValidationVM ValidateToken(HttpRequest request);
    }

}
