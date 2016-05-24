using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingMonkey.IdentityServer.Models
{
    using System.IO;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Internal;
    using Microsoft.Extensions.PlatformAbstractions;

    using Newtonsoft.Json;

    internal class Utility
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public Utility()
        {
            this.hostingEnvironment = new HostingEnvironment();
        }

        internal List<T> GetListFromConfig<T>(string filePath)
        {
            string jsonContent;

            try
            {
                jsonContent = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Could not read the json file.", ex);
            }

            return JsonConvert.DeserializeObject<List<T>>(jsonContent);
        }
    }
}
