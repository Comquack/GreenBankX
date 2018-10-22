using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Google.SignIn;
using UIKit;

namespace GreenBankX.iOS
{
    public class LoginiOS : NSObject, ILogin,  ISignInDelegate, ISignInUIDelegate
    {
        public static LoginiOS instance = new LoginiOS();
        private Action<GoogleUser, string> _onLoginComplete;
        private UIViewController _viewController { get; set; }
        public ILogin GetInstance()
        {
            if (instance == null)
            {
                return new LoginiOS();
            }
            return instance;
        }
        public LoginiOS()
        {
            Google.SignIn.SignIn.SharedInstance.UIDelegate = this;
            Google.SignIn.SignIn.SharedInstance.Delegate = this;
        }

        public void Login(Action<GoogleUser, string> OnLoginComplete)
        {
            _onLoginComplete = OnLoginComplete;

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            _viewController = vc;

            Google.SignIn.SignIn.SharedInstance.SignInUser();
        }

        public bool SignIn()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            _viewController = vc;

            Google.SignIn.SignIn.SharedInstance.SignInUser();
            return true;
        }

        public void SignOut()
        {
            Google.SignIn.SignIn.SharedInstance.SignOutUser();
        }

        public void DidSignIn(SignIn signIn, Google.SignIn.GoogleUser user, NSError error)
        {

            if (user != null && error == null)
                _onLoginComplete?.Invoke(new GoogleUser()
                {
                    Name = user.Profile.Name,
                    Email = user.Profile.Email,
                    Picture = user.Profile.HasImage ? new Uri(user.Profile.GetImageUrl(500).ToString()) : new Uri(string.Empty)
                }, string.Empty);
            else
                _onLoginComplete?.Invoke(null, error.LocalizedDescription);
        }

        [Export("signIn:didDisconnectWithUser:withError:")]
        public void DidDisconnect(SignIn signIn, Google.SignIn.GoogleUser user, NSError error)
        {
            // Perform any operations when the user disconnects from app here.
        }

        [Export("signInWillDispatch:error:")]
        public void WillDispatch(SignIn signIn, NSError error)
        {
            //myActivityIndicator.StopAnimating();
        }

        [Export("signIn:presentViewController:")]
        public void PresentViewController(SignIn signIn, UIViewController viewController)
        {
            _viewController?.PresentViewController(viewController, true, null);
        }

        [Export("signIn:dismissViewController:")]
        public void DismissViewController(SignIn signIn, UIViewController viewController)
        {
            _viewController?.DismissViewController(true, null);
        }

        ILogin ILogin.GetInstance()
        {
            throw new NotImplementedException();
        }

        string ILogin.AccountName()
        {
            return "testing";
        }

        string ILogin.UseDrive(int select)
        {
            throw new NotImplementedException();
        }

        string ILogin.Download(int select)
        {
            throw new NotImplementedException();
        }

        void ILogin.SignOut()
        {
            throw new NotImplementedException();
        }
    }
    public class GoogleUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Uri Picture { get; set; }
    }
}