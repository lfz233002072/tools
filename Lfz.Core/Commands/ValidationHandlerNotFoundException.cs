using System;

namespace Lfz.Commands
{
    /// <summary>
    /// 
    /// </summary>
     public class ValidationHandlerNotFoundException : Exception
    {
         /// <summary>
         /// 
         /// </summary>
         /// <param name="type"></param>
         public ValidationHandlerNotFoundException(Type type)
            : base(string.Format("Validation handler not found for command type: {0}", type))
        {
        }
    }
}
