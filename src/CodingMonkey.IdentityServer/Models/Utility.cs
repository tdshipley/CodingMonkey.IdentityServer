using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingMonkey.IdentityServer.Models
{
    using System.IO;

    using Microsoft.Extensions.PlatformAbstractions;

    using Newtonsoft.Json;

    internal static class Utility
    {
        internal static List<T> GetListFromConfig<T>(string fileName)
        {
            string clientSecretsPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, fileName);

            string clientsAsJson;

            try
            {
                clientsAsJson = File.ReadAllText(clientSecretsPath);
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Could not read the client secrets file.", ex);
            }

            return JsonConvert.DeserializeObject<List<T>>(clientsAsJson);
        }
    }
}
