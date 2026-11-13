using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserManagement.Web.Constants
{
    public class HttpStatusCodes
    {
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int InternalServerError = 500;
    }
}
