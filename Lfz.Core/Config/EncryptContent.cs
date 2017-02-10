using System;
using System.Xml.Serialization;

namespace Lfz.Config
{
    /// <summary>
    ///  加密内容
    /// </summary>
    [Serializable]
    [XmlRoot("EncryptContent")]
    public class EncryptContent : IConfigInfo
    {
        /// <summary>
        /// 配置内容
        /// </summary>
        [XmlElement(ElementName = "Content")]
        public string Content { get; set; }
    }
}