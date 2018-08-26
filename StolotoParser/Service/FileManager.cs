using CsQuery;
using StolotoParser.Service.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace StolotoParser.Service
{
    class FileManager : IFileManager
    {
        private IHtmlService _htmlService;

        public event EventHandler AppendProgressMessage;
        public event EventHandler AppendMaxValue;
        public event EventHandler AppendValue;

        public FileManager()
        {
            _htmlService = new HtmlService();
        }

        public bool IsMove(string directoriTo)
        {
            return !string.IsNullOrEmpty(directoriTo);
        }

        bool IFileManager.IsFolderExist(string directoryFrom)
        {
            try
            {
                bool isExist = Directory.Exists(directoryFrom);
                return isExist;
            }
            catch
            {
                return false;
            }
        }

        bool IFileManager.IsFileExist(string path)
        {
            return File.Exists(path);
        }

        string IFileManager.Rename(string oldName)
        {
            if (oldName == "rapido")
            {
                return "Рапидо";
            }
            if (oldName == "top3")
            {
                return "Топ-3";
            }
            if (oldName == "keno")
            {
                return "КЕНО";
            }
            else
            {
                return oldName.Replace('x', '-');
            }
        }

        string IFileManager.CreateUri(string selectedName)
        {
            return string.Format(@"http://www.stoloto.ru/{0}/archive", selectedName);
        }

        void IFileManager.DownloadAndWrite(string url, string path, int take, int takeFrom)
        {
            try
            {
                _htmlService.HttpGetBaseStringContent(url, path, take, takeFrom, GetDraws, ParseAndWriteHtml);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ParseAndWriteHtml(string content, string path, int take)
        {
            try
            {
                CQ cq = CQ.Create(content);

                var container = cq[".month .elem"].Take(take);
                var count = container.Count();

                List<string> info = null;
                if (File.Exists(path))
                {
                    info = GetInfo(path);
                }
                else
                {
                    info = new List<string>(count);
                }

                foreach (var item in container)
                {
                    var draw = string.Empty;
                    var stringNumbers = string.Empty;
                    try
                    {
                        var html = item.InnerHTML;
                        CQ cq2 = CQ.Create(html);

                        //var prize = cq2[".main .prize.with_jackpot"]; // .FirstChild(super);
                        draw = Regex.Replace(cq2[".main .draw"].Text(), @"[\W\D]+", string.Empty);
                        var numbers = cq2[".main .numbers .container.cleared:not(.sorted):not(.sub) b:not(.extra)"];
                        stringNumbers = string.Join(" ", numbers.Select(num => Regex.Replace(num.InnerText, @"[\W\D]+", string.Empty)).ToArray());

                        info.Add(string.Format("{0} _ {1}", draw, stringNumbers).Replace("\r", string.Empty));
                    }
                    catch
                    {
                        throw new Exception(string.Format("Ошибка данных, тираж: {0}, номера: {1}", draw, stringNumbers));
                    }
                }

                WriteInfo(path, info, count);
            }
            catch
            {
                throw new Exception("Ошибка в процессе парсинга данных");
            }
        }

        private List<string> GetInfo(string path)
        {
            List<string> info = new List<string>(2500);

            FileStream fileStream = new FileStream(path, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);

            try
            {
                while (!streamReader.EndOfStream)
                {
                    info.Add(streamReader.ReadLine());
                }
            }
            catch
            {
                throw new Exception("Ошибка в процессе записи файла");
            }
            finally
            {
                streamReader.Dispose();
                fileStream.Dispose();
            }

            return info;
        }

        private void WriteInfo(string path, List<string> info, int oldCount)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            var infoDistinct = info.Distinct().OrderByDescending(s => int.Parse(s.Split('_')[0])).ToList();
            var count = infoDistinct.Count;

            AppendMaxValue(count, EventArgs.Empty);
            AppendValue(0, EventArgs.Empty);
            AppendProgressMessage(string.Format("{0}/{2}/{1}", 0, count, oldCount), EventArgs.Empty);

            try
            {
                StringBuilder sb = new StringBuilder(count);
                var index = 0;
                foreach (var item in infoDistinct)
                {
                    sb.AppendLine(item);

                    index++;
                    AppendProgressMessage(string.Format("{0}/{2}/{1}", index, count, oldCount), EventArgs.Empty);
                    AppendValue(index, EventArgs.Empty);
                }

                streamWriter.Write(sb.ToString());

            }
            catch
            {
                throw new Exception("Ошибка в процессе записи файла");
            }
            finally
            {
                streamWriter.Dispose();
                fileStream.Dispose();
            }
        }

        private List<string> GetDraws(string content)
        {
            List<string> draws = null;
            try
            {
                CQ cq = CQ.Create(content);

                var container = cq[".month .elem"];
                var count = container.Count();

                draws = new List<string>(count);
                foreach (var item in container)
                {
                    var draw = string.Empty;
                    var stringNumbers = string.Empty;
                    try
                    {
                        var html = item.InnerHTML;
                        CQ cq2 = CQ.Create(html);

                        //var prize = cq2[".main .prize.with_jackpot"]; // .FirstChild(super);
                        draw = Regex.Replace(cq2[".main .draw"].Text(), @"[\W\D]+", string.Empty);

                        draws.Add(draw);
                    }
                    catch
                    {
                        //throw new Exception(string.Format("Ошибка данных, тираж: {0}, номера: {1}", draw, stringNumbers));
                    }
                }

            }
            catch
            {
                throw new Exception("Ошибка в процессе парсинга данных");
            }

            return draws;
        }

        int IFileManager.GetMaxDraw(string path)
        {
            int draw = -1;

            FileStream fileStream = new FileStream(path, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);

            try
            {
                var firstLine = streamReader.ReadLine();
                if (!string.IsNullOrEmpty(firstLine))
                {
                    int.TryParse(firstLine.Split('_')[0], out draw);
                }
            }
            catch
            {
                throw new Exception("Что-то пошло не так");
            }
            finally
            {
                streamReader.Dispose();
                fileStream.Dispose();
            }

            return draw;
        }
    }
}
