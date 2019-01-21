using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenBankX.Resources;
using OxyPlot.Axes;
using OxyPlot.Series;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ManagePlots : ContentPage
	{
        List<Plot> changedPlot;
        int GraphNo = -1;
        ObservableCollection<PlotContainer> plotty = new ObservableCollection<PlotContainer>();
        PlotContainer doubletap2;
        int year = DateTime.Now.Year;
		public ManagePlots ()
		{
            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent();
            ToolPricing.Text = "";
            changedPlot = new List<Plot>();
        }
        //activates when index is changed in the plot picker
        public void SelectPlot()
        { string trees = "";
            PlotList.IsVisible = false;
            ObservableCollection<Tree> TreeTails = new ObservableCollection<Tree>();
            string girths = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth")+"\n";
            string heights = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + "\n";
            if (pickPlot.SelectedIndex > -1)
            {
                ToolEdit.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EditPlot");
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                trees = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPlot.GetName();
                if (ThisPlot.Owner != null && ThisPlot.Owner != "")
                {
                    trees += AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Owner") + ": " + ThisPlot.Owner + "\n";
                }
                if (ThisPlot.NearestTown != null && ThisPlot.NearestTown != "")
                {
                    trees += AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Location") + ": " + ThisPlot.NearestTown + "\n";
                }
                if (ThisPlot.Describe != null && ThisPlot.Describe != "")
                {
                    trees += AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Comments") + ": " + ThisPlot.Describe + "\n";
                }
                List<Tree> TreeList = ThisPlot.getTrees();
                List<DetailsGraph2> Detailstree = new List<DetailsGraph2>();
                Tree ThisTree;
                for (int x = 0; x < TreeList.Count; x++)
                {
                    ThisTree = TreeList.ElementAt(x);
                    Detailstree.Add(new DetailsGraph2() { girth = Math.Round(ThisTree.Diameter * (Girtdswitch.IsToggled ? 1/ Math.PI : 1 ), 2), ID = ThisTree.Id, price = ThisTree.Merch, tree = ThisTree });
                    TreeTails.Add(ThisTree);
                }
                DetailsList.IsVisible = true;
                ToolPricing.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CPrice");
                DetailsList.ItemsSource = Detailstree;
                DetailsList.HeightRequest = (40 * Math.Min(TreeTails.Count, 5)) + (10 * Math.Min(TreeTails.Count, 5)) + 60;
                PlotTitle.Text = trees;

                ToolDelete.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DeletePlot");
                ToolDeleteTree.Text = "";
                Girtdswitch.IsVisible = true;
                Girtdlab.IsVisible = true;
            }
            else {
                OnAppearing();
            }
        }
        //activates when index for tree picker is changed
     

        public void DunLLoadin()
        {
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutmanage"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Manage Plots", "This page allows you to manage the plots you have created.", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Skip"));
                    if (res)
                    {
                        await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots"), "After selecting a plot from the menu, you will be shown the list trees on the plot. The  plot can be added to by pressing the add to plot button, or by going to the measure trees page", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("", "From this menu you can edit or delete plots or, delete trees.", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        Application.Current.Properties["Tutmanage"] = false;
                        Application.Current.Properties["Tutmanage2"] = true;
                        Application.Current.Properties["TLogs"] = true;
                    }
                    else
                    {
                        Application.Current.Properties["Tutmanage"] = false;
                    }
                });
            }
        }

        public async void DelPlot() {
            if (pickPlot.SelectedIndex > -1)
            {
                MessagingCenter.Unsubscribe<DeleteConfirm>(this, "Delete");
                MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", async (sender) => {
                    ((List<Plot>)Application.Current.Properties["Plots"]).RemoveAt(pickPlot.SelectedIndex);
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots")+": saving"; 
                    SaveAll.GetInstance().SavePlots();
                    SaveAll.GetInstance().SaveTrees2();
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saved";
                    OnAppearing();
                    PlotTitle.IsVisible = false;
                    await Task.Delay(5000);
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");

                });
                await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());
            }
        }

        public async void AddNewTree()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                Application.Current.Properties["Counter"] = pickPlot.SelectedIndex;
                MessagingCenter.Unsubscribe<AddTreePop>(this, "Add");
        
               MessagingCenter.Subscribe<AddTreePop>(this, "Add", async (sender) =>
               {
                    int hold = pickPlot.SelectedIndex;
                   pickPlot.SelectedIndex = -1;
                   pickPlot.SelectedIndex = hold;
                   Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saving";
                   SaveAll.GetInstance().SaveTrees2();
                   Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saved";
                   await Task.Delay(5000);
                   Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
               });
                await PopupNavigation.Instance.PushAsync(AddTreePop.GetInstance());
            }
       }
        //adds new measurement to selected tree
        public async void AddTreeMes()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                Application.Current.Properties["Counter"] = pickPlot.SelectedIndex;
                MessagingCenter.Unsubscribe<AddMesPop>(this, "Append");

                MessagingCenter.Subscribe<AddMesPop>(this, "Append", async (sender) =>
                {

                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saving";
                    SaveAll.GetInstance().SaveTrees2();
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saved";
                    await Task.Delay(5000);
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                });
                MessagingCenter.Subscribe<AddMesPop>(this, "Alter", async (sender) =>
                {
                    GraphNo = (int)Application.Current.Properties["HCounter"];
                    DetailsList.IsVisible = false;
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saving";
                    SaveAll.GetInstance().SaveTrees2();
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saved";
                    await Task.Delay(5000);
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                });
                await PopupNavigation.Instance.PushAsync(AddMesPop.GetInstance());
            }
        }
        public async void DelTree()
        {
            if (pickPlot.SelectedIndex > -1&& ToolDelete.Text!="")
            {
                MessagingCenter.Unsubscribe<DeleteConfirm>(this, "Delete");
                MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", async (sender) => {
                    string trees;
                   int selec = ((List<DetailsGraph2>)DetailsList.ItemsSource).IndexOf((DetailsGraph2)DetailsList.SelectedItem);
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().RemoveAt(selec);
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    trees = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPlot.GetName() +" "+ AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Area") + ": " + Math.Round(ThisPlot.GetArea(), 2) + "km2";
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);          
                    }
                    int storen = pickPlot.SelectedIndex;
                    pickPlot.SelectedIndex = -1;
                    pickPlot.SelectedIndex = storen;
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saving";
                    SaveAll.GetInstance().SaveTrees2();
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saved";
                    await Task.Delay(5000);
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                    ToolDelete.Text = "";
                });
                await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());

            }
        }

        public void Save()
        {
            //if (saveplot)
            //{
                SaveAll.GetInstance().SavePlots();
            //}
            //if (savetree)
            //{
                SaveAll.GetInstance().SaveTrees2();
            //}
            SaveAll.GetInstance().Kamel();
        }
        async public void ChangePricing()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                Application.Current.Properties["Counter"]= pickPlot.SelectedIndex;
                 await PopupNavigation.Instance.PushAsync(ChangePrice.GetInstance());
                }
        }
        //data displaed changes when selection is changed
 
  

        private void DetailsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

                    ToolDeleteTree.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DeleteTree");
        }


        protected override void  OnAppearing() {
            base.OnAppearing();
            plotty = new ObservableCollection<PlotContainer>();
            List<string> pickplotlist = new List<string>();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            DunLLoadin();
            GraphNo = -1;
            DetailsList.IsVisible = false;
             year = DateTime.Now.Year;
            //pickPlot.Items.Clear();
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
                ((List<Plot>)Application.Current.Properties["Plots"]).Count();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                plotty.Add(new PlotContainer(thisPlot.GetName(), thisPlot.getTrees().Count, thisPlot.YearPlanted));
                pickplotlist.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
            }
            pickPlot.ItemsSource = pickplotlist;
            PlotList.ItemsSource = plotty;
            pickPlot.SelectedIndex = -1;
            PlotList.IsVisible = true;
            Girtdswitch.IsVisible = false;
            Girtdlab.IsVisible = false;
        }
        protected override void OnSizeAllocated(double width, double height) {
            base.OnSizeAllocated(width,height);
            if (DetailsList.ItemsSource.GetType() == new ObservableCollection<Tree>().GetType())
            {
                if (width < height)
                {
                    DetailsList.HeightRequest = (height/14 * Math.Min(((ObservableCollection<Tree>)DetailsList.ItemsSource).Count, 7)) + height / 10;
                }
                else
                {
                    DetailsList.HeightRequest = (width / 14 * ((ObservableCollection<Tree>)DetailsList.ItemsSource).Count)  + width / 10;
                }
            }  
        }



        public async void EditPlot()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                Application.Current.Properties["ThisPlot"] = pickPlot.SelectedIndex;
                Application.Current.Properties["ThisLocation"] = null;
                MessagingCenter.Subscribe<PlotPopupEdit>(this, "Edit", async (sender) =>
                {
                    SelectPlot();
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saving";
                    SaveAll.GetInstance().SavePlots();
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots") + ": saved";
                    await Task.Delay(5000);
                    Titlename.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                });
                await PopupNavigation.Instance.PushAsync(PlotPopupEdit.GetInstance());
            }
        }
        private void PlotList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (doubletap2 == PlotList.SelectedItem)
            {
                pickPlot.SelectedIndex = plotty.IndexOf((PlotContainer)PlotList.SelectedItem);
            }
            else
            {
                doubletap2 = (PlotContainer)PlotList.SelectedItem;
            }
        }
        protected override bool OnBackButtonPressed()
        {
            if (DetailsList.IsVisible)
            {
                OnAppearing();
                PlotTitle.IsVisible = false;
                return true;
            }
            else
            {
                base.OnBackButtonPressed();
                return false;
            }
        }


        private void Girtdswitch_Toggled(object sender, ToggledEventArgs e)
        {
            GirthDetailsList.Text = Girtdswitch.IsToggled ? AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Diameter") : AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth");
            if (DetailsList.IsVisible)
            {
                SelectPlot();
            }

        }
    }

    public class DetailsGraph {
        public double volume { get; set; }
        public double price { get; set; }
        public string label { get; set; }
        public int logs { get; set; }
        public DetailsGraph()
        { }

        public override string ToString()
        {
            return label;
        }
    }

    public class DetailsGraph2
    {
        public double volume { get; set; }
        public double price { get; set; }
        public int ID { get; set; }
        public double girth { get; set; }
        public Tree tree { get; set; }
        public string label { get; set; }
        public double height { get; set; }
        public double[,] result { get; set; }
        public SortedList<double, double> brack { get; set;}
        public int resultrow { get; set; }


        public DetailsGraph2()
        { }
    }
}