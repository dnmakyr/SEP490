using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.Common
{
    public static class SystemConfig
    {
        public const string CONNECTION_STRING = "";
        public const int ACCESS_TOKEN_LIFE = 5; //hour
        public const int REFRESH_TOKEN_LIFE = 14; //days
        public const string DOMAIN = "http://localhost:8000/api/auth";
        public const string BE_DOMAIN = "http://localhost:8000/api";
        public const string FE_DOMAIN = "http://localhost:3000";
    }
}
