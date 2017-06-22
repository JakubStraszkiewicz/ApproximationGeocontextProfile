using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using doubleDictionary = System.Collections.Generic.Dictionary<double, double>;

namespace MN3
{
    class File
    {
        public doubleDictionary dictionary = new doubleDictionary();

        public void readFromFile(string name)
        {
            string[] lines = ReadAllLines(name);
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            dictionary = lines.Select(l => l.Split(','))
                              .ToDictionary(a => Double.Parse(a[0], provider), a => Double.Parse(a[1], provider));
        }
        public void writeToFile(string name, doubleDictionary result,doubleDictionary generate)
        {
            StreamWriter streamWriter = new StreamWriter(name);
            int i = 0;
            foreach (KeyValuePair<double, double> pair in result)
            {
                if(i!=generate.Count)
                if (pair.Key == generate.ElementAt(i).Key)
                {
                    streamWriter.WriteLine(pair.Key + ";" + pair.Value + ";" + generate.ElementAt(i).Value);
                    i++;
                }
                else
                    streamWriter.WriteLine(pair.Key + ";" + pair.Value);
            }
            streamWriter.Close();
        }

        public static string[] ReadAllLines(string path)
        {
            List<string> list = new List<string>();
            StreamReader streamReader = new StreamReader(path);
            string item;
            bool first = true;
            while ((item = streamReader.ReadLine()) != null)
            {
                if (first)
                    first = false;
                else
                    list.Add(item);
            }
            return list.ToArray();
        }
    }
}
