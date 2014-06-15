using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SILO.Pandora.Engine.Data;
using SILO.Pandora.Engine.Responses;

namespace SILO.Pandora.Engine.Requests {
    internal class GetStationListRequest: PandoraRequest {
        public override string MethodName {
            get { return "user.getStationList"; }
        }

        public override Type ReturnType {
            get { return typeof(GetStationListResponse); }
        }

        public override bool IsSecure {
            get { return false; }
        }

        public override bool IsEncrypted {
            get { return true; }
        }

        public GetStationListRequest(PandoraSession session) :
            base(session) {
        }
    }
}
