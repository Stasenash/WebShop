using Newtonsoft.Json;

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
            string userHeader =  JsonConvert.SerializeObject(_httpContextAccessor.HttpContext.Items["User"]);
            request.Headers.Add("User", userHeader);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
