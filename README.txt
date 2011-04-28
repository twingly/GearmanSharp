GearmanSharp is a C# API for Gearman (http://www.gearman.org).


Description
===========
GearmanSharp is a C# API for Gearman. Currently it only provides the basic
parts of the protocol, but there is enough to build a basic client and worker.
In the future, we hope it will provide a more complete implementation of the
protocol. It requires .NET 3.5.


License
=======
Copyright 2010 Twingly AB. GearmanSharp is provided under the three-clause
BSD License. See the included LICENSE.txt file for specifics.


Source code
===========
The source code is located on GitHub at:
http://github.com/twingly/GearmanSharp/


Examples
========
For more examples, check:
http://github.com/twingly/GearmanSharp/blob/master/GearmanSharp/Examples/Example.cs


* Client examples

    // Create a simple client and add "localhost" as server
    var client = new GearmanClient();
    client.AddServer("localhost");

    // You can submit a simple background job
    client.SubmitBackgroundJob("reverse",
        Encoding.ASCII.GetBytes("helloworld"));

    // And you can submit a more advanced job
    var oembeds = client.SubmitJob<IList<string>, IList<OEmbed>>(
        "GetOEmbeds",
        new List<string> { "http://www.youtube.com/watch?v=abc123456" },
        Serializers.JsonSerialize<IList<string>>,
        Serializers.JsonDeserialize<IList<OEmbed>>);



* Worker examples

    // Create a simple worker and register a function that handles "reverse"
    var worker = new GearmanWorker();
    worker.AddServer("localhost");
    worker.RegisterFunction("reverse", ReverseFunction);

    // Perform one unit of work
    worker.Work();
    
    // You can also register more advanced functions
    worker.RegisterFunction<IList<string>, IList<OEmbed>>("GetOEmbeds",
        GetOembedsFunction,
        Serializers.JsonDeserialize<IList<string>>,
        Serializers.JsonSerialize<IList<OEmbed>>);
    
    // The function definition and GearmanJob:
    public void GetOEmbeds(IGearmanJob<IList<string>, IList<OEmbed>> job)
    {
        // The FunctionArgument of the job will be an IList<string>
        IList<string> urls = job.FunctionArgument;
        // ...
        
        // and the Complete(..) function takes an IList<OEmbed>
        job.Complete(new List<OEmbed>());
    }
        
    // You can also start a worker in a separate thread
    var worker = new GearmanThreadedWorker();
    
    // Start the worker thread
    worker.StartWorkLoop();
    // ... do other stuff ...
    worker.StopWorkLoop();
