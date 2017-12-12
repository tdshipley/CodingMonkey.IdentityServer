namespace CodingMonkey.IdentityServer.Models
{
    using System.IO;
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class Utility
    {
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
