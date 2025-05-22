using Microsoft.AspNetCore.Components.WebAssembly.Http;
namespace BlazorSSR_IdentityCookie.Middleware;

public class CookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CookieHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// Main method to override for the handler.
    /// </summary>
    /// <param name="request">The original request.</param>
    /// <param name="cancellationToken">The token to handle cancellations.</param>
    /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookie = _httpContextAccessor.HttpContext?.Request.Cookies[".AspNetCore.Cookies"];
        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", $".AspNetCore.Cookies={cookie}");
        }
        // include cookies!
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);
        return base.SendAsync(request, cancellationToken);
    }
}