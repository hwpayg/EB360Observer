using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EB360Observer.Common
{
    public class HttpClientDownloader
    {
        private static readonly ILog Loger = LogManager.GetLogger("HttpClientDownloader");
        public string Download(string url, string method, string encode = "gb2312")
        {
            int statusCode = 0;
            var result = string.Empty;
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest httpWebRequest = GetHttpWebRequest(url, method);
                response = (HttpWebResponse)httpWebRequest.GetResponse();
                statusCode = (int)response.StatusCode;
                result = HandleResponse(Encoding.GetEncoding(encode), response, statusCode);
            }
            catch (Exception ex)
            {
                Loger.Fatal("请求资源失败：", ex);
            }
            finally
            {
                response.Close();
            }
            return result;
        }

        private bool StatusAccept(ICollection<int> acceptStatCode, int statusCode)
        {
            return acceptStatCode.Contains(statusCode);
        }

        private HttpWebRequest GetHttpWebRequest(string url, string method)
        {

            HttpWebRequest httpWebRequest = SelectRequestMethod(url, method);
            Random rnd = new Random();
            httpWebRequest.UserAgent = UserAgentList[rnd.Next(UserAgentList.Count() - 1)];

            httpWebRequest.Headers.Add("Accept-Encoding", "gzip");

            httpWebRequest.Timeout = 30000;

            return httpWebRequest;
        }

        public List<string> UserAgentList
        {
            get
            {
                return new List<string>()
                {
                    "User-Agent:Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                    "User-Agent:Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                    "User-Agent:Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0",
                    "User-Agent:Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0",
                    "User-Agent:Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
                    "User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
                    "User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
                    "User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; TencentTraveler 4.0)",
                    "User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)",
                    "User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)"
                };
            }
        }

        private HttpWebRequest SelectRequestMethod(string url, string method)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = method.ToString();
            return webrequest;
        }

        private string HandleResponse(Encoding charset, HttpWebResponse response, int statusCode)
        {
            string content = GetContent(charset, response);
            return content;
        }

        private string GetContent(Encoding charset, HttpWebResponse response)
        {
            byte[] contentBytes = GetContentBytes(response);

            if (charset == null)
            {
                Encoding htmlCharset = GetHtmlCharset(response.ContentType, contentBytes);
                if (htmlCharset != null)
                {
                    return htmlCharset.GetString(contentBytes);
                }

                return Encoding.Default.GetString(contentBytes);
            }
            return charset.GetString(contentBytes);
        }

        private byte[] GetContentBytes(HttpWebResponse response)
        {
            Stream stream = null;

            //GZIIP处理  
            if (response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
            {
                //开始读取流并设置编码方式
                var tempStream = response.GetResponseStream();
                if (tempStream != null) stream = new GZipStream(tempStream, CompressionMode.Decompress);
            }
            else
            {
                //开始读取流并设置编码方式  
                stream = response.GetResponseStream();
            }

            MemoryStream resultStream = new MemoryStream();
            if (stream != null)
            {
                stream.CopyTo(resultStream);
                return resultStream.StreamToBytes();
            }
            return null;
        }

        private Encoding GetHtmlCharset(string contentType, byte[] contentBytes)
        {
            // charset
            // 1、encoding in http header Content-Type
            string value = contentType;
            var encoding = UrlUtils.GetEncoding(value);
            if (encoding != null)
            {
                return encoding;
            }
            // use default charset to decode first time
            Encoding defaultCharset = Encoding.Default;
            string content = defaultCharset.GetString(contentBytes);
            string charset = null;
            try
            {
                return Encoding.GetEncoding(string.IsNullOrEmpty(charset) ? "UTF-8" : charset);
            }
            catch
            {
                return Encoding.Default;
            }
        }
    }
    public static class HttpConstant
    {
        public static class Method
        {
            public static readonly string Get = "GET";

            public static readonly string Head = "HEAD";

            public static readonly string Post = "POST";

            public static readonly string Put = "PUT";

            public static readonly string Delete = "DELETE";

            public static readonly string Trace = "TRACE";

            public static readonly string Connect = "CONNECT";
        }

        public static class Header
        {
            public static readonly string Referer = "Referer";
            public static readonly string UserAgent = "User-Agent";
        }
    }
    public class UrlUtils
    {
        private static readonly Regex PatternForCharset = new Regex("charset\\s*=\\s*['\"]*([^\\s;'\"]*)");

        /// <summary>
        ///  
        /// </summary>
        /// <param name="url"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string CanonicalizeUrl(string url, string refer)
        {
            Uri bas = new Uri(refer);

            // workaround: java resolves '//path/file + ?foo' to '//path/?foo', not '//path/file?foo' as desired
            //if (url.StartsWith("?"))
            //	url = bas.PathAndQuery + url;

            Uri abs = new Uri(bas, url);

            return abs.AbsoluteUri;
        }

        //public static string getHost(string url)
        //{
        //	string host = url;
        //	int i = StringUtils.ordinalIndexOf(url, "/", 3);
        //	if (i > 0)
        //	{
        //		host = StringUtils.substring(url, 0, i);
        //	}
        //	return host;
        //}

        public static string RemoveProtocol(string url)
        {
            return Regex.Replace(url, "[\\w]+://", "", RegexOptions.IgnoreCase);
        }

        public static string GetDomain(string url)
        {
            string domain = RemoveProtocol(url);
            int i = domain.IndexOf("/", 1, StringComparison.Ordinal);
            if (i > 0)
            {
                domain = domain.Substring(0, i);
            }
            return domain;
        }


        public static Encoding GetEncoding(string contentType)
        {
            Match match = PatternForCharset.Match(contentType);

            if (!string.IsNullOrEmpty(match.Value))
            {
                string charset = match.Value.Replace("charset=", "");
                try
                {
                    return Encoding.GetEncoding(charset);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
    public static class StreamExtensions
    {
        public static byte[] StreamToBytes(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
    }
}
