using System;
using MessagePack;

namespace Triathlon.Networking
{
    [MessagePackObject]
    public class Response
    {
        [Key(0)]
        public ResponseType Type { get; set; }

        [Key(1)]
        public object Data { get; set; }

        public Response() { } 

        public Response(ResponseType type, object data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}