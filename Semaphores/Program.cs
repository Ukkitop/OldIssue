using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Semaphores
{
    internal class Program
    {
        public enum OldState
        {
            Eat,
            Wait,
            Talk
        }

        private struct Old
        {
            public OldState state { get; set; }
        }

        struct Fork
        {
            public bool isTaken { get; set; }
        }

        private static Fork[] forkArray;
        private static Old[] oldArray;
        private static Random rnd = new Random(100);
        private static Semaphore semaphore;

        private static void Main(string[] args)
        {
            var count = int.Parse(Console.ReadLine());
            oldArray = new Old[count];
            forkArray = new Fork[count];
            int semaphoreNumber = Int32.MaxValue;
            for (var i = 0; i < forkArray.Length; i++)
            {
                forkArray[i].isTaken = false;
                //Thread.Sleep(500);
            }
            if (count % 2 != 0)
            {
                semaphoreNumber = count / 2;
            }
            else semaphoreNumber = count / 2 - 1;
            semaphore = new Semaphore(semaphoreNumber, semaphoreNumber);

            for (var i = 0; i < oldArray.Length; i++)
            {
                oldArray[i].state = OldState.Talk;
                var t = new Thread(new ParameterizedThreadStart(OldWork));
                t.Start(i);
                //Thread.Sleep(500);
            }
        }

        public static void OldWork(object num)
        {
            var number = int.Parse(num.ToString());
            while (true)
            {
                Console.WriteLine($"old#{num} talk ");
                oldArray[number].state = OldState.Talk;
                Thread.Sleep(1000);

                if (number == 0)
                {
                    Console.WriteLine($"Old #{number} wait forks");
                    oldArray[number].state = OldState.Wait;
                    semaphore.WaitOne();
                    Monitor.Enter(forkArray);
                    if (forkArray[number].isTaken || forkArray[forkArray.Length - 1].isTaken)
                    {
                        Monitor.Exit(forkArray);
                        
                        Thread.Sleep(1000);
                        Monitor.Enter(forkArray);
                        if (forkArray[number].isTaken || forkArray[forkArray.Length - 1].isTaken)
                        {
                            Monitor.Exit(forkArray);
                            semaphore.Release();
                            continue;
                        }


                    }

                    forkArray[number].isTaken = true;
                    forkArray[forkArray.Length - 1].isTaken = true;
                    Monitor.Exit(forkArray);
                    oldArray[number].state = OldState.Eat;
                    Console.WriteLine($"Old #{number} eating");
                    Thread.Sleep(1000);
                    Monitor.Enter(forkArray);
                    forkArray[number].isTaken = false;
                    forkArray[forkArray.Length - 1].isTaken = false;
                    Monitor.Exit(forkArray);
                    semaphore.Release();
                    Console.WriteLine($"Old #{number} finish eating");
                }
               
                else
                {
                    Console.WriteLine($"Old #{number} wait forks");
                    oldArray[number].state = OldState.Wait;
                    semaphore.WaitOne();
                    Monitor.Enter(forkArray);
                    if (forkArray[number].isTaken || forkArray[number - 1].isTaken)
                    {
                        Monitor.Exit(forkArray);
                        Thread.Sleep(1000);
                        Monitor.Enter(forkArray);
                        if (forkArray[number].isTaken || forkArray[number - 1].isTaken)
                        {
                            Monitor.Exit(forkArray);
                            semaphore.Release();
                            continue;
                        }
                        
                        
                    }
                    
                        forkArray[number].isTaken = true;
                        forkArray[number - 1].isTaken = true;
                        Monitor.Exit(forkArray);
                        oldArray[number].state = OldState.Eat;
                        Console.WriteLine($"Old #{number} eating");
                        Thread.Sleep(1000);
                        Monitor.Enter(forkArray);
                        forkArray[number].isTaken = false;
                        forkArray[number - 1].isTaken = false;
                        Monitor.Exit(forkArray);
                        semaphore.Release();
                        Console.WriteLine($"Old #{number} finish eating");
                    
                }


              
            }
        }
    }
}