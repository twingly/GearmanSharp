using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twingly.Gearman.Packets
{
    public class SetClientIdRequest : RequestPacket
    {
        public string ClientId { get; protected set; }

        public SetClientIdRequest(string clientId)
            : base(PacketType.SET_CLIENT_ID)
        {
            if (clientId == null)
                throw new ArgumentNullException("clientId");

            ClientId = clientId;
        }

        public override byte[] GetData()
        {
            return Util.JoinByteArraysForData(Encoding.UTF8.GetBytes(ClientId));
        }
    }
}
