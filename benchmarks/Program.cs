using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            int benchmarkCount = 20;
            int loopCount = 10000000;
            int ctr = 0;

            for (int j = 0; j < benchmarkCount; j++)
            {
                DateTime[] start = new DateTime[2], stop = new DateTime[2];

                start[0] = DateTime.Now;
                for (int i = 0; i < loopCount; i++)
                {
                    if (i % 5 == 0)
                    {
                        ctr++;
                    }
                }
                stop[0] = DateTime.Now;
                Func<int, int> func = i =>
                {
                    if (i % 5 == 0) return 1;
                    return 0;
                };

                start[1] = DateTime.Now;
                for (int i = 0; i < loopCount; i++)
                {
                    ctr += func(i);
                }
                stop[1] = DateTime.Now;
                Console.WriteLine("simple dt = {0}, func dt = {1}", stop[0] - start[0], stop[1] - start[1]);
            }


            Console.ReadKey();

        }
    }
}
