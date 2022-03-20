namespace WebShopAdminGateway.Db
{
    public class RegisterUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AuthUserRequest 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
