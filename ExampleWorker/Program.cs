using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Twingly.Gearman;

namespace ExampleWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var worker = new GearmanThreadedWorker();
                var host = "smeagol";
                worker.AddServer(host, 4731);
                worker.AddServer(host, 4730);
                worker.SetClientId("my-threaded-worker");
                worker.RegisterFunction<string, string>("reverse", DoReverse, Serializers.UTF8StringDeserialize, Serializers.UTF8StringSerialize);
                worker.RegisterFunction<string, string>("reverse_with_status", DoReverseWithStatus, Serializers.UTF8StringDeserialize, Serializers.UTF8StringSerialize);

                Console.WriteLine("Press enter to start work loop, and press enter again to stop");
                Console.ReadLine();

                worker.StartWorkLoop();
                
                Console.ReadLine();

                worker.StopWorkLoop();

                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got exception: {0}", ex);
                return;
            }

        }

        private static void DoReverse(IGearmanJob<string, string> job)
        {
            Console.WriteLine("Got job with handle: {0}, function: {1}", job.Info.JobHandle, job.Info.FunctionName);
            
            var str = job.FunctionArgument;

            var strArray = str.ToCharArray();
            Array.Reverse(strArray);
            var reversedStr = new string(strArray);

            Console.WriteLine("  Reversed: {0}", reversedStr);

            job.Complete(reversedStr);
        }

        private static void DoReverseWithStatus(IGearmanJob<string, string> job)
        {
            Console.WriteLine("Got job with handle: {0}, function: {1}", job.Info.JobHandle, job.Info.FunctionName);

            var str = job.FunctionArgument;
            job.SetStatus(0, (uint)str.Length);
            var reversedArray = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                reversedArray[str.Length - i - 1] = str[i];
                job.SetStatus((uint)i+1, (uint)str.Length);
            }

            var reversedStr = new string(reversedArray);
            Console.WriteLine("  Reversed: {0}", reversedStr);

            job.Complete(reversedStr);
        }
    }
}
