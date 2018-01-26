namespace CodingMonkey.IdentityServer.Models
{
    using System.Collections.Generic;
    using IdentityServer4.Models;

    static class GetIdentityResources
    {
        public static List<IdentityResource> Get()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
    }
}