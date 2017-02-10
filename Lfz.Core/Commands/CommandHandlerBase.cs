using Lfz.Logging;

namespace Lfz.Commands
{
    /// <summary>
    /// 
    /// </summary> 
    /// <typeparam name="TResult"></typeparam> 
    public abstract class CommandHandlerBase<TResult> : ICommandHandler<TResult>
        where TResult : ICommandResult
    {
        /// <summary>
        /// 
        /// </summary>
        protected CommandHandlerBase()
        {
            Logger = LoggerFactory.GetLog();
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// 可以重载的方法
        /// </summary> 
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract TResult Execute( object command);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam> 
    public abstract class CommandHandlerBase<TCommand, TResult> : CommandHandlerBase<TResult>, ICommandHandler<TCommand, TResult>
        where TResult : ICommandResult
    {

        /// <summary>
        /// 可以重载的方法
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract TResult Execute(TCommand command);
    }
}
