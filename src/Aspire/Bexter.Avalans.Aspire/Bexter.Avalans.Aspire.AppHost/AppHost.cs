var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Bexter_Avalans_Items_Api>("bexter-avalans-items-api");

builder.AddProject<Projects.Bexter_Avalans_Locations_Api>("bexter-avalans-locations-api");

builder.Build().Run();
