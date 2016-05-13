namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;

    using IdentityServer4.Core.Models;

    static class Scopes
    {
        public static List<Scope> Get()
        {
            const string ScopeSecretsFileName = "scope.secrets.json";
            return Utility.GetListFromConfig<Scope>(ScopeSecretsFileName);
        }
    }
}
