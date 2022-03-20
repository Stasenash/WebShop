namespace WebShopAdminGateway
{
    public class FillUserHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FillUserHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("User", _httpContextAccessor.HttpContext.Items["User"] as string);

            //do stuff and optionally call the base handler..
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
