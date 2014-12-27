using System;
using System.Linq;
using System.Net;

namespace EasyGelf.Core
{
    public class RandomEndpointSelector : IEndpointSelector
    {
        [ThreadStatic] private static Random random;

        private static Random Random {
            get { return random ?? (random = new Random()); }
        }

        public IPEndPoint GetEnpoint(IPEndPoint[] topology)
        {
            var index = Random.Next(topology.Count());
            return topology[index];
        }
    }
}