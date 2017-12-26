using System;
using System.Collections.Generic;
using System.Text;

namespace ConsulSharp
{
    /// <summary>
    /// Node
    /// </summary>
    public class Node1
    {
        public string ID { get; set; }

        public string Node { get; set; }

        public string Address { get; set; }

        public string Datacenter { get; set; }

        public TaggedAddress[] TaggedAddresses { get; set; }

        public object Meta { get; set;}

        public int CreateIndex { get; set; }

        public int ModifyIndex { get; set; }
    }
}
