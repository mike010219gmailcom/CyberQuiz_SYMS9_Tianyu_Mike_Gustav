using System.Net.Http.Headers;

namespace CyberQuiz_UI_V2.Services;

public class TokenHttpMessageHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public TokenHttpMessageHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _tokenService.GetToken();

        // DEBUG: Log token status
        Console.WriteLine($"[TokenHandler] Token exists: {!string.IsNullOrEmpty(token)}");
        Console.WriteLine($"[TokenHandler] Token length: {token?.Length ?? 0}");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"[TokenHandler] Authorization header added");
        }
        else
        {
            Console.WriteLine($"[TokenHandler] NO TOKEN - Authorization header NOT added!");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
