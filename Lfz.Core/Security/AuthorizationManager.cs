using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Lfz.Security
{
    /// <summary>
    /// 身份认证管理器
    /// </summary>
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool CheckAccess(OperationContext operationContext, ref Message message)
        {
            //此模式并不提供消息的完整性和保密性，而是仅提供基于 HTTP 的客户端身份验证。
            return base.CheckAccess(operationContext, ref message);
        }

        private IPrincipal GetPrincipal(OperationContext operationContext)
        {
            return operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as IPrincipal;
        }
    }
}