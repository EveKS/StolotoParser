using System;

namespace StolotoParser.Service
{
    interface IFileManager
    {
        event EventHandler AppendProgressMessage;
        event EventHandler AppendMaxValue;
        event EventHandler AppendValue;

        void DownloadAndWrite(string url, string path, int take, int takeFrom);
        bool IsFolderExist(string directoryFrom);
        bool IsFileExist(string path);
        bool IsMove(string directoriTo);
        string Rename(string oldName);
        int GetMaxDraw(string path);
        string CreateUri(string selectedName);
    }
}