using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenBankX.Resources;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//generic delete confirmation popup uses message centre "delete"
namespace GreenBankX
{
    public partial class ChangePrice : Rg.Plugins.Popup.Pages.PopupPage
    {
        (int, int) numbers;
        public static ChangePrice instance = new ChangePrice();
        public static ChangePrice GetInstance()
        {
            if (instance == null)
            {
                return new ChangePrice();
            }
            return instance;
        }

        private ChangePrice()
        {

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            numbers = (((int, int))Application.Current.Properties["Priceholder"]);
            //Len.Text = numbers.Item1.ToString()+"|"+numbers.Item1.ToString();

            Len.Text = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetLength().ToString();
            minDiam.Text = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2).Key.ToString();
            try { maxDiam.Text = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2+1).Key.ToString(); }
            catch { }
            price.Text = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2).Value.ToString();

        }

        private void Len_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 100 || double.Parse(e.NewTextValue) < 0))
            {
                Len.Text = e.OldTextValue;
            }

        }

        private void maxDiam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                maxDiam.Text = e.OldTextValue;
            }
        }
        private void minDiam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                minDiam.Text = e.OldTextValue;
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }

        private async void Change_Clicked(object sender, EventArgs e)
        {
            if (Len.Text != null)
            {
                ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).SetLength(double.Parse(Len.Text));
            }
            else
            {              Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("invalid size", "Size invalid", "OK");
                });
                return; }
            if (minDiam.Text != null && price.Text != null && double.Parse(minDiam.Text) > 0 && double.Parse(price.Text) > 0)
            {
                double key = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2).Key;
                double value = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2).Value;
                ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().RemoveAt(numbers.Item2);
                if (!((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).addBrack(double.Parse(minDiam.Text), double.Parse(price.Text)))
                {
                    try
                    {
                        ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).addBrack(key, value);

                    }
                    catch { }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), "OK");
                    });
                }
                else
                {
                    if (maxDiam.Text != null && double.Parse(maxDiam.Text) > double.Parse(minDiam.Text) && numbers.Item2 < ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().Count - 1 && ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2 + 1).Key != double.Parse(maxDiam.Text))
                    {
                        try
                        {
                            double key2 = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2 + 1).Key;
                            double value2 = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2 + 1).Value;
                            ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().RemoveAt(numbers.Item2 + 1);
                            try { double key3 = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).GetBrack().ElementAt(numbers.Item2 + 2).Key;
                                if (key3 <= double.Parse(maxDiam.Text)) {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MaxV"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MaxV2"), "OK");
                                    });

                                    return;
                                }
                            }
                            catch { }
                            if (!((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).addBrack(double.Parse(maxDiam.Text), value2))
                            {
                                try
                                {
                                    ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(numbers.Item1).addBrack(key2, value2);
                                }
                                catch { }
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), "OK");
                                });
                            }
                        }
                        catch { }
                    }
                    maxDiam.Text = null;
                    price.Text = null;
                    MessagingCenter.Send(this, "Change");
                    await PopupNavigation.Instance.PopAsync(); ;
                };

            }
            else if (minDiam.Text == null || double.Parse(minDiam.Text) <= 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DiaInvalid"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DiaInvalid"), "OK");
                });
            }
            else if (price.Text == null || double.Parse(price.Text) <= 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PriceInvalid"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PriceInvalid"), "OK");
                });
            }
            else
            {
                await PopupNavigation.Instance.PopAsync(); ;
                return;
            }

        }
        }
}
