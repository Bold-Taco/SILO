using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SILO.Pandora.Engine.Data;

namespace SILO.Pandora.Engine.Responses {
    internal class GetGenreStationsResponse: PandoraData {
        [JsonProperty(PropertyName = "categories")]
        public List<PandoraStationCategory> Categories {
            get;
            set;
        }

        [JsonProperty(PropertyName = "checksum")]
        public string Checksum {
            get;
            set;
        }
    }
}
