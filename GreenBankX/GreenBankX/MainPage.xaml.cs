﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenBankX
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            Application.Current.Properties["Plots"] = new List<Plot>();
        }
        async void OpenMenu(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuPage());
        }
        async void OpenMap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePlot());
        }
    }
}
