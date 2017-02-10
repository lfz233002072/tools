/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :CustomX509Validator.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-05-23 16:39
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Lfz.Security
{
    /// <summary>
    /// X509认证验证器
    /// </summary>
    public class CustomX509Validator : X509CertificateValidator
    {
        /// <summary>
        /// Validates a certificate.
        /// </summary>
        /// <param name="certificate">The certificate the validate.</param>
        public override void Validate(X509Certificate2 certificate)
        {
            // validate argument
            if (certificate == null)
                throw new ArgumentNullException("X509认证证书为空！"); 
            // check if the name of the certifcate matches
            if (certificate.SubjectName.Name != ConfigurationManager.AppSettings["CertName"])
                throw new SecurityTokenValidationException("Certificated was not issued by thrusted issuer");
        }
    }
}