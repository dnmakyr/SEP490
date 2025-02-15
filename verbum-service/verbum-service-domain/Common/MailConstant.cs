namespace verbum_service_domain.Common
{
    public static class MailConstant
    {
        public const string CONFIRM_EMAIL_HEADER = "Hello!, please confirm your email!!";
        public const int MailExpirationTime = 1; //hour
        public const string REJECT_ORDER_HEADER = "Your order {0} has been rejected";
        public const string ACCEPT_ORDER_HEADER = "Your order {0} has been accepted";
        public const string COMPLETE_ORDER_HEADER = "Your order {0} has been completed";
        public const string REJECT_JOB_HEADER = "Your job has been rejected";
        public const string ACCEPT_JOB_HEADER = "Your job has been accepted";
        public const string REJECT_ISSUE_HEADER = "Your issue {0} has been rejected";
        public const string ACCEPT_ISSUE_HEADER = "Your issue {0} has been accepted";
        public const string CANCEL_ISSUE_HEADER = "Your issue {0} has been cancelled";
    }
}
