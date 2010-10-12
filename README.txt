GearmanSharp
============
GearmanSharp is a C# API for Gearman (http://www.gearman.org).

Not everything in the Gearman protocol are supported yet, many things are
missing, but there should be enough to build a basic client and/or worker.
For example, both background and foreground jobs are supported, but continous
updates from the worker to client(s) is not implemented.


Examples
--------
For more examples, check:
https://github.com/twingly/GearmanSharp/blob/master/GearmanSharp/Examples/Example.cs


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
