/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :UserNamePasswordValidator.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-05-23 16:03
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace Lfz.Security
{
    /// <summary>
    /// 授权服务
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// 验证用户是否有效
        /// </summary>  
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Login(string userName, string password);

    }

    public class NullAuthenticationService : IAuthenticationService
    { 
        public bool Login(string userName, string password)
        {
            return true;
        }
    }

    /// <summary>
    /// 自定义用户名、密码验证器
    /// </summary>
    public class UserNamePasswordCustomValidator : UserNamePasswordValidator
    {
        private readonly IAuthenticationService _authenticationService;

        public UserNamePasswordCustomValidator()
            : this(new NullAuthenticationService())
        {
        }

        public UserNamePasswordCustomValidator(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        ///  Validates the user name and password combination.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public override void Validate(string userName, string password)
        {
            if (_authenticationService.Login(userName, password)) return;
            throw new SecurityTokenException("用户名或者密码错误！");
        }
    }
}
