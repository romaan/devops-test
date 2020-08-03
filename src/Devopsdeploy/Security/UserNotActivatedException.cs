using System.Security.Authentication;

namespace DevOpsDeploy.Security {
    public class UserNotActivatedException : AuthenticationException {
        public UserNotActivatedException(string message) : base(message)
        {
        }
    }
}
