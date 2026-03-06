namespace CyberQuiz_UI_V2.Services;

public class TokenService
{
    private string? _token;

    public void SetToken(string token)
    {
        _token = token;
    }

    public string? GetToken()
    {
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
