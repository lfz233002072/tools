using System.Collections.Generic;
using Lfz.Validation;

namespace Lfz.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface IValidationHandler<in TCommand>   
    {
        /// <summary>
        /// 命令验证
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IEnumerable<ValidationResult> Validate(TCommand command);
    }
     
     
}
