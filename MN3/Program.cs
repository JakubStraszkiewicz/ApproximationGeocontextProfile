using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using doubleDictionary = System.Collections.Generic.Dictionary<double, double>;


namespace MN3
{
    /*
    Parametry wywołania: 
    args[0] - nazwa pliku z ktorego wczytujemy dane
    args[1] - 
                1 -> generator wezłów bedącymi zerami wielomianu czebyszewa (potrzebny args[2] - ilość węzłów)
                2 -> generator węzłów o stały interwał (potrzeby args[2] - interwał)
                3 -> generator ktory generuje wiecej wezlow jesli bardziej stroma trasa

    */
    class Program
    {
        static void Main(string[] args)
        {
            File file = new File();
            Calculation calc = new Calculation();
            Generators generators = new Generators();
            file.readFromFile(args[0]);

            doubleDictionary generateDictionary = new doubleDictionary();
            doubleDictionary wyniki = new doubleDictionary();

            switch (args[1])
             {
                 case "1":
                     generateDictionary = generators.chebyshev(file.dictionary,Int32.Parse(args[2]));
                     break;
                 case "2":
                     generateDictionary = generators.oneForAFew(Int32.Parse(args[2]), file.dictionary);
                     break;
                 case "3":
                     generateDictionary = generators.moreIfVertical(file.dictionary);
                     break;
            }          
            
            wyniki = calc.algorythmCSI(generateDictionary,file.dictionary);
            file.writeToFile("wynikCSI.csv", wyniki, generateDictionary);
            Console.WriteLine("CSI:");
            Console.WriteLine(calc.compareResult(wyniki, file.dictionary)[0] + ";" + calc.compareResult(wyniki, file.dictionary)[1]);

            wyniki = calc.Lagrange(generateDictionary, file.dictionary);
            file.writeToFile("wynikLagrange.csv", wyniki, generateDictionary);
            Console.WriteLine("Lagrange:");
            Console.WriteLine(calc.compareResult(wyniki, file.dictionary)[0] + ";"+calc.compareResult(wyniki, file.dictionary)[1]);
           
            Console.ReadKey();
        }
    }    
    
}
