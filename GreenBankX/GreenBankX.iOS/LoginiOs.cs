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
using Google.SignIn;

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
        var SignInB = new SignInButton();
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



    public string Download(int select)
    {
        return "";
    }



