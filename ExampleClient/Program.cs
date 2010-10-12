using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Twingly.Gearman;

namespace ExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new GearmanClient();
            var host = "10.21.1.201";
            client.AddServer(host, 4730);
            client.AddServer(host, 4731);

            CreateBackgroundJobs(client, 100);
            CreateJobs(client, 100);
        }

        private static void CreateJobs(GearmanClient client, int jobCount)
        {
            for (int i = 0; i < jobCount; i++)
            {
                var result = client.SubmitJob<string, string>("reverse", String.Format("{0}: Hello World", i),
                    Serializers.UTF8StringSerialize, Serializers.UTF8StringDeserialize);
                Console.WriteLine("Job result: {0}", result);
            }
        }

        private static void CreateBackgroundJobs(GearmanClient client, int jobCount)
        {
            for (int i = 0; i < jobCount; i++)
            {
                var handle = client.SubmitBackgroundJob("reverse", Encoding.UTF8.GetBytes(String.Format("{0}: Hello World", i)));
                Console.WriteLine("Submitted background job. Handle: {0}", handle);
            }
        }
    }

}
