using System;
using MessagePack;

namespace Triathlon.Networking
{
    [MessagePackObject]
    public class Request
    {
        [Key(0)]
        public RequestType Type { get; set; }

        [Key(1)]
        public object Data { get; set; }

        public Request() { } 

        public Request(RequestType type, object data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}