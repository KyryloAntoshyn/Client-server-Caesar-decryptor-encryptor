using System;
using System.Collections.Generic;
using System.Linq;

namespace PR_MultiThreadedServer
{
    class FrequencyAnalizator
    {
        private Dictionary<char, double> frequenciesDict; // Буква и её частота
        private List<char> engAlphabet;
        private const double difference = 0.0025; // Допустимое отклонение
        private Dictionary<char, double> commonFrequencies;
        public FrequencyAnalizator()
        {
            frequenciesDict = new Dictionary<char, double>();
            engAlphabet = new List<char>
            { 'A', 'B', 'C', 'D', 'E', 'F',
                'G', 'H', 'I', 'J', 'K',
                'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z'
            };
            commonFrequencies = new Dictionary<char, double>()
            { 
                { 'a', 0.0817 },
                { 'b', 0.0149 },
                { 'c', 0.0278 },
                { 'd', 0.0425 },
                { 'e', 0.1270 },
                { 'f', 0.0223 },
                { 'g', 0.0202 },
                { 'h', 0.0609 },
                { 'i', 0.0697 },
                { 'j', 0.0015 },
                { 'k', 0.0077 },
                { 'l', 0.0403 },
                { 'm', 0.0241 },
                { 'n', 0.0675 },
                { 'o', 0.0751 },
                { 'p', 0.0193 },
                { 'q', 0.0010 },
                { 'r', 0.0599 },
                { 's', 0.0633 },
                { 't', 0.0906 },
                { 'u', 0.0276 },
                { 'v', 0.0098 },
                { 'w', 0.0236 },
                { 'x', 0.0015 },
                { 'y', 0.0197 },
                { 'z', 0.0007 }
        };
        }
        public void CalculateFrequencies(string sourceText)
        {
            Dictionary<char, double> dictLetters = new Dictionary<char, double>();
            foreach (char c in sourceText)
            {
                if (Char.IsLetter(c) && engAlphabet.Contains(Char.ToUpper(c))) // Если буква не английского алфавита, то её в словарь не заношу
                {
                    if (dictLetters.ContainsKey(c)) // Если уже существует элемент с таким ключом
                    {
                        double lastValue = dictLetters[c];
                        dictLetters[c] = lastValue + 1; // Инкремент значения
                    }
                    else
                        dictLetters.Add(c, 1); // Добавление нового элемента
                }
            }
            // Совместить большие символы с малыми в словаре
            bool flag = true;
            while (flag && dictLetters.Count != 0)
            {
                foreach (KeyValuePair<char, double> kvp in dictLetters)
                {
                    if (Char.IsUpper(kvp.Key))
                    {
                        dictLetters.Remove(kvp.Key);
                        if (dictLetters.ContainsKey(Char.ToLower(kvp.Key)))
                            dictLetters[Char.ToLower(kvp.Key)] += 1;
                        else
                            dictLetters.Add(Char.ToLower(kvp.Key), kvp.Value);
                        break;
                    }
                    flag = false;
                }
                // Пока есть большие символы продолжаю алгоритм
                foreach (KeyValuePair<char, double> kvp in dictLetters)
                {
                    if (Char.IsUpper(kvp.Key))
                    {
                        flag = true;
                        break;
                    } 
                }
            }
            // Добавляю нулевые частоты
            for (int i = 97; i <= 122; i++)
            {
                if (!dictLetters.ContainsKey((char)i))
                    dictLetters.Add((char)i, 0);
            }

            // Сортировка в алфавитном порядке
            dictLetters = dictLetters.OrderBy(k => k.Key).
                ToDictionary(process => process.Key, process => process.Value);
            // Подсчёт общего количества букв
            double generalCountLetters = 0;
            foreach (KeyValuePair<char, double> kvp in dictLetters)
                generalCountLetters += kvp.Value;
            // Формирую словарь частот
            foreach (KeyValuePair<char, double> kvp in dictLetters)
                frequenciesDict.Add(kvp.Key, Math.Round(kvp.Value / generalCountLetters, 4));
        }
        public override string ToString()
        {
            string str = String.Empty;
            foreach (KeyValuePair<char, double> kvp in frequenciesDict)
            {
                char letterName = kvp.Key;
                // Посылаю букву и её частоту вхождения
                str += letterName.ToString() + ' ' + kvp.Value + '\n';
            }
            return str;
        }

        // Частотный анализ для разгадки ключа
        public int FrequencyAnalisys()
        {
            List<int> shifts = new List<int>(); // Список сдвигов
                                                // Проверка, является ли текст зашифрованым - сравниваю частоты букв с частотами, встречающимися наиболее часто в англ языке
            foreach (KeyValuePair<char, double> current in frequenciesDict)
            {
                foreach (KeyValuePair<char, double> standart in commonFrequencies)
                {
                    if (current.Value >= (standart.Value - difference) &&
                        current.Value <= (standart.Value + difference))
                        shifts.Add(current.Key - standart.Key);
                }
            }

            // Нахожу смещение как наиболее часто встречающийся элемент в списке смещений
            shifts.Sort();
            int count = 0; // Количество похожих
            int maxCount = count; // Максимальное кол-во похожих
            int key = 0;
            for (int i = 1; i < shifts.Count; i++)
            {
                if (shifts[i - 1] == shifts[i])
                    count++;
                else
                {
                    if (count > maxCount) // Беру элемент с наибольшим вхождением в список
                    {
                        maxCount = count;
                        key = shifts[i - 1];
                    }
                    count = 0;
                }
            }
            if (key < 0)
                key += 26;
            return key;
        }
    }
}