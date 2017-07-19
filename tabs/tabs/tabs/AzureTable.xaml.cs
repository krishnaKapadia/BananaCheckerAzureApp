using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using tabs.DataModles;

namespace tabs
{
    public partial class AzureTable : ContentPage
    {

        public AzureTable()
        {
            InitializeComponent();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<bananaInformation> list = await AzureManager.AzureManagerInstance.GetBananaInformation();

            BananaList.ItemsSource = list;
        }

    }


}