var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireCvAnalyzerCodeCrunchOctoberMonth_ApiService>("apiservice");

builder.AddProject<Projects.AspireCvAnalyzerCodeCrunchOctoberMonth_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
