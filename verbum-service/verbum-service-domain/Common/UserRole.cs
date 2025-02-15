using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.Common
{
    public static class UserRole
    {
        public const string ADMIN = "ADMIN";
        public const string LINGUIST = "LINGUIST";
        public const string TRANSLATE_MANAGER = "TRANSLATE_MANAGER";
        public const string EDIT_MANAGER = "EDIT_MANAGER";
        public const string EVALUATE_MANAGER = "EVALUATE_MANAGER";
        public const string CLIENT = "CLIENT";
        public const string DIRECTOR = "DIRECTOR";
        public const string STAFF = "STAFF";
        public const string MANAGER = TRANSLATE_MANAGER + "," +EDIT_MANAGER + "," + EVALUATE_MANAGER;
    }
}
