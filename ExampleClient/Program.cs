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
            var host = "10.20.10.200";
            client.AddServer(host, 4730);
            client.AddServer(host, 4731);

            //CreateBackgroundJobs(client, 1000);
            //CreateJobs(client, 1000);
            CreateJsonJobs(client);
            //CreateBackgroundJsonJobs(client);
        }

        private static void CreateBackgroundJsonJobs(GearmanClient client)
        {
            var urls = JArray.FromObject(new List<string> {"http://www.youtube.com/watch?v=4Hcv-CTnFDQ&feature=spotlight"});

            client.SubmitBackgroundJob<JArray>("GetOEmbeds", urls,
                Serializers.JsonSerialize<JArray>);
        }

        private static void CreateJsonJobs(GearmanClient client)
        {
            var urls = JArray.FromObject(new List<string> {"http://www.youtube.com/watch?v=4Hcv-CTnFDQ&feature=spotlight"});

            var oembedJObject = client.SubmitJob<JArray, JArray>("GetOEmbeds", urls,
                Serializers.JsonSerialize<JArray>, Serializers.JsonDeserialize<JArray>);
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
                var handle = client.SubmitBackgroundJob<string>("reverse", String.Format("{0}: Hello World", i),
                    Serializers.UTF8StringSerialize);
                Console.WriteLine("Submitted background job. Handle: {0}", handle);
            }
        }
    }
}
