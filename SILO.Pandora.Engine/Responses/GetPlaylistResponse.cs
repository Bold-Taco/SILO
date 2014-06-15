using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SILO.Pandora.Engine.Data;

namespace SILO.Pandora.Engine.Responses {
    internal class GetPlaylistResponse: PandoraData {
        [JsonProperty(PropertyName = "items")]
        public List<PandoraSong> Songs {
            get;
            set;
        }
    }
}
