using System.Security.Authentication;

namespace DevOpsDeploy.Security {
    public class UsernameNotFoundException : AuthenticationException {
        public UsernameNotFoundException(string message) : base(message)
        {
        }
    }
}
