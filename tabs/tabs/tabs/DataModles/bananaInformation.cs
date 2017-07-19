using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace tabs.DataModles
{
    public class bananaInformation
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "isBanana")]
        public bool IsBanana { get; set; }

        [JsonProperty(PropertyName = "color")]
        public String Color { get; set; }


        //Used when creating a new banana from custom vision api. Id and createdAt are both auto generated
        public bananaInformation(bool isBanana, String color)
        {
            this.IsBanana = isBanana;
            this.Color = color;
        }
    }
}
