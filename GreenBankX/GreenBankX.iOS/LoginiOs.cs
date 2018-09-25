using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.IO;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Auth;
using GreenBankX;
using Newtonsoft.Json;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

[assembly: Dependency(typeof(LoginiOs))]

class LoginiOs : ILogin
{
    OAuth2Authenticator authenticator;
    FileStream inputStream;
    Account accnt;
    User user;
    public static LoginiOs instance = new LoginiOs();
    public ILogin GetInstance()
    {
        if (instance == null)
        {
            return new LoginiOs();
        }
        return instance;
    }
    public LoginiOs() {
        var authenticator = new OAuth2Authenticator(
    "263109938909-bts0mgt2859gv9btr2h9ep36fqtk31dh.apps.googleusercontent.com",
    null,
    Constants.scopes,
    new Uri(Constants.AuthorizeUrl),
    new Uri("com.googleusercontent.apps.263109938909-bts0mgt2859gv9btr2h9ep36fqtk31dh"),
    new Uri(Constants.AccessTokenUrl),
    null,
    true);
        authenticator.Completed += OnAuthCompleted;
    }

    private void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
    {
        if (e.IsAuthenticated)
        {
            accnt = e.Account;

        }
        
    }

    public bool SignIn()
    {
        var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
        presenter.Login(authenticator);
        return true;

    }
    public void SignOut()
    {

    }
    public string AccountName()
    {
        if (accnt != null) {
            return accnt.Username;
        }
        return "Not Implemented";
    }
    public string UseDrive(int select)
    {

        return "";
    }


    public async Task Request()
    {
        var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, accnt);
        var response = await request.GetResponseAsync();
        if (response != null)
        {
            string userJson = response.GetResponseText();
            user = JsonConvert.DeserializeObject<User>(userJson);
            AccountStore.Create().Save(accnt, Constants.AppName);
        }
        return;
    }
    public string Download(int select)
    {
        return "";
    }
    public void Authenticate()
    {
        // Register "Other" application in Google Console to get both clientId and clientSecret
        var secrets = new ClientSecrets() { ClientId = "263109938909-bts0mgt2859gv9btr2h9ep36fqtk31dh.apps.googleusercontent.com", ClientSecret = null };
        var initializer = new GoogleAuthorizationCodeFlow.Initializer { ClientSecrets = secrets };
        var flow = new GoogleAuthorizationCodeFlow(initializer);

        // Refresh token can be obtained with the following curl commands: 
        // http://stackoverflow.com/questions/5850287/youtube-api-single-user-scenario-with-oauth-uploading     videos/8876027#8876027
        // You should be able to achieve the same via Xamarin.Auth
        var token = new TokenResponse { RefreshToken = accnt.Properties["refresh_token"]};

        credentials = new UserCredential(flow, "user", token);
    }
    UserCredential credentials;

    DriveService Service
    {
        get
        {
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "<application.name>",
            });
        }
    }
}


