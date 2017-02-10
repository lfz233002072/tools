using Lfz.Logging;

namespace Lfz.Commands
{
    /// <summary>
    /// 命令控制器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ICommandHandler<out TResult> where TResult : ICommandResult
    {
        /// <summary>
        /// 日志
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        /// <param name="command"></param>
        /// <returns></returns>
        TResult Execute( object command);
    }

    /// <summary>
    /// 命令控制器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<in TCommand, out TResult> : ICommandHandler<TResult> where TResult : ICommandResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        TResult Execute(TCommand command);
    }
}

