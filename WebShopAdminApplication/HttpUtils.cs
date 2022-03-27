namespace WebShopAdminApplication
{
    public static class HttpUtils
    {
        public static bool GetIsAuth()
        {
            var str = GetHttpContext().Session.GetString("IsAuth");
            bool.TryParse(str, out bool isAuth);
            return isAuth;
        }

        public static void SetIsAuth(bool isAuth = true)
        {
            GetHttpContext().Session.SetString("IsAuth", isAuth.ToString());
        }

        public static string GetToken()
        {
            var token = GetHttpContext().Session.GetString("Token");
            return token;
        }

        public static void SetToken(string token)
        {
            GetHttpContext().Session.SetString("Token", token);
        }

        public static void ResetToken()
        {
            GetHttpContext().Session.Remove("Token");
        }

        private static HttpContext GetHttpContext()
        {
            var httpContext = new HttpContextAccessor().HttpContext;
            if (httpContext == null) throw new Exception("HttpContext is null");
            return httpContext;
        }
    }
}
