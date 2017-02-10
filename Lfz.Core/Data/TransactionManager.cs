using System;
using System.Transactions;
using PMSoft.Logging;

namespace PMSoft.Data
{
    /// <summary>
    /// 事务管理
    /// </summary>
    public interface ITransactionManager : IDisposable
    {
        /// <summary>
        /// 事务提交
        /// </summary>
        void Demand();
        /// <summary>
        /// 取消事务
        /// </summary>
        void Cancel();
    }

    /// <summary>
    /// 事务管理
    /// </summary>
    public class TransactionManager : ITransactionManager, IDisposable
    {
        private TransactionScope _scope;
        private bool _cancelled;

        /// <summary>
        /// 
        /// </summary>
        public TransactionManager()
        {
            Logger = LoggerFactory.GetLog();
        }

        /// <summary>
        /// 事务开始请求
        /// </summary>
        public ILogger Logger { get; set; }

        void ITransactionManager.Demand()
        {
            if (_cancelled)
            {
                try
                {
                    _scope.Dispose();
                }
                catch
                {
                    // swallowing the exception
                }

                _scope = null;
            }

            if (_scope == null)
            {
                Logger.Debug("Creating transaction on Demand");
                _scope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadCommitted
                    });
            }
        }

        void ITransactionManager.Cancel()
        {
            Logger.Debug("Transaction cancelled flag set");
            _cancelled = true;
        }

        void IDisposable.Dispose()
        {
            if (_scope != null)
            {
                if (!_cancelled)
                {
                    Logger.Debug("Marking transaction as complete");
                    _scope.Complete();
                }

                Logger.Debug("Final work for transaction being performed");
                try
                {
                    _scope.Dispose();
                }
                catch
                {
                    // swallowing the exception
                }
                Logger.Debug("Transaction disposed");
            }
        }
    }
}
