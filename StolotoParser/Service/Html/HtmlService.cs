using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StolotoParser.Service.Html
{
    class HtmlService : IHtmlService
    {
        void IHtmlService.HttpGetBaseStringContent(string url, string path, int take, int takeFrom, Func<string, List<string>> getDraws, Action<string, string, int> writeContent)
        {
            try
            {
                var takeTo = 0;
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml";
                client.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                client.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1";

                try
                {
                    var responseStream = new GZipStream(client.OpenRead(url), CompressionMode.Decompress);
                    var reader = new StreamReader(responseStream);

                    var content = reader.ReadToEnd();
                    takeTo = getDraws(content)
                        .Select(int.Parse)
                        .Max();

                    responseStream.Dispose();
                    reader.Dispose();
                }
                catch
                {
                    throw new Exception("Сервер вернул не корректные данные");
                }
                finally
                {
                    client.Dispose();
                }

                if (take != 0)
                {
                    takeFrom = takeTo - take;
                    if (takeFrom < 0)
                        takeFrom = 0;
                }

                HttpGetStringContent(url, path, takeTo, takeFrom, writeContent);
            }
            catch
            {
                throw new Exception("Ошибка в процессе скачивания");
            }
        }

        private string UrlSetRequest(string url, int takeFrom, int takeTo)
        {
            var date = DateTime.Now;
            var dateTo = date.ToString("dd.MM.yyyy");
            var dateFrom = date.AddDays(-7).ToString("dd.MM.yyyy");
            return string.Format("{0}?from={1}&to={2}&firstDraw={3}&lastDraw={4}&mode=draw",
                url, dateFrom, dateTo, takeFrom, takeTo);
        }

        void HttpGetStringContent(string url, string path, int takeTo, int takeFrom, Action<string, string, int> writeContent)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml";
                client.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                client.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1";

                try
                {
                    url = UrlSetRequest(url, takeFrom, takeTo);
                    var responseStream = new GZipStream(client.OpenRead(url), CompressionMode.Decompress);
                    var reader = new StreamReader(responseStream);

                    var content = reader.ReadToEnd();
                    writeContent(content, path, takeTo - takeFrom);

                    responseStream.Dispose();
                    reader.Dispose();
                }
                catch
                {
                    throw new Exception("Сервер вернул не корректные данные");
                }
                client.Dispose();
            }
            catch
            {
                throw new Exception("Ошибка в процессе скачивания");
            }
        }
    }
}
