using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var host = "10.21.1.201";
                worker.AddServer(host, 4731);
                worker.AddServer(host, 4730);
                worker.SetClientId("my-threaded-worker");
                worker.RegisterFunction<string, string>("reverse", DoReverse, Serializers.UTF8StringDeserialize, Serializers.UTF8StringSerialize);

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
    }
}
