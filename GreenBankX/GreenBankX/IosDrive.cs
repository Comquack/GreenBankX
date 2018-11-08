using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;
using Xamarin.Forms;

namespace GreenBankX
{
    class IosDrive
    {
        private DriveService Service { get; set; }
        Account acc;
        public IosDrive(Account account) {
            acc = account;
            if (Service != null)
                return;
            //TODO: Service Account Credentials are commented as the Certificate needs to be used manually.
            //var credential = DependencyService.Get<IServiceCredential>().Credential;
            var googleFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = Constants.iOSClientId,
                    ClientSecret = Constants.iOSClientSecret,
                }
            });

            var tokenResponse = new TokenResponse
            {
                TokenType =
        acc.Properties["token_type"],
                AccessToken =
        acc.Properties["access_token"]
            }; ;
            // implement token
           var userCredentials = new UserCredential(googleFlow, "", tokenResponse);
           Service = new DriveService(new BaseClientService.Initializer()
            {
               HttpClientInitializer = userCredentials,
                ApplicationName = Constants.AppName,
            });
        }
    }
}
