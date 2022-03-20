namespace WebShopAdminApplication
{
    public static class HttpUtils
    {
        public static bool GetIsAuth(HttpContext httpContext)
        {
            var str = httpContext.Session.GetString("IsAuth");
            bool.TryParse(str, out bool isAuth);
            return isAuth;
        }

        public static void SetIsAuth(HttpContext httpContext, bool isAuth = true)
        {
            httpContext.Session.SetString("IsAuth", isAuth.ToString());
        }
    }
}
