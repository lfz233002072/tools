namespace Lfz.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultCommandResult : ICommandResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        public DefaultCommandResult(bool success)
        {
            this.Success = success;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; private set; }
    }
}