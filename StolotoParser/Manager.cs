using StolotoParser.Service;
using System;
using System.Windows.Forms;

namespace StolotoParser
{
    class Manager
    {
        private readonly IMainForm _view;
        private readonly IFileManager _service;
        private readonly IMessageService _messegeService;

        public Manager(IMainForm view, IFileManager service, IMessageService messegeService)
        {
            _view = view;
            _service = service;
            _messegeService = messegeService;

            _view.Download += _view_Download;
            _service.AppendProgressMessage += _service_AppendProgressMessage;
            _service.AppendMaxValue += _service_AppendMaxValue;
            _service.AppendValue += _service_AppendValue;
        }

        private void _service_AppendValue(object sender, EventArgs e)
        {
            if (sender is int)
            {
                _view.SetMessageValue = (int)sender;
            }
        }

        private void _service_AppendMaxValue(object sender, EventArgs e)
        {
            if (sender is int)
            {
                _view.SetMessageMaxValue = (int)sender;
            }
        }

        private void _service_AppendProgressMessage(object sender, EventArgs e)
        {
            if (sender is string)
            {
                _view.SetMessageProgress = (string)sender;
            }
        }

        private void _view_Download(object sender, EventArgs e)
        {
            _view.EnableBtn = false;

            try
            {
                string max = _view.GetMaximum;
                int maxTake = 0;
                if (!string.IsNullOrEmpty(max) && !int.TryParse(max, out maxTake))
                {
                    _messegeService.ShowExclamation("Введите целое, положительное число");
                    _view.EnableBtn = true;
                    return;
                }

                string selectedName = _view.NameChecked;

                if (string.IsNullOrEmpty(selectedName))
                {
                    _messegeService.ShowExclamation("Выбранная один из вариантов");
                    _view.EnableBtn = true;
                    return;
                }
                else
                {
                    selectedName = selectedName.Replace("radio", string.Empty);
                }

                string path = _view.SelectToPath;

                bool isMove = _service.IsMove(path);

                if (!isMove)
                {
                    path = Application.StartupPath;
                }

                bool isFolderExist = _service.IsFolderExist(path);

                if (!isFolderExist && isMove)
                {
                    _messegeService.ShowExclamation("Выбранная папка не найдена");
                    _view.EnableBtn = true;
                    return;
                }

                var url = _service.CreateUri(selectedName);
                path += string.Format(@"\{0}.txt", _service.Rename(selectedName));

                int takeFrom = 0;
                if (maxTake == 0)
                {
                    if (_service.IsFileExist(path))
                    {
                        takeFrom = _service.GetMaxDraw(path);
                        if (takeFrom == -1)
                        {
                            maxTake = 2500;
                        }
                    }
                    else
                    {
                        maxTake = 2500;
                    }
                }

                _service.DownloadAndWrite(url, path, maxTake, takeFrom);
            }
            catch (Exception ex)
            {
                _messegeService.ShowError(ex.Message);
                return;
            }

            _view.EnableBtn = true;
            if (MessageBox.Show("Успешное обновление, продолжить?", "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                Application.Exit();
            }
            else
            {
                _service_AppendValue(0, EventArgs.Empty);
            }
            //_messegeService.ShowMessage("Успешное обновление!");
        }
    }
}
