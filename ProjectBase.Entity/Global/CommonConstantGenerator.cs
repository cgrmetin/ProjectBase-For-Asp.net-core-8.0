namespace ProjectBase.Entity.Global
{
    public static class CommonConstantGenerator
    {
        public static class ReourceCode
        {
            public static string ErrorMessage = "ErrorMessage";
            public static string SuccessMessage = "SuccessMessage";
        }

        public static class ResourceKey
        {
            public static string WrongUserName = "WrongUserName";
            public static string PassiveUser = "PassiveUser";
            public static string WrongPassword = "WrongPassword";
            public static string LoginSuccess= "LoginSuccess";
            public static string UserBlockedDuetoTooManyAttempts = "UserBlockedDuetoTooManyAttempts";
            public static string LoginFailed = "LoginFailed";
            public static string EmailAddressAlreadyRegistered = "EmailAddressAlreadyRegistered";
            public static string RegisteredSuccesfuly = "RegisteredSuccesfuly";
        }

        public static class ApiConstant
        {
            public static string LastError = "LastError";
        }

        public static class CachePrefix
        {
            public static string Languages = "Languages";
        }
        public static class CachceKey
        {
            public static string AllLanguages = "AllLanguages";
        }
    }
}
