﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using GreenBankX.Resources;
using System.Threading;
using Rg.Plugins.Popup.Services;
using Xamarin.Auth;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace GreenBankX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        Account account;
        AccountStore store;
        UpDowner loader = UpDowner.GetInstance();
        public MenuPage()
        {
            Application.Current.Properties["PriceStore"] = null;

            store = AccountStore.Create();
            InitializeComponent();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            Xamarin.Forms.Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Welcome1");
        }
        public void Signot()
        {
            if (Toolout.Text == "")
            {
                return;
            }
           
   
        }
        void Driv3r()
        {
            if (ToolDrive.Text == "")
            {
                return;
            }
            else {
                Xamarin.Forms.Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("UploadPW");
                loader.UpList();
            }
           // string nu = DependencyService.Get<ILogin>().UseDrive(-1);

        }
        void OnLoginTest()
        {
            string clientId = null;
            string redirectUri = null;
            if (ToolIn.Text == "")
            {
                return;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.iOSClientId;
                    redirectUri = Constants.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = Constants.AndroidClientId;
                    redirectUri = Constants.AndroidRedirectUrl;
                    break;
            }

            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Constants.scopes,
                new Uri(Constants.AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(Constants.AccessTokenUrl),
                null,
                true);
            
            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }



        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            User user = null;
            if (e.IsAuthenticated)
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    // Deserialize the data and store it in the account store
                    // The users email address will be used to identify data in SimpleDB
                    string userJson = await response.GetResponseTextAsync();
                    user = JsonConvert.DeserializeObject<User>(userJson);
                }
                if (account != null)
                {
                    store.Delete(account, Constants.AppName);
                }
                Application.Current.Properties["User"] = user;
                await store.SaveAsync(account = e.Account, Constants.AppName);
                Application.Current.Properties["Account"] = (await store.FindAccountsForServiceAsync(Constants.AppName)).FirstOrDefault();
                Application.Current.Properties["Signed"] = true;
                Xamarin.Forms.Application.Current.Properties["Boff"] = "Hello " + user.Name +"\n"+AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Welcome2");
                loader.Tokenise();
            }
            else
            {
                Xamarin.Forms.Application.Current.Properties["Boff"] = "error";
            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            Xamarin.Forms.Application.Current.Properties["Boff"] = "Authentication error: " + e.Message;
        }


        async void OpenMeasure(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MeasureTree());
        }

        async void OpenManager(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ManagePlots());
        }
        async void OpenPrice(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePricing());
        }
        private async System.Threading.Tasks.Task ChangeLang() {
            MessagingCenter.Unsubscribe<LangPop>(this, "Done");
            MessagingCenter.Subscribe<LangPop>(this, "Done", (sender) => {
                Bttn1.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                BttnOther.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot");
                Bttn3.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                Bttn4.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Pricings");

                Summary.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Summary");
                Curren.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Currency");
                Lang.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Language");
                Tute.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Tutorial");
                Cred.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Team");
                try
                {
                    User user = null;
                    user = (User)Application.Current.Properties["User"];
                    Xamarin.Forms.Application.Current.Properties["Boff"] = "Hello " + user.Name + "\n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Welcome2");
                }
                catch { Xamarin.Forms.Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Welcome1"); }
            });
               await PopupNavigation.Instance.PushAsync(LangPop.GetInstance());
        }

        private void ToolDown_Clicked(object sender, EventArgs e)
        {
            if (ToolDown.Text == "")
            {
                return;
            }
            else {
                Xamarin.Forms.Application.Current.Properties["Boff"] = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DownloadPW");
                loader.FilesList();
                SaveAll.GetInstance().LoadAll();
            }



        }

        private void boffo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (boffo.Text == "Finished" && (bool)Application.Current.Properties["Load"]) {
                SaveAll.GetInstance().LoadAll();
            } else if((bool)Application.Current.Properties["Signed"]) {
                ToolDrive.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Upload");
                ToolDown.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Download");
                Toolout.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SignOut");
                ToolIn.Text = "";
            }
            else if (!(bool)Xamarin.Forms.Application.Current.Properties["Signed"])
            {
                ToolDrive.Text = "";
                ToolDown.Text = "";
                Toolout.Text = "";
                ToolIn.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SignIn");
            }
        }
        protected override void OnAppearing()
        {
            Application.Current.Properties["PriceStore"] = null;
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            base.OnAppearing();
        }

            private async void Tute_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(Tutepop.GetInstance());

        }

        async private void BttnOther_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NotPopup());
        }

        async private void Summary_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Summary());
        }

        async private void Curren_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Currency());
        }

        async private void Cred_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(Credits.GetInstance());
        }
    }
    class ClockViewModel : INotifyPropertyChanged
    {
        string dateTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClockViewModel()
        {
            DateTime = (string)Application.Current.Properties["Boff"];

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                DateTime = (string)Application.Current.Properties["Boff"];
                return true;
            });
        }

        public string DateTime
        {
            set
            {
                if (dateTime != value)
                {
                    dateTime = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("DateTime"));
                    }
                }
            }
            get
            {
                return dateTime;
            }
        }
    }
}