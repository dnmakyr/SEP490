using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.Common.ErrorModel
{
    public class ValidationAlertCode
    {
        public const string REQUIRED = "{0} is required";
        public const string FAILED_VALIDATION = "{0} failed validation";
        public const string LENGTH_RANGE_FAILED = "{0} length is from {1} to {2}";
        public const string NOT_FOUND = "{0} is not found in database";
        public const string DUPLICATE = "{0} is already in database";
        public const string INVALID = "{0} is invalid";
        public const string EMAIL_NOT_VERIFIED = "Email is not verified";
        public const string EMAIL_EXPIRED = "The email verification email token might be expired. Please check your email again";
        public const string LOGIN_FAIL = "Login fail";
        public const string UPDATE_RECORD_FAIL = "No record affected";
        public const string CANNOT_DELETE = "Cannot delete this {0}";
        public const string CANNOT_UPDATE = "Cannot update this {0}";
        public const string ISSUE_CREATE_WHEN_ORDER_NOT_COMPLETED = "Cannot create issue when order is not completed";
    }
}
