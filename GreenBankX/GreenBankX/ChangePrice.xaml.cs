using System;
using System.Collections.Generic;
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
        public static ChangePrice instance = new ChangePrice();
        Plot thisPlot;
        int counter = -1;
        public static ChangePrice GetInstance()
        {
            if (instance == null)
            {
                return new ChangePrice();
            }
            instance.Refresh();
            return instance;
        }

        private ChangePrice()
        {

            InitializeComponent();
            Refresh();
        }

        public void Refresh() {
            ChosePrice.Items.Clear();
            counter = (int)Application.Current.Properties["Counter"];
            if (counter > -1) {
            thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter);
                for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
                {
                    ChosePrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
                }
            }
    }


        public async Task Confirm()
        {
            if (ChosePrice.SelectedIndex > -1) {
                NameLabel.Text = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(ChosePrice.SelectedIndex).GetName() + counter.ToString();
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).SetRange(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(ChosePrice.SelectedIndex));
                MessagingCenter.Send<ChangePrice>(this, "Change");
            }
            await PopupNavigation.Instance.PopAsync();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
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

        private void ChosePrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Fills list of prices
            string prices = "";
            string logs = "";
            if (ChosePrice.SelectedIndex > -1)
            {
                PriceRange ThisPrice = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(ChosePrice.SelectedIndex);
                SortedList<double, double> brack = ThisPrice.GetBrack();
                prices = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Price") + "/m\xB3\n";
                logs = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("minimumdiameter") + "\n ";
                for (int x = 0; x < brack.Count(); x++)
                {
                    prices = prices + brack.ElementAt(x).Value + "\n";
                    logs = logs + brack.ElementAt(x).Key + "\n";
                }
                NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPrice.GetName() + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LogLength") + ": " + ThisPrice.GetLength().ToString() + "m";
                lab2.Text = prices;
                lab1.Text = logs;
            }
        }
    }
}
