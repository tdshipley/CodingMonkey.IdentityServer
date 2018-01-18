namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;
    using System.IO;

    using IdentityServer4.Models;

    static class ApiResources
    {
        public static List<ApiResource> Get(string secretsPath)
        {
            const string ApiSecretsFileName = "apiresource.secrets.json";
            string apiSecretsPath = Path.Combine(secretsPath, ApiSecretsFileName);
            var apis = new Utility().GetListFromConfig<ApiResource>(apiSecretsPath);

            // Client secrets must be hashed as Sha256
            // https://github.com/IdentityServer/IdentityServer3/issues/741
            foreach (var api in apis)
            {
                foreach (var secret in api.ApiSecrets)
                {
                    secret.Value = secret.Value.Sha256();
                }
            }

            return apis;
        }
    }
}
