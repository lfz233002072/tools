namespace Lfz.Network
{
    /// <summary>
    /// 套接接收数据事件委托定义
    /// </summary>
    /// <param name="buffer"></param>
    public delegate void SocketReceivedEventHandler(PacketBufferToken buffer);
}