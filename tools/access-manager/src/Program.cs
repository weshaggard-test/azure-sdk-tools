﻿using System.CommandLine;
using Azure.Identity;

await Entrypoint(args);

static async Task Entrypoint(string[] args)
{
    var rootCommand = new RootCommand("RBAC and Federated Identity manager for Azure SDK apps");
    var fileArgument = new Argument<FileInfo>("file", "Path to access config file for identities");

    rootCommand.Add(fileArgument);
    rootCommand.SetHandler(async (fileArgumentValue) => await Run(fileArgumentValue), fileArgument);

    await rootCommand.InvokeAsync(args);
}

static async Task Run(FileInfo config)
{
    Console.WriteLine("Using config -> " + config.FullName + Environment.NewLine);

    var accessConfig = AccessConfig.Create(config.FullName);
    Console.WriteLine(accessConfig.ToString());

    var credential = new DefaultAzureCredential();
    var reconciler = new Reconciler(new GraphClient(credential), new RbacClient(credential));
    await reconciler.Reconcile(accessConfig);
}
