namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;
    using System.IO;

    using IdentityServer4.Models;

    static class Scopes
    {
        public static List<Scope> Get(string secretsPath)
        {
            const string ScopeSecretsFileName = "scope.secrets.json";
            string scopeSecretsPath = Path.Combine(secretsPath, ScopeSecretsFileName);
            var scopes = new Utility().GetListFromConfig<Scope>(scopeSecretsPath);

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
