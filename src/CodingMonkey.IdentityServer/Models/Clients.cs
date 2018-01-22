namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    using IdentityServer4.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using System;

    static class Clients
    {
        public static List<Client> Get(string secretsPath, IConfiguration configuration)
        {
            const string ClientSecretsFileName = "client.config.json";
            string clientSecretsPath = Path.Combine(secretsPath, ClientSecretsFileName);
            var clients = new Utility().GetListFromConfig<Client>(clientSecretsPath);

            // Client secrets must be hashed as Sha256
            // https://github.com/IdentityServer/IdentityServer3/issues/741
            foreach (var client in clients)
            {
                try
                {
                    var secret_environment_var_name = $"{client.ClientName.ToUpper()}-CLIENT-SECRET";
                    //var secret = configuration[secret_environment_var_name];
                    var secret = System.Environment.GetEnvironmentVariable(secret_environment_var_name);

                    if(secret == null)
                    {
                        string allEnvVars = string.Empty;
                        foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
                        {
                            string name = (string)env.Key;
                            string value = (string)env.Value;
                            allEnvVars += (System.Environment.NewLine + "{0}={1}", name, value);
                        }

                        throw new Exception($"The secret for the Client is null. Env Var Name: {secret_environment_var_name}, All env vars: {System.Environment.NewLine + allEnvVars}");
                    }

                    client.ClientSecrets.Add(new Secret{
                        Value = secret.Sha256()
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception("There was a problem creating ID Server Clients", ex);
                }
            }

            return clients;
        }
    }
}
