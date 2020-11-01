#addin "nuget:?package=SharpZipLib&Version=1.3.0"
#addin "nuget:?package=Cake.Compression&Version=0.2.4"
#addin "nuget:?package=Cake.FileHelpers&Version=3.3.0"


var buildId = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0";
var buildVersion = EnvironmentVariable("IMPOSTOR_VERSION") ?? "1.0.0";
var buildBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH") ?? "dev";
var buildDir = MakeAbsolute(Directory("./build"));

var prNumber = EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER");
var target = Argument("target", "Deploy");
var configuration = Argument("configuration", "Release");

// On any branch that is not master, we need to tag the version as prerelease.
if (buildBranch != "master") {
    buildVersion = buildVersion + "-ci." + buildId;
}

//////////////////////////////////////////////////////////////////////
// UTILS
//////////////////////////////////////////////////////////////////////

// Remove unnecessary files for packaging.
private void ImpostorClean(string directory) {
    foreach (var file in System.IO.Directory.GetFiles(directory, "*.pdb", SearchOption.AllDirectories)) {
        System.IO.File.Delete(file);
    }
}

private void ImpostorPublish(string name, string project, string runtime, bool isServer = false) {
    var projBuildDir = buildDir.Combine(name + "_" + runtime);
    var projBuildName = name + "_" + buildVersion + "_" + runtime;

    DotNetCorePublish(project, new DotNetCorePublishSettings {
        Configuration = configuration,
        NoRestore = true,
        Framework = "net5.0",
        Runtime = runtime,
        SelfContained = false,
        PublishSingleFile = true,
        PublishTrimmed = false,
        OutputDirectory = projBuildDir
    });

    ImpostorClean(projBuildDir.ToString());

    if (isServer) {
        CreateDirectory(projBuildDir.Combine("plugins"));
        CreateDirectory(projBuildDir.Combine("libraries"));

        if (runtime == "win-x64") {
            FileWriteText(projBuildDir.CombineWithFilePath("run.bat"), "@echo off\r\nImpostor.Server.exe\r\npause");
        }
    }

    if (runtime == "win-x64") {
        Zip(projBuildDir, buildDir.CombineWithFilePath(projBuildName + ".zip"));
    } else {
        GZipCompress(projBuildDir, buildDir.CombineWithFilePath(projBuildName + ".tar.gz"));
    }
}

private void ImpostorPublishNF(string name, string project) {
    var runtime = "win-x64";
    var projBuildDir = buildDir.Combine(name + "_" + runtime);
    var projBuildZip = buildDir.CombineWithFilePath(name + "_" + buildVersion + "_" + runtime + ".zip");

    DotNetCorePublish(project, new DotNetCorePublishSettings {
        Configuration = configuration,
        NoRestore = true,
        Framework = "net472",
        OutputDirectory = projBuildDir
    });

    ImpostorClean(projBuildDir.ToString());

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
    });

Task("Patch")
    .WithCriteria(BuildSystem.AppVeyor.IsRunningOnAppVeyor)
    .Does(() => {
        ReplaceRegexInFiles("./src/**/*.csproj", @"<Version>.*?<\/Version>", "<Version>" + buildVersion + "</Version>");
        ReplaceRegexInFiles("./src/**/*.props", @"<Version>.*?<\/Version>", "<Version>" + buildVersion + "</Version>");
    });

Task("Replay")
    .Does(() => {
        // D:\Projects\GitHub\Impostor\Impostor\src\Impostor.Tools.ServerReplay\sessions
        DotNetCoreRun(
            "./src/Impostor.Tools.ServerReplay/Impostor.Tools.ServerReplay.csproj", 
            "./src/Impostor.Tools.ServerReplay/sessions", new DotNetCoreRunSettings {
                Configuration = configuration,
                NoRestore = true,
                Framework = "net5.0"
            });
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Patch")
    .IsDependentOn("Restore")
    .IsDependentOn("Replay")
    .Does(() => {
        // Tests.
        DotNetCoreBuild("./src/Impostor.Tests/Impostor.Tests.csproj", new DotNetCoreBuildSettings {
            Configuration = configuration,
        });

        // Only build artifacts if;
        // - buildBranch is master/dev
        // - it is not a pull request
        if ((buildBranch == "master" || buildBranch == "dev") && string.IsNullOrEmpty(prNumber)) {
            // Client.
            ImpostorPublishNF("Impostor-Patcher", "./src/Impostor.Patcher/Impostor.Patcher.WinForms/Impostor.Patcher.WinForms.csproj");

            ImpostorPublish("Impostor-Patcher-Cli", "./src/Impostor.Patcher/Impostor.Patcher.Cli/Impostor.Patcher.Cli.csproj", "win-x64");
            ImpostorPublish("Impostor-Patcher-Cli", "./src/Impostor.Patcher/Impostor.Patcher.Cli/Impostor.Patcher.Cli.csproj", "osx-x64");
            ImpostorPublish("Impostor-Patcher-Cli", "./src/Impostor.Patcher/Impostor.Patcher.Cli/Impostor.Patcher.Cli.csproj", "linux-x64");
            
            // Server.
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "win-x64", true);
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "osx-x64", true);
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-x64", true);
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm", true);
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm64", true);

            // API.
            DotNetCorePack("./src/Impostor.Api/Impostor.Api.csproj", new DotNetCorePackSettings {
                Configuration = configuration,
                OutputDirectory = buildDir,
                IncludeSource = true,
                IncludeSymbols = true
            });
        } else {
            DotNetCoreBuild("./src/Impostor.Patcher/Impostor.Patcher.WinForms/Impostor.Patcher.WinForms.csproj", new DotNetCoreBuildSettings {
                Configuration = configuration,
                NoRestore = true,
                Framework = "net472"
            });

            DotNetCoreBuild("./src/Impostor.Server/Impostor.Server.csproj", new DotNetCoreBuildSettings {
                Configuration = configuration,
                NoRestore = true,
                Framework = "net5.0"
            });
        }
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