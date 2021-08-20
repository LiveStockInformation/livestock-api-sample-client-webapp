using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace livestock_api_samples.Helpers
{
    public static class MsalAppBuilder
    {

        private static IConfidentialClientApplication clientapp;


        /// <summary>
        /// Shared method to create an IConfidentialClientApplication from configuration and attach the application's token cache implementation
        /// </summary>
        /// <param name="currentUser">The current ClaimsPrincipal</param>
        /// <returns></returns>
        public static IConfidentialClientApplication BuildConfidentialClientApplication(ClaimsPrincipal currentUser,AzureAdB2COptions options)
        {
            if (clientapp != null)
                return clientapp;

                 clientapp = ConfidentialClientApplicationBuilder.Create(options.ClientId)
                  .WithClientSecret(options.ClientSecret)
                  .WithRedirectUri(options.RedirectUri)
                  .WithAuthority(new Uri(options.Authority))
                  .Build();


            // After the ConfidentialClientApplication is created, we overwrite its default UserTokenCache with our implementation
            MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache, currentUser ?? ClaimsPrincipal.Current);
            return clientapp;
        }

        /// <summary>
        /// Common method to remove the cached tokens for the currently signed in user
        /// </summary>
        /// <returns></returns>
        public static async Task ClearUserTokenCache(AzureAdB2COptions options)
        {
            IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(options.ClientId)
                  .WithClientSecret(options.ClientSecret)
                  .WithRedirectUri(options.RedirectUri)
                  .WithAuthority(new Uri(options.Authority))
                  .Build();

            // We only clear the user's tokens.
            MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache);
            var userAccount = await clientapp.GetAccountAsync(ClaimsPrincipal.Current.GetMsalAccountId());

            //Remove the users from the MSAL's internal cache
            await clientapp.RemoveAsync(userAccount);
            userTokenCache.Clear();
        }
    }
}
