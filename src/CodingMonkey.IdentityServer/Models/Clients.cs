namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;
    using System.IO;

    using IdentityServer4.Models;

    static class Clients
    {
        public static List<Client> Get(string secretsPath)
        {
            const string ClientSecretsFileName = "client.secrets.json";
            string clientSecretsPath = Path.Combine(secretsPath, ClientSecretsFileName);
            var clients = new Utility().GetListFromConfig<Client>(clientSecretsPath);

            // Client secrets must be hashed as Sha256
            // https://github.com/IdentityServer/IdentityServer3/issues/741
            foreach (var client in clients)
            {
                foreach (var secret in client.ClientSecrets)
                {
                    secret.Value = secret.Value.Sha256();
                }
            }

            return clients;
        }
    }
}
