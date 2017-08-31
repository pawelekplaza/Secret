using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Encrypter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private int Offset => Convert.ToInt32(Properties.Resources.Offset);

        public ICommand ReadTextFileCommand => new RelayCommand(() =>
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                ReadFileContent(dialog.FileName);
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
            set { _secretReadKey = value; RaisePropertyChanged(nameof(SecretReadKey)); }
        }

        private string _secondSecretReadKey;    
        public string SecondSecretReadKey
        {
            get { return _secondSecretReadKey ?? ""; }
            set { _secondSecretReadKey = value; RaisePropertyChanged(nameof(SecondSecretReadKey)); }
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

        private void ReadFileContent(string filePath)
        {
            try
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(file))
                {
                    var text = reader.ReadToEnd();
                    var convertedFrom64Base = Convert.FromBase64String(text);
                    var unicodeString = Encoding.Unicode.GetString(convertedFrom64Base);
                    var encrypted = GetEncryptedString(unicodeString);
                    TextRead = encrypted;
                }
            }
            catch (Exception ex)
            {
                TextRead = ex.Message;                  
            }
        }

        private void SaveFileContent(string filePath)
        {
            try
            {
                using (var newFile = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(newFile))
                {
                    var text = GetCryptedString(TextWrite);
                    var bytes = Encoding.Unicode.GetBytes(text);
                    var crypted = Convert.ToBase64String(bytes);
                    writer.Write(crypted);
                }
            }
            catch (Exception ex)
            {
                TextRead = ex.Message;
            }
        }

        private string GetCryptedString(string text)
        {
            try
            {
                var builder = new StringBuilder();
                var offset = GetCharactersSum(SecretWriteKey, SecondSecretWriteKey) + Offset;
                int index = 0;
                foreach (var c in text)
                {
                    var element = Convert.ToChar(((c + offset + GetCharacterOffset(SecretWriteKey, SecondSecretWriteKey, index)) % 512));
                    builder.Append(element);
                    index++;
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {
                TextRead = ex.Message;
                return "";
            }
        }

        private string GetEncryptedString(string text)
        {
            try
            {
                var builder = new StringBuilder();
                var offset = GetCharactersSum(SecretReadKey, SecondSecretReadKey) + Offset;
                int index = 0;
                foreach (var c in text)
                {
                    var currentOffset = offset + GetCharacterOffset(SecretReadKey, SecondSecretReadKey, index);
                    var baseModulo = currentOffset % 512;
                    char character;
                    if (baseModulo > c)
                    {
                        var x = currentOffset / 512;
                        var y = x + 1;
                        var offsetUsedWhileCrypting = y * 512 + c;
                        character = Convert.ToChar(offsetUsedWhileCrypting - currentOffset);
                    }
                    else
                    {
                        var y = currentOffset / 512;
                        var offsetUsedWhileCrypting = y * 512 + c;
                        character = Convert.ToChar(offsetUsedWhileCrypting - currentOffset);
                    }

                    builder.Append(character);
                    index++;
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private int GetCharactersSum(string text1, string text2)
        {
            if (text1 == null || text2 == null)
            {
                return 0;
            }

            int sum = 0;
            int index = 2;
            foreach (var c in text1)
            {
                sum += (c * index++ + 211) / 5;
            }
            foreach (var c in text2)
            {
                sum += (c * index++ + 132) / 4;
            }

            return sum;
        }

        private int GetCharacterOffset(string text1, string text2, int index)
        {
            var c1 = text1.Length > 0 ? text1[(index + 3)% text1.Length] : 'x' + index;
            var c2 = text2.Length > 0 ? text2[(index + 11) % text2.Length] : '$' + index;

            return (7 * c1) % 412 + (11 * c2) % 371;
        }
    }
}
