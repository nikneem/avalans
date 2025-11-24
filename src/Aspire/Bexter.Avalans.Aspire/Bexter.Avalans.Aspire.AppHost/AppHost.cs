var builder = DistributedApplication.CreateBuilder(args);

// Add Azure Storage with Table Storage for Items
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var tables = storage.AddTables("tables");

builder.AddProject<Projects.Bexter_Avalans_Items_Api>("bexter-avalans-items-api")
    .WithReference(tables)
    .WaitFor(tables);

builder.AddProject<Projects.Bexter_Avalans_Locations_Api>("bexter-avalans-locations-api");

builder.AddProject<Projects.Bexter_Avalans_Transactions_Api>("bexter-avalans-transactions-api");

builder.Build().Run();
