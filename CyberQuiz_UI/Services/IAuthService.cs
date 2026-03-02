using System.Threading.Tasks;

namespace CyberQuiz_UI.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password, bool rememberMe);
    }
}
