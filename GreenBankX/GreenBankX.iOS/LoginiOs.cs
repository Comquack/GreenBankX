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

[assembly: Dependency(typeof(LoginiOs))]

class LoginiOs :  ILogin
    {
    OAuth2Authenticator authenticator;
        FileStream inputStream;
    Account accnt;
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
    new Uri("om.googleusercontent.apps.263109938909-bts0mgt2859gv9btr2h9ep36fqtk31dh"),
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
        throw new NotImplementedException();
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


    public async Task<string> UseDriveTask(int select)
    {
        var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, accnt);
        var response = await request.GetResponseAsync();
        if (response != null)
        {
            string userJson = response.GetResponseText();
            var user = JsonConvert.DeserializeObject<User>(userJson);
        }
        return "";
    }
        public string Download(int select)
        {
            
            return "";
        }
     
    }


