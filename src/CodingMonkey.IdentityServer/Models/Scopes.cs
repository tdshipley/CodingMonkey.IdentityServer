namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;

    using IdentityServer4.Core.Models;

    static class Scopes
    {
        public static List<Scope> Get()
        {
            const string ScopeSecretsFileName = "scope.secrets.json";

            var scopes = Utility.GetListFromConfig<Scope>(ScopeSecretsFileName);

            // Client secrets must be hashed as Sha256
            // https://github.com/IdentityServer/IdentityServer3/issues/741
            foreach (var scope in scopes)
            {
                foreach (var secret in scope.ScopeSecrets)
                {
                    secret.Value = secret.Value.Sha256();
                }
            }

            return scopes;
        }
    }
}
