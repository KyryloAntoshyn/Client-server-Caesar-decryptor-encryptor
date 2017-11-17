using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR_Client_CaesarCipher
{
    class DiagramBuilder
    {
        public List<Letter> LetterCollection { get; set; }
        public string DataForCollection { get; set; }
        public DiagramBuilder()
        {
            LetterCollection = new List<Letter>();
            DataForCollection = "";
        }
        public DiagramBuilder(List<Letter> LetterCollection, string DataForCollection)
        {
            this.DataForCollection = DataForCollection;
            this.LetterCollection = LetterCollection;
            CreateCollection();
        }
        public void CreateCollection()
        {
            string[] pairs = DataForCollection.Split('\n');
            for (int i = 0; i < pairs.Length - 1; i++)
            {
                string[] tmpStr = pairs[i].Split(' ');

                string name = tmpStr[0];
                double frequency = double.Parse(tmpStr[1]);

                LetterCollection.Add(new Letter(name, frequency));
            }
        }
    }
}