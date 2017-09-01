using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Encrypter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {        
        private string _readFilePath = "";

        public ICommand ReadTextFileCommand => new RelayCommand(() =>
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                _readFilePath = dialog.FileName;
                ReadFileContent(_readFilePath);
            }
        });

        public ICommand SaveFileCommand => new RelayCommand(() =>
        {
            var dialog = new SaveFileDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                SaveFileContent(dialog.FileName);
            }
        });

        private string _textRead;
        public string TextRead
        {
            get { return _textRead ?? ""; }
            set { _textRead = value; RaisePropertyChanged(nameof(TextRead)); }
        }

        private string _textWrite;
        public string TextWrite
        {
            get { return _textWrite ?? ""; }
            set { _textWrite = value; RaisePropertyChanged(nameof(TextWrite)); }
        }

        private string _secretReadKey;
        public string SecretReadKey
        {
            get { return _secretReadKey ?? ""; }
            set
            {
                _secretReadKey = value;
                ReadFileContent(_readFilePath);
                RaisePropertyChanged(nameof(SecretReadKey));
            }
        }

        private string _secondSecretReadKey;    
        public string SecondSecretReadKey
        {
            get { return _secondSecretReadKey ?? ""; }
            set
            {
                _secondSecretReadKey = value;
                ReadFileContent(_readFilePath);
                RaisePropertyChanged(nameof(SecondSecretReadKey));
            }
        }

        private string _secretWriteKey;
        public string SecretWriteKey
        { 
            get { return _secretWriteKey ?? ""; }
            set { _secretWriteKey = value; RaisePropertyChanged(nameof(SecretWriteKey)); }
        }

        private string _secondSecretWriteKey;   
        public string SecondSecretWriteKey
        {
            get { return _secondSecretWriteKey ?? ""; }
            set { _secondSecretWriteKey = value; RaisePropertyChanged(nameof(SecondSecretWriteKey)); }
        }


        public void InvokeReadTextFileCommand()
        {
            ReadTextFileCommand.Execute(null);
        }

        public void InvokeSaveFileCommand()
        {
            SaveFileCommand.Execute(null);
        }

        private async void ReadFileContent(string filePath)
        {
            try
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(file))
                {
                    var text = reader.ReadToEnd();
                    var worker = new CryptWorker(text);
                    TextRead = await worker.GetEncryptedText(new Keys { FirstKey = SecretReadKey, SecondKey = SecondSecretReadKey });                    
                }
            }
            catch (ArgumentException)
            {
                TextRead = "There is no file loaded to view its content.";
            }
            catch (Exception ex)
            {
                TextRead = ex.Message;                  
            }
        }

        private async void SaveFileContent(string filePath)
        {
            try
            {
                using (var newFile = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(newFile))
                {
                    var worker = new CryptWorker(TextWrite);
                    writer.Write(await worker.GetCryptedText(new Keys { FirstKey = SecretWriteKey, SecondKey = SecondSecretWriteKey }));
                }
            }
            catch (Exception ex)
            {
                TextRead = ex.Message;
            }
        }        
    }
}
