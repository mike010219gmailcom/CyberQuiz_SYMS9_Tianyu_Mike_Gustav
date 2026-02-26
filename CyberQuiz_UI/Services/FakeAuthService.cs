using System.Threading.Tasks;

namespace CyberQuiz_UI.Services
{
    // Fake Service
    public class FakeAuthService : IAuthService
    {
        public Task<bool> LoginAsync(string username, string password, bool rememberMe)
        {
            // Seedad user
            if (username == "user" && password == "Password1234!")
            {
                return Task.FromResult(true);
            }

           
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
