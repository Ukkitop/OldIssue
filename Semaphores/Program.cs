using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Semaphores
{
    class Program
    {
        private static bool[] oldArray;
        private static Random rnd = new Random(100);
        private static Semaphore[] _pool;
        static void Main(string[] args)
        {
            int count = Int32.Parse(Console.ReadLine());
            oldArray = new bool[count];
            _pool = new Semaphore[count];
            for (int i = 0; i < count; i++)
            {
                _pool[i] = new Semaphore(1, 1);
            }

            for (int i = 0; i < oldArray.Length; i++)
            {
                    oldArray[i] = false;
                Thread t = new Thread(new ParameterizedThreadStart(OldWork));
                t.Start(i);
                //Thread.Sleep(500);
            }
            

        }

        public static void OldWork(object num)
        {
            var number = Int32.Parse(num.ToString());
            while (true)
            {
                Console.WriteLine($"old#{num} talk ");
                Thread.Sleep(rnd.Next(1000, 5000));
                Console.WriteLine($"old#{num} try to eat");
                if (number == 0)
                {
                   _pool[_pool.Length - 1].WaitOne();
                   _pool[number].WaitOne();
                }
                else if (number == _pool.Length - 1)
                {
                    _pool[_pool.Length - 1].WaitOne();
                    _pool[0].WaitOne();
                }
                else
                {
                    _pool[number - 1].WaitOne(); 
                    _pool[number].WaitOne(); 

                }
                
                
                Console.WriteLine($"Old#{number} eating");
                Thread.Sleep(rnd.Next(1000, 5000));
                if (number == 0)
                {
                    _pool[_pool.Length - 1].Release();
                    _pool[number].Release();
                }
                else if (number == _pool.Length - 1)
                {
                    _pool[_pool.Length - 1].Release();
                    _pool[0].Release();
                }
                else
                {
                    _pool[number - 1].Release();
                    _pool[number].Release();

                }
                Console.WriteLine($"Old#{number}finish eating");
            }
        }
    }
}
