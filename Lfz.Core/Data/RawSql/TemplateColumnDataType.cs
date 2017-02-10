// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :TemplateColumnDataType.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-05 15:56
// *        https://git.oschina.net/lfz/tools
// *
// *======================================================================*/
namespace Lfz.Data.RawSql
{
    /// <summary>
    /// 
    /// </summary>
    public enum TemplateColumnDataType
    {
        [CustomDescription("整数")]
        TemplateInt,
        [CustomDescription("字符串")]
        TemplateString,
        [CustomDescription("时间")]
        TemplateDatetime,
        [CustomDescription("数字")]
        TemplateDecimal,
        [CustomDescription("文本")]
        TemplateText
    }
}