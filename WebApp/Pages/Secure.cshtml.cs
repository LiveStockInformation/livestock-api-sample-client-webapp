using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using livestock_api_samples.Helpers;
using System.Security.Cryptography.X509Certificates;

namespace livestock_api_samples.Pages
{
    [Authorize]
    public class SecureModel : PageModel
    {
        private AzureAdB2COptions _azureAdB2COptions;

        public SecureModel(
            IOptions<AzureAdB2COptions> azureAdB2COptions)
        {
            _azureAdB2COptions = azureAdB2COptions.Value;
           
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string responseString = "";

            try
            {
                // Retrieve the token with the specified scopes
                var scope = _azureAdB2COptions.ApiScopes.Split(' ');

                string signedInUserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var cca = MsalAppBuilder.BuildConfidentialClientApplication(HttpContext.User, _azureAdB2COptions);
                var userAccount = await cca.GetAccountAsync(HttpContext.User.GetMsalAccountId());
                if (userAccount == null)
                {
                    // Dealing with guest users
                    var accounts = await cca.GetAccountsAsync();
                    userAccount = accounts.FirstOrDefault();
                }

                var result = await cca.AcquireTokenSilent(scope, userAccount)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                var accessTokenRequest = cca.AcquireTokenSilent(scope, userAccount);
                var accessToken = accessTokenRequest.ExecuteAsync().Result.AccessToken;
                var IdToken = accessTokenRequest.ExecuteAsync().Result.IdToken;
                var tenantId = accessTokenRequest.ExecuteAsync().Result.TenantId;
                var scopes = accessTokenRequest.ExecuteAsync().Result.Scopes;
                var account = accessTokenRequest.ExecuteAsync().Result.Account;

                var httpClient = new HttpClient();
                var request= new HttpRequestMessage(HttpMethod.Get, _azureAdB2COptions.ApiUrl); ;
                HttpResponseMessage response;
                try
                {
                    request = new HttpRequestMessage(HttpMethod.Get, _azureAdB2COptions.ApiUrl);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.Add("Ocp-Apim-Subscription-Key", _azureAdB2COptions.ApiSubscriptionkey);
                    response = await httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }

                ViewData["AuthenticationResultAccessToken"] = accessToken;
                ViewData["AuthenticationResultIdToken"] = IdToken;
                ViewData["AuthenticationResultTenantId"] = tenantId;
                ViewData["AuthenticationResultScopes"] = JsonConvert.SerializeObject(scopes, Formatting.Indented);
                ViewData["AuthenticationResultUser"] = JsonConvert.SerializeObject(account, Formatting.Indented);
                ViewData["AzureAdB2COptionsClientId"] = _azureAdB2COptions.ClientId;
                ViewData["AzureAdB2COptionsAuthority"] = _azureAdB2COptions.Authority;
                ViewData["AzureAdB2COptionsRedirectUri"] = _azureAdB2COptions.RedirectUri;
                ViewData["AzureAdB2COptionsApiUrl"] = _azureAdB2COptions.ApiUrl;
                ViewData["AzureAdB2COptionsApiScopes"] = _azureAdB2COptions.ApiScopes;

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        responseString = await response.Content.ReadAsStringAsync();
                        break;

                    case HttpStatusCode.Unauthorized:
                        responseString = $"Please sign in again. {response.ReasonPhrase}";
                        break;

                    default:
                        responseString = $"Error calling API. StatusCode=${response.StatusCode}";
                        break;
                }
            }
            catch (MsalUiRequiredException ex)
            {
                responseString = $"Session has expired. Please sign in again. {ex.Message}";
            }
            catch (Exception ex)
            {
                responseString = $"Error calling API: {ex.Message}";
            }

            ViewData["ResponsePayload"] = $"{responseString}";

            return Page();
        }
        static X509Certificate2 GetCert(String certificatePath, string cert_password)
        {
            X509Certificate2 cer = new X509Certificate2(certificatePath, cert_password, X509KeyStorageFlags.EphemeralKeySet);
            return cer;
        }
    }
}