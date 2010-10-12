using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Twingly.Gearman.Examples
{ 
    // App.config:
    // -----------
    // <?xml version="1.0" encoding="utf-8" ?>
    // <configuration>
    //   <configSections>
    //     <section name="gearman" type="Twingly.Gearman.Configuration.GearmanConfigurationSection, Twingly.Gearman" />
    //   </configSections>
    //   <gearman>
    //     <clusters>
    //       <cluster name="gearmanCluster">
    //         <servers>
    //           <server host="gearman.example.com" port="4730" />
    //           <server host="10.0.0.3" port="4730" />
    //         </servers>
    //       </cluster>
    //     </clusters>
    //   </gearman>
    // </configuration>

    class Example
    {
        // This example uses the JSON.NET library for JSON serializaion/deserialization
        [JsonObject]
        public class OEmbed
        {
            public string Title { get; set; }

            [JsonProperty(PropertyName = "author_name")]
            public string AuthorName { get; set; }

            // ...
        }

        public void SimpleClient()
        {
            var client = new GearmanClient("gearmanCluster");

            var handle = client.SubmitBackgroundJob("reverse", Encoding.ASCII.GetBytes("helloworld"));
        }

        public void AdvancedClient()
        {
            var client = new GearmanClient();
            client.AddServer("gearman.example.com");
            client.AddServer("10.0.0.2", 4730);

            var urls = new List<string> { "http://www.youtube.com/watch?v=abc123456", "http://www.youtube.com/watch?v=xyz9876" };
            
            var oembeds = client.SubmitJob<IList<string>, IList<OEmbed>>("GetOEmbeds", urls,
                Serializers.JsonSerialize<IList<string>>, Serializers.JsonDeserialize<IList<OEmbed>>);
        }

        public void SimpleWorker()
        {
            var worker = new GearmanWorker("gearmanCluster");
            worker.RegisterFunction("reverse", ReverseFunction);
            
            while (/* we should continue working is */ true)
            {
                worker.Work();
            }
        }
        
        private static void ReverseFunction(IGearmanJob<byte[], byte[]> job)
        {
            var str = Encoding.ASCII.GetString(job.FunctionArgument);
            var strArray = str.ToCharArray();
            Array.Reverse(strArray);
            var reversedStr = new string(strArray);
            job.Complete(Encoding.ASCII.GetBytes(reversedStr));
        }

        public void AdvancedWorker()
        {
            var worker = new GearmanThreadedWorker();
            worker.AddServer("gearman.example.com");
            worker.AddServer("10.0.0.2", 4730);
            worker.SetClientId("my-client");

            worker.RegisterFunction<IList<string>, IList<OEmbed>>("GetOEmbeds", GetOembedsFunction,
                Serializers.JsonDeserialize<IList<string>>, Serializers.JsonSerialize<IList<OEmbed>>);
            
            // start worker thread
            worker.StartWorkLoop();

            // do other stuff

            // when it's time to stop
            worker.StopWorkLoop();
        }

        public void GetOembedsFunction(IGearmanJob<IList<string>, IList<OEmbed>> job)
        {
            var urls = job.FunctionArgument;
            var oembeds = new List<OEmbed>();

            foreach (var url in urls)
            {
                // ... fetch oEmbeds ...
            }

            job.Complete(oembeds);
        }
    }
}
