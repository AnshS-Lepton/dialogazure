using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
   public class TokenDetail
    {
        public string loginId { get; set; }
        public string userId { get; set; }
        public string email { get; set; }
        public int loginHistoryId { get; set; }
        public string isActive { get; set; }
        public string userRoleId { get; set; }
        public string userRole { get; set; }
        public string clientIp { get; set; }
        public string browserVersion { get; set; }
        public string browserName { get; set; }
        public string Source { get; set; }
        public string IsMasterLogin { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        [JsonProperty(".issued")]
        public string issued { get; set; }
        [JsonProperty(".expires")]
        public string expires { get; set; }

        public string refresh_token { get; set; }

    }
}
