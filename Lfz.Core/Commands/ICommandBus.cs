using System.Collections.Generic;
using Lfz.Validation;

namespace Lfz.Commands
{
    /// <summary>
    /// 工作单元唯一的
    /// </summary>
    public interface ICommandBus<out TResult> where TResult : ICommandResult
    {
        /// <summary>
        /// 命令提交
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        TResult Submit<TCommand>(TCommand command) ;
          
        /// <summary>
        /// 命令验证
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        IEnumerable<ValidationResult> Validate<TCommand>(TCommand command) ;

    }
}

