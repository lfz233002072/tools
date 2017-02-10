using System;

namespace Lfz.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandHandlerNotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public CommandHandlerNotFoundException(Type type) : base(string.Format("Command handler not found for command type: {0}", type))
        {
        }
    }
}

