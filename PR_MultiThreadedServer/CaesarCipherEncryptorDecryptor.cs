using System;
using System.Collections.Generic;
using System.Linq;

namespace PR_MultiThreadedServer
{
    public class CaesarCipherEncryptorDecryptor
    {
        private int shift;
        private string dataToEncrypt;
        private List<char> engAlphabet;
        public CaesarCipherEncryptorDecryptor(string dataToEncrypt, int shift)
        {
            this.dataToEncrypt = dataToEncrypt;
            this.shift = shift;
            engAlphabet = new List<char>
            { 'A', 'B', 'C', 'D', 'E', 'F',
                'G', 'H', 'I', 'J', 'K',
                'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z'
            };
        }

        // Шифровка символа по ключу
        public string DoSymbolEncryption(char symbol)
        {
            // Когда пользователь вводит > 26 или отрицательный сдвиги
            if (shift > 26 || shift < 0)
                Math.DivRem(shift, 26, out shift);

            char letter = symbol;
            letter = (char)(letter + shift);

            if (Char.IsUpper(symbol))
            {
                if (letter > 'Z')
                    letter = (char)(letter - 26);
                else if (letter < 'A')
                    letter = (char)(letter + 26);
            }
            else
            {
                if (letter > 'z')
                    letter = (char)(letter - 26);
                else if (letter < 'a')
                    letter = (char)(letter + 26);
            }
            return letter.ToString();
        }
        // Дешифровка символа по ключу
        public string DoSymbolDecryption(char symbol)
        {
            // Когда пользователь вводит > 26 или отрицательный сдвиги
            if (shift > 26 || shift < 0)
                Math.DivRem(shift, 26, out shift);

            char letter = symbol;
            letter = (char)(letter - shift);

            if (Char.IsUpper(symbol))
            {
                if (letter > 'Z')
                    letter = (char)(letter - 26);
                else if (letter < 'A')
                    letter = (char)(26 + letter);
            }
            else
            {
                if (letter > 'z')
                    letter = (char)(letter - 26);
                else if (letter < 'a')
                    letter = (char)(26 + letter);
            }
            return letter.ToString();
        }
        // Общий метод шифровальщика
        public string EncryptionDecryption(bool encryptionDecryption)
        {
            string result = String.Empty;
            if (!(shift % 26 == 0) || !(shift == 0)) // Если сдвиг НЕ на ноль символов и НЕ на кратное 26
            {
                char[] textArray = dataToEncrypt.ToArray();
                // Делаю шифровку
                if (encryptionDecryption)
                {
                    for (int i = 0; i < textArray.Length; i++)
                    {
                        char symbol = textArray[i];
                        if (Char.IsLetter(symbol) && engAlphabet.Contains(Char.ToUpper(symbol))) // Отбрасываю другие знаки и шивровка только для букв англ. алфавита
                            result += DoSymbolEncryption(symbol);
                        else
                            result += symbol; // Пробелы и другие символы
                    }
                }
                // Дешифровка
                else
                {
                    for (int i = 0; i < textArray.Length; i++)
                    {
                        char symbol = textArray[i];
                        if (Char.IsLetter(symbol) && engAlphabet.Contains(Char.ToUpper(symbol)))
                            result += DoSymbolDecryption(symbol);
                        else
                            result += symbol;
                    }
                }
            }
            else // Нету смысла делать сдвиг (сдвиг или на ноль или кратное 26)
                result = dataToEncrypt;
            return result;
        }
    }
}