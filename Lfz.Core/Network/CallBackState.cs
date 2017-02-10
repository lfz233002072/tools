using System.Net.Sockets;

namespace Lfz.Network
{
    internal class CallBackState
    {
        /// <summary>
        /// 
        /// </summary>
        public TcpClient Client { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public NetworkStream NetworkStream { get; set; }

        public byte[] Data;
    }
}