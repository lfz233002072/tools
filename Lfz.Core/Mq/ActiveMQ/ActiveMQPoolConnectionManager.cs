using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Lfz.Logging;

namespace Lfz.Mq.ActiveMQ
{
    /// <summary>
    /// 
    /// </summary>
    public class ActiveMQPoolConnectionManager
    {
        private readonly IMqConfigService _clusterService;
        private readonly object _connlock = new object();
        /// <summary>
        /// 最大连接客户端
        /// </summary>
        public int MaxConnectCount { get; private set; }
        private readonly ConcurrentDictionary<Guid, PoolItem> _poolItemList;
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary> 
        public ActiveMQPoolConnectionManager(IMqConfigService clusterService, int maxConnectCount = 5)
        {
            _clusterService = clusterService;
            _logger = LoggerFactory.GetLog();
            MaxConnectCount = maxConnectCount;
            _poolItemList = new ConcurrentDictionary<Guid, PoolItem>();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IConnection GetPoolConnection(Guid storeId, out int mqInstanceId)
        {
            PoolItem poolItem;
            if (_poolItemList.ContainsKey(storeId)) poolItem = _poolItemList[storeId];
            else
            {
                var computer = _clusterService.GetMqConfig(storeId, true);
                if (computer == null)
                {
                    mqInstanceId = 0;
                    return null;
                }
                var connectionFactory =
                    new ConnectionFactory(string.Format("tcp://{0}:{1}", computer.IpAddress, computer.Port));
                connectionFactory.UserName = computer.AccessUsername;
                connectionFactory.Password = computer.AccessPassword;
                poolItem = new PoolItem()
                {
                    Factory = connectionFactory,
                    MqInstanceId = computer.MqInstanceId,
                    Connections = new List<IConnection>()
                };
                _poolItemList.AddOrUpdate(storeId, poolItem, (x, y) => poolItem);
            }
            var connections = poolItem.Connections;
            var factory = poolItem.Factory;
            mqInstanceId = poolItem.MqInstanceId;
            if (connections.Count < this.MaxConnectCount)
            {
                IConnection connection = null;
                try
                {
                    connection = factory.CreateConnection();
                    bool flag = true;
                    lock (this._connlock)
                    {
                        if (connections.Count < this.MaxConnectCount)
                        {
                            connections.Add(connection);
                            return connection;
                        }
                        flag = false;
                    }
                    if (!flag)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                    _logger.Error(ex, this.GetType().Name + ".GetPoolConnection");
                }
            }
            int index = this.Random(this.MaxConnectCount);
            IConnection activeMQPoolConnection2 = connections[index];
            if (!this.ConnectionCheck(activeMQPoolConnection2))
            {
                lock (this._connlock)
                {
                    activeMQPoolConnection2 = connections[index];
                    if (!this.ConnectionCheck(activeMQPoolConnection2))
                    {
                        connections[index] = factory.CreateConnection();
                        activeMQPoolConnection2 = connections[index];
                    }
                }
            }
            return activeMQPoolConnection2;
        }
        private int Random(int maxvalue)
        {
            return new System.Random(System.Guid.NewGuid().GetHashCode()).Next(0, maxvalue);
        }

        private bool ConnectionCheck(IConnection c)
        {
            try
            {
                if (!c.IsStarted)
                {
                    c.Start();
                }
                if (c is Connection)
                {
                    Connection connection = c as Connection;
                    if (connection.FirstFailureError != null || connection.TransportFailed)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, this.GetType().Name + ".ConnectionCheck");
            }
            return false;
        }

        /// <summary>
        /// 清理缓存池
        /// </summary>
        public void CleanPool()
        {
            foreach (var poolItem in _poolItemList)
            {
                try
                {
                    if (poolItem.Value != null && poolItem.Value.Connections != null)
                    {
                        poolItem.Value.Connections.ForEach(delegate(IConnection c)
                        {
                            c.Close();
                            c.Dispose();
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, this.GetType().Name + ".Dispose");
                }
            }
            _poolItemList.Clear();
        }

        class PoolItem
        {
            public ConnectionFactory Factory { get; set; }

            public int MqInstanceId { get; set; }


            public List<IConnection> Connections { get; set; }

        }
    }
}
