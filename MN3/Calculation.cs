using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using doubleDictionary = System.Collections.Generic.Dictionary<double, double>;
using System.Linq.Expressions;

namespace MN3
{
    class Calculation
    {
        
        private Func<int, double[], double> µ = (j, h) => h[j] / (h[j] + h[j + 1]);
        private Func<int, double[], double> λ = (j, h) => h[j + 1] / (h[j] + h[j + 1]);

        private Func<int, double[], double[], double[], double> b = (j,M,y,h) => 
        (y[j + 1] - y[j]) / h[j + 1] - ((2 * M[j] + M[j + 1]) / 6 * h[j + 1]);

        private Func<int, double[], double[], double> delta = (j, y, h) =>
           6 / (h[j] + h[j + 1]) * (((y[j + 1] - y[j]) / h[j + 1]) - ((y[j] - y[j - 1]) / h[j]));

        public doubleDictionary Lagrange(doubleDictionary coordinates, doubleDictionary all)
        {
            double omega = 1, rest = 0, denominator = 1;
            doubleDictionary result = new doubleDictionary();
            for (int i = 0; i < all.Count; i++)
            {
                foreach (KeyValuePair<double, double> pair in coordinates)
                {
                    omega *= (all.ElementAt(i).Key - pair.Key);
                    foreach (KeyValuePair<double, double> pair2 in coordinates)
                        if (pair.Key != pair2.Key)
                            denominator *= (pair.Key - pair2.Key);
                    if(all.ElementAt(i).Key != pair.Key)
                    denominator *= (all.ElementAt(i).Key - pair.Key);
                    rest += pair.Value / denominator;
                    denominator = 1;
                }             
                if(omega*rest!=0)
                    result.Add(all.ElementAt(i).Key , omega * rest);
                else
                    result.Add(all.ElementAt(i).Key, all.ElementAt(i).Value);
                omega = 1;
                rest = 0;
            }
            return result;
        }

        public doubleDictionary algorythmCSI(doubleDictionary coordinates, doubleDictionary all)
        {
            int N = coordinates.Count;
            double[][] coefficients = new double[N+1][];
            for (int i = 0; i < N+1; i++)
                coefficients[i] = new double[N+1];

            double[] h = new double[N];
            for (int j = 0; j < N - 1; j++)
                h[j+1] = coordinates.ElementAt(j+1).Key - coordinates.ElementAt(j).Key;

            double[] δ = new double[N+1];
            for (int j = 1; j < N - 1; j++)
                δ[j] = delta(j, coordinates.Values.ToArray<double>(), h);
            δ[0] = 0;
            δ[N] = 0;

            double[] M = new double[N + 1];

            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < N + 1; j++)
                    switch (i - j)
                    {
                        case 0:
                            coefficients[i][j] = 2;
                            break;
                        case 1:
                            if (i == N || i == N - 1)
                                coefficients[i][j] = 0;
                            else
                                coefficients[i][j] = µ(i, h);
                            break;
                        case -1:
                            if (i == 0 || i == N-1)
                                coefficients[i][j] = 0;
                            else
                                coefficients[i][j] = λ(i, h);
                            break;
                        default:
                            coefficients[i][j] = 0;
                            break;
                    }
            }
            M = metodaBezpodrednialepsza(coefficients, N + 1, δ);
            
            double[][] abcd = new double[N][];
            for (int i = 0; i < N ; i++)
                abcd[i] = new double[4];

            for (int j = 0; j < N - 1 ; j++)
            {
                abcd[j][0] = coordinates.ElementAt(j).Value;
                abcd[j][1] = b(j, M, coordinates.Values.ToArray<double>(), h);
                abcd[j][2] = M[j] / 2;
                abcd[j][3] = (M[j + 1] - M[j]) / 6 / h[j + 1];
            }

            doubleDictionary result = new doubleDictionary();
            for(int i=0;i<all.Count;i++)
            {
                for(int j=0;j<N;j++)
                {
                    if (j != N - 2)
                    {
                        if (all.ElementAt(i).Key >= coordinates.ElementAt(j).Key
                           && all.ElementAt(i).Key <= coordinates.ElementAt(j + 1).Key)
                        {
                            result.Add(all.ElementAt(i).Key, abcd[j][0] + abcd[j][1] * (all.ElementAt(i).Key - coordinates.ElementAt(j).Key)
                                + abcd[j][2] * Math.Pow(all.ElementAt(i).Key - coordinates.ElementAt(j).Key, 2)
                                + abcd[j][3] * Math.Pow(all.ElementAt(i).Key - coordinates.ElementAt(j).Key, 3));
                            break;
                        }
                    }
                    else
                    {
                        result.Add(all.ElementAt(i).Key, abcd[N-1][0] + abcd[j][1] * (all.ElementAt(i).Key - coordinates.ElementAt(N-1).Key)
                                 + abcd[N-1][2] * Math.Pow(all.ElementAt(i).Key - coordinates.ElementAt(N-1).Key, 2)
                                 + abcd[N-1][3] * Math.Pow(all.ElementAt(i).Key - coordinates.ElementAt(N-1).Key, 3));
                        break;
                    }               
                }
            }
            return result;
        }

        public double[] compareResult(doubleDictionary result, doubleDictionary all)
        {
            //0 to srednia, 1 to odchylenie standardowe
            double[] compared = new double[2] { 0, 0 };
            
            for(int i=0;i<all.Count;i++)
                compared[0] += Math.Abs(all.ElementAt(i).Value - result.ElementAt(i).Value);
            compared[0] /= all.Count;

            for (int i = 0; i < all.Count - 1; i++)
                compared[1] += Math.Pow(Math.Abs(Math.Abs(all.ElementAt(i).Value - result.ElementAt(i).Value) - compared[0]), 2);
            compared[1] = Math.Sqrt(compared[1] / (all.Count - 1));

            return compared;
        }

        public double[] metodaBezpodrednialepsza(double[][] tablica, int N, double[] wyrazyWolne)
        {
            double[] poczWolne = new double[N];
            for (int i = 0; i < N; i++)
                poczWolne[i] = wyrazyWolne[i];
            double[][] poczTab = new double[N][];
            for (int i = 0; i < N; i++)
                poczTab[i] = new double[N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    poczTab[i][j] = tablica[i][j];

            double[] wyrazyWolne1 = new double[N];
            wyrazyWolne1[0] = wyrazyWolne[0];
            double[] wyniki = new double[N];
            double[][] tablica1 = new double[N][];
            for (int i = 0; i < N; i++)
                tablica1[i] = new double[N];
            for (int i = 0; i < N; i++)
                tablica1[0][i] = tablica[0][i];
            //1 etap
            for (int k = 1; k < N; k++)
            {
                for (int i = k; i < N; i++)
                {
                    for (int j = k; j < N; j++)
                    {
                        tablica1[i][j] = tablica[i][j] - ((tablica[i][k - 1] / tablica[k - 1][k - 1]) * tablica[k - 1][j]);
                    }
                    wyrazyWolne1[i] = wyrazyWolne[i] - ((tablica[i][k - 1] / tablica[k - 1][k - 1]) * wyrazyWolne[k - 1]);
                }

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                        tablica[i][j] = tablica1[i][j];
                }
                for (int i = 0; i < N; i++)
                    wyrazyWolne[i] = wyrazyWolne1[i];

            }
            // 2 etap
            wyniki[N - 1] = wyrazyWolne1[N - 1] / tablica1[N - 1][N - 1];
            for (int i = N - 1; i > 0; i--)
            {
                double pom = 0;
                for (int j = i; j < N; j++)
                {
                    pom += tablica1[i - 1][j] * wyniki[j];
                }
                wyniki[i - 1] = (wyrazyWolne1[i - 1] - pom) / tablica1[i - 1][i - 1];
            }
            return wyniki;
        }
    }
}
