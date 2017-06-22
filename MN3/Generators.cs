using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using doubleDictionary = System.Collections.Generic.Dictionary<double, double>;

namespace MN3
{
    class Generators
    {
        public doubleDictionary oneForAFew(int interval, doubleDictionary all)
        {
            doubleDictionary result = new doubleDictionary();
            int i = 0;

            foreach (KeyValuePair<double, double> pair in all)
            {
                if (i % interval == 0)
                    result.Add(pair.Key, pair.Value);
                i++;
            }

            if(512%interval!=1)
                result.Add(all.ElementAt(all.Count()-1).Key, all.ElementAt(all.Count()-1).Value);

            return result;
        }

        public doubleDictionary moreIfVertical(doubleDictionary all)
        {
            doubleDictionary result = new doubleDictionary();
            double srednia = 0, odchylenie = 0;
            double[] roznice = new double[all.Count];
            for (int i = 0; i < all.Count - 1; i++)
                roznice[i] = Math.Abs(all.ElementAt(i + 1).Value - all.ElementAt(i).Value);
            for (int i = 0; i < all.Count - 1; i++)
                srednia += roznice[i];
            srednia = srednia / (all.Count-1);

            for (int i = 0; i < all.Count - 1; i++)
                odchylenie += Math.Pow(Math.Abs(roznice[i] - srednia), 2);
            odchylenie = Math.Sqrt(odchylenie / (all.Count - 1));

            int ile_bez = 0;
            result.Add(all.ElementAt(0).Key, all.ElementAt(0).Value);
            for (int i=0;i<all.Count-1;i++)
            {
                if (Math.Abs(all.ElementAt(i + 1).Value - all.ElementAt(i).Value) > odchylenie)
                    result.Add(all.ElementAt(i).Key, all.ElementAt(i).Value);
                else
                ile_bez++;
                if (ile_bez % 10 == 0)
                {
                    result.Add(all.ElementAt(i).Key, all.ElementAt(i).Value);
                    ile_bez = 0;
                }
            }

            result.Add(all.ElementAt(all.Count - 1).Key, all.ElementAt(all.Count - 1).Value);
            return result;
        }


        public doubleDictionary chebyshev(doubleDictionary all,int amount)
        {
            doubleDictionary result = new doubleDictionary();
            int a = 0, b = all.Count()-1;
            double tmp,x;
            int x1;
            for (int i = amount-1; i >= 0; i--)
            {
                tmp = Math.Cos(((2 * i + 1) / (2 * (double)(amount - 1) + 2)) * Math.PI);
                x = ((b + a) / 2) + ((b - a) / 2 * tmp);
                x1 = (int)x;
                result.Add(all.ElementAt(x1).Key, all.ElementAt(x1).Value);
            }
            
            return result;
        }
    }
}
