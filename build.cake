var buildVersion = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "1.0.0";
var buildDir = MakeAbsolute(Directory("./build"));

var target = Argument("target", "Deploy");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// UTILS
//////////////////////////////////////////////////////////////////////

public void ImpostorPublish(string name, string project, string runtime) {
    var projBuildDir = buildDir.Combine(name + "-" + runtime);
    var projBuildZip = buildDir.CombineWithFilePath(name + "-" + buildVersion + "-" + runtime + ".zip");

    DotNetCorePublish(project, new DotNetCorePublishSettings {
        Configuration = configuration,
        NoRestore = true,
        Framework = "net5.0",
        Runtime = runtime,
        SelfContained = true,
        PublishSingleFile = true,
        PublishTrimmed = true,
        OutputDirectory = projBuildDir
    });

    Zip(projBuildDir, projBuildZip);
}

public void ImpostorPublishNF(string name, string project) {
    var runtime = "win-x64";
    var projBuildDir = buildDir.Combine(name + "-" + runtime);
    var projBuildZip = buildDir.CombineWithFilePath(name + "-" + buildVersion + "-" + runtime + ".zip");

    DotNetCorePublish(project, new DotNetCorePublishSettings {
        Configuration = configuration,
        NoRestore = true,
        Framework = "net472",
        OutputDirectory = projBuildDir
    });

    Zip(projBuildDir, projBuildZip);
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        if (DirectoryExists(buildDir)) {
            DeleteDirectory(buildDir, new DeleteDirectorySettings {
                Recursive = true
            });
        }
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore("./src/Impostor.sln");
        DotNetCoreRestore("./src/Impostor.Client/Impostor.Client.WinForms/Impostor.Client.WinForms.csproj");
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        // Tests.
        DotNetCoreBuild("./src/Impostor.Tests/Impostor.Tests.csproj", new DotNetCoreBuildSettings {
            Configuration = configuration,
        });

        // Client.
        ImpostorPublishNF("Impostor-Client", "./src/Impostor.Client/Impostor.Client.WinForms/Impostor.Client.WinForms.csproj");
        
        // Server.
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "win-x64");
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "osx-x64");
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-x64");
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm");
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm64");
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest("./src/Impostor.Tests/Impostor.Tests.csproj", new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true
        });
    });

Task("Deploy")
    .IsDependentOn("Test")
    .Does(() => {
        Information("Finished.");
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);