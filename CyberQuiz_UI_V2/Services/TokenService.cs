namespace CyberQuiz_UI_V2.Services;

public class TokenService
{
    private string? _token;

    public void SetToken(string token)
    {
        _token = token;
        Console.WriteLine($"[TokenService] Token SET - Length: {token?.Length ?? 0}");
    }

    public string? GetToken()
    {
        Console.WriteLine($"[TokenService] Token GET - Exists: {!string.IsNullOrEmpty(_token)}, Length: {_token?.Length ?? 0}");
        return _token;
    }

    public void ClearToken()
    {
        _token = null;
    }

    public bool HasToken()
    {
        return !string.IsNullOrEmpty(_token);
    }
}
