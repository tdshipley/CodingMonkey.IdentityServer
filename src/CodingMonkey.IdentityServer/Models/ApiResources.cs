namespace CodingMonkey.IdentityServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using IdentityServer4.Models;
    using Microsoft.Extensions.Configuration;

    static class ApiResources
    {
        public static List<ApiResource> Get(string secretsPath, IConfiguration configuration)
        {
            const string ApiSecretsFileName = "apiresource.config.json";
            string apiSecretsPath = Path.Combine(secretsPath, ApiSecretsFileName);
            var apis = new Utility().GetListFromConfig<ApiResource>(apiSecretsPath);

            // Client secrets must be hashed as Sha256
            // https://github.com/IdentityServer/IdentityServer3/issues/741
            foreach (var api in apis)
            {
                try
                {
                    var secret_environment_var_name = $"{api.Name}_api_secret";
                    var secret = configuration[secret_environment_var_name];

                    if(secret == null)
                    {
                        throw new Exception($"The secret for the API is null. Env Var Name: {secret_environment_var_name}");
                    }

                    api.ApiSecrets.Add(new Secret{
                        Value = secret.Sha256()
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception("There was a problem creating ID Server Clients", ex);
                }
            }

            return apis;
        }
    }
}
