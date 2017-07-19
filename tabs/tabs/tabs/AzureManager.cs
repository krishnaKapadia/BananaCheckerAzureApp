using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tabs.DataModles;

namespace tabs
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<bananaInformation> bananaTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://bananaapp.azurewebsites.net");
            this.bananaTable = this.client.GetTable<bananaInformation>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<bananaInformation>> GetBananaInformation()
        {
            return await this.bananaTable.ToListAsync();
        }

        public async Task createNewBanana(bananaInformation banana)
        {
            await this.bananaTable.InsertAsync(banana);
        }
    }
}