using System.Security.Claims;

namespace DevOpsDeploy.Test.Setup {
    public class MockClaimsPrincipalProvider {
        public MockClaimsPrincipalProvider(ClaimsPrincipal user)
        {
            User = user;
        }

        public ClaimsPrincipal User { get; }
    }
}
