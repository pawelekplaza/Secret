using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encrypter
{
    public class CryptWorker
    {
        private int Offset => Convert.ToInt32(Properties.Resources.Offset);
        public string Text { get; set; }

        public CryptWorker()
        {
            Text = "";
        }

        public CryptWorker(string text)
        {
            Text = text;            
        }

        public Task<string> GetEncryptedText(Keys keys)
        {
            return Task.Factory.StartNew(() =>
            {
                var convertedFrom64Base = Convert.FromBase64String(Text);
                var unicodeString = Encoding.Unicode.GetString(convertedFrom64Base);
                return GetEncryptedString(unicodeString, keys);
            });
        }

        public Task<string> GetCryptedText(Keys keys)
        {
            return Task.Factory.StartNew(() =>
            {
                var text = GetCryptedString(Text, keys);
                var bytes = Encoding.Unicode.GetBytes(text);
                return Convert.ToBase64String(bytes);
            });
        }

        private string GetCryptedString(string text, Keys keys)
        {
            try
            {
                var builder = new StringBuilder();
                var offset = GetCharactersSum(keys.FirstKey, keys.SecondKey) + Offset;
                int index = 0;
                foreach (var c in text)
                {
                    var element = Convert.ToChar(((c + offset + GetCharacterOffset(keys.FirstKey, keys.SecondKey, index)) % 512));
                    builder.Append(element);
                    index++;
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {                
                return ex.Message;
            }
        }

        private string GetEncryptedString(string text, Keys keys)
        {
            try
            {
                var builder = new StringBuilder();
                var offset = GetCharactersSum(keys.FirstKey, keys.SecondKey) + Offset;
                int index = 0;
                foreach (var c in text)
                {
                    var currentOffset = offset + GetCharacterOffset(keys.FirstKey, keys.SecondKey, index);
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
            int index = 22;
            foreach (var c in text1)
            {
                sum += (c * index++ + 7) / 9;
            }
            foreach (var c in text2)
            {
                sum += (c * index++ + 21) / 4;
            }

            return sum;
        }

        private int GetCharacterOffset(string text1, string text2, int index)
        {
            var c1 = text1.Length > 0 ? text1[(((index + 13) % text1.Length) * 7) % text1.Length] : 'x' + index;
            var c2 = text2.Length > 0 ? text2[(((index + 11) % text2.Length) * 3) % text2.Length] : '$' + index;

            return (8 * c1) % 411 + (13 * c2) % 341;
        }
    }
}
