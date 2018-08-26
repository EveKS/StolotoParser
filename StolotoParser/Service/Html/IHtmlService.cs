using System;
using System.Collections.Generic;

namespace StolotoParser.Service.Html
{
    interface IHtmlService
    {
        void HttpGetBaseStringContent(string url, string path, int take, int takeFrom,Func<string, List<string>> getBaseContent, Action<string, string, int> writeContent);
    }
}