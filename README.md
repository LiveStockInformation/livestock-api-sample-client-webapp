

# Livestock API Samples: Asp.Net Web App signing in users with Azure Active Directory B2C and calling a livestock Information systems API ( Hello World API)

> This sample is using MSAL.NET 4.x. 


This simple sample demonstrates how to use the [Microsoft Authentication Library (MSAL) for .NET] to get an access token and call a Livestock Information Systems API secured by Azure AD B2C.


### Run the project

(1) Open the `livestock-api-sample-client-webapp` project. 
(2) Set as startup Project 
(3) Add folloiwng required values in Appsettings.json files:
    (a) Tenant
    (b) RedirectUrl
    (c) CLientId
    (d) SignUpSignInPolicyId
    (e) ResetPasswordPolicyId
    (f) ApiScopes
    (g) ApiUrl
	  (h) APISubscriptionKey 
    (i) Authority
	
and run the project. 


The sample demonstrates the following functionality: 

1. Click the sign-in button at the top of the application screen. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. Upon successful sign in,you can goto claims and see the user claims information.
2. Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
3. Sign out by clicking the Sign out button and confirm that you lose access to the API until the exit interactive sign in. 
4. Call the api by clicking on the Secure-APICall menu item.

## More information
For more information on Azure B2C, see [the Azure AD B2C documentation homepage](http://aka.ms/aadb2c). 
