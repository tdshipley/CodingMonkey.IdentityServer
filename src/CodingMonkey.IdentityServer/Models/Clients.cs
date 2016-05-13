namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;

    using IdentityServer4.Core.Models;

    static class Clients
    {
        public static List<Client> Get()
        {
            const string ClientSecretsFileName = "client.secrets.json";
            var clients = Utility.GetListFromConfig<Client>(ClientSecretsFileName);

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
