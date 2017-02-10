namespace Lfz.Utitlies
{
    /// <summary>
    /// pdf引擎
    /// </summary>
    public class PdfEngine
    {
        //private readonly string _rootDirectory;
        //private readonly string _virtualRootDirectory;

        //private readonly ILogger _logger;

        //#region 构造函数

        //public PdfEngine()
        //    : this("~/UploadRoot/BADJ")
        //{
        //}

        ///// <summary>
        ///// 存储文件跟目录
        ///// </summary>
        ///// <param name="rootDirectory"></param>
        //public PdfEngine(string rootDirectory)
        //{
        //    _rootDirectory = Utils.MapPath(rootDirectory);
        //    CheckDirectory(_rootDirectory);
        //    _logger = LoggerFactory.GetLog();
        //    _virtualRootDirectory = rootDirectory;
        //}

        //#endregion

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="identityName"></param>
        ///// <param name="filename"></param>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //public bool CreatePdf(Guid identityName, string filename, string url)
        //{
        //    var pdfDirectory = Path.Combine(_rootDirectory, identityName.ToString("N"));
        //    if (!CheckDirectory(pdfDirectory)) throw new DirectoryNotFoundException("文件夹创建失败!");
        //    string outputPath = Path.Combine(pdfDirectory, filename);
        //    HtmlToPdfFromUrl(url, outputPath);
        //    return true;
        //}

        ///// <summary>
        ///// 获取实际文件路径
        ///// </summary>
        ///// <param name="identityName"></param>
        ///// <param name="filename"></param>
        ///// <returns></returns>
        //public string GetRealFilePath(Guid identityName, string filename)
        //{
        //    var pdfDirectory = Path.Combine(_rootDirectory, identityName.ToString("N"));
        //    string outputPath = Path.Combine(pdfDirectory, filename);
        //    return File.Exists(outputPath) ? outputPath : string.Empty;
        //}
        ///// <summary>
        ///// 获取实际文件路径
        ///// </summary>
        ///// <param name="identityName"></param>
        ///// <param name="filename"></param>
        ///// <returns></returns>
        //public string GetVirtualFilePath(Guid identityName, string filename)
        //{
        //    var pdfDirectory = Path.Combine(_virtualRootDirectory, identityName.ToString("N"));
        //    string outputPath = Path.Combine(pdfDirectory, filename);
        //    return File.Exists(Utils.MapPath(outputPath)) ? outputPath.Replace("~","") : string.Empty;
        //}

        //#region HTMLTOPdf

        //public ToPdfResult HtmlToPdfFromUrl(string url, string outputPath)
        //{
        //    var html = GetHtml(url);
        //    return HtmlToPdf(html, outputPath);
        //}

        ///// <summary>
        ///// HTML转pdf
        ///// </summary>
        ///// <param name="html">需要转换的html的代码</param>
        ///// <param name="outputPath">保存的路径（带保存的文件名称）</param>
        ///// <returns></returns>
        //public ToPdfResult HtmlToPdf(string html, string outputPath)
        //{
        //    ToPdfResult result = new ToPdfResult() { Flag = false };
        //    PDFNet.Initialize();
        //    var dllurl = GetDllPath();
        //    HTML2PDF.SetModulePath(dllurl);
        //    try
        //    {
        //        using (var doc = new PDFDoc())
        //        {
        //            var converter = new HTML2PDF();
        //            html = html.Replace("font-family:黑体;", "");
        //            var webSite = new HTML2PDF.WebPageSettings();
        //            webSite.SetDefaultEncoding("utf-8");
        //            // Add html data
        //            converter.InsertFromHtmlString(html, webSite);
        //            // Note, InsertFromHtmlString can be mixed with the other Insert methods.
        //            if (converter.Convert(doc))
        //            {
        //                doc.Save(outputPath, SDFDoc.SaveOptions.e_linearized);
        //                result.FilePath = outputPath;
        //                result.TotalPages = doc.GetPageCount();
        //                result.Flag = true;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error(MethodBase.GetCurrentMethod().Name + "html转pdf失败！" + e.Message);
        //        result.Flag = false;
        //    }
        //    PDFNet.Terminate();
        //    _logger.Debug("PDF转换成功");
        //    return result;
        //}

        //#endregion

        //#region 获取HTML值

        //public string GetHtml(string url)
        //{
        //    using (var client = GetHttpClient())
        //    {
        //        return client.GetStringAsync(url).Result;
        //    }
        //}

        //#endregion

        //#region Methods

        //private string GetDllPath()
        //{
        //    if (HostingEnvironment.IsHosted)
        //    {
        //        return Path.Combine(HttpRuntime.AppDomainAppPath, "DLLFile");
        //    }
        //    else
        //    {
        //        //not hosted. For example, run in unit tests
        //        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DLLFile");
        //    }
        //}


        ///// <summary>
        ///// 获取WebApi操作对象(API调用2分钟有效时间，过期无反映表示超时)
        ///// </summary> 
        ///// <returns>WebApi操作对象</returns>
        //private HttpClient GetHttpClient()
        //{

        //    var handler = new HttpClientHandler { AllowAutoRedirect = false };
        //    var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(2) };
        //    // 为JSON格式添加一个Accept报头
        //    //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    httpClient.MaxResponseContentBufferSize = 64 * 1024;//64K
        //    // Add a user-agent header
        //    httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        //    return httpClient;
        //}

        ///// <summary>
        ///// 检查文件夹是否存在，如果不存在，那么创建文件夹
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //private bool CheckDirectory(string path)
        //{
        //    if (!Directory.Exists(path))
        //    {
        //        try
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //private bool FileExists(string filename)
        //{
        //    return File.Exists(filename);
        //}

        //#endregion
    }

    public class ToPdfResult
    {
        /// <summary>
        /// 操作成功与否标记为 true 成功，false 错误
        /// </summary>
        public bool Flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
    }
}
