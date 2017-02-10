//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename : Utils
//        DESCRIPTION : 工具箱类
//
//        Created By 林芳崽 at  2012-08-25 19:32:40
//        https://git.oschina.net/lfz/tools
//
//====================================================================== 

using System;

namespace Lfz.Utitlies
{
    public class NetMailUtils
    {
        #region 字段属性

        private System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();

        /// <summary>
        /// 要发送的邮件
        /// </summary>
        public System.Net.Mail.MailMessage Email
        {
            get { return this.myEmail; }
            set { this.myEmail = value; }
        }

        private string _serverName = "";
        /// <summary>
        /// 设置邮件服务器
        /// </summary>
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        private int _serverPort = 25;
        /// <summary>
        /// 设置服务器端口号
        /// </summary>
        public int ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; }
        }

        private string _userName = "";
        /// <summary>
        /// SMTP认证时使用的用户名
        /// </summary>
        public string MailServerUserName
        {
            set
            {
                if (value.Trim() != "")
                {
                    this._userName = value.Trim();
                }
                else
                {
                    this._userName = "";
                }
            }
            get
            {
                return _userName;
            }
        }

        private string _userPassWord = "";
        /// <summary>
        /// SMTP认证时使用的密码
        /// </summary>
        public string MailServerPassWord
        {
            set
            {
                this._userPassWord = value;
            }
            get
            {
                return this._userPassWord;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svrName">smtp服务器</param>
        /// <param name="iPort">smtp服务器端口 0=默认</param>
        /// <param name="strName">验证email</param>
        /// <param name="strPwd">验证email密码</param>
        public NetMailUtils(string svrName,int iPort, string strName, string strPwd,string strForm)
        {
            this._serverName = svrName;
            this._userName = strName;
            this._userPassWord = strPwd;
            this.ServerPort= iPort == 0 ? 25 : iPort;
            this.myEmail.Priority = System.Net.Mail.MailPriority.Normal;
            this.myEmail.From = new System.Net.Mail.MailAddress(strForm);
            this.myEmail.IsBodyHtml = true;
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <returns>1=正常发送,-1=发送出错</returns>
        public int Send()
        {
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(this.ServerName, this.ServerPort);
                client.Credentials = new System.Net.NetworkCredential(this.MailServerUserName, this.MailServerPassWord);
                client.Send(myEmail);
                return 1;
            }
            catch(Exception e)
            {
                return -1;
                //throw e;
            }
        }
    }
}