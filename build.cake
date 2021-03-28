#addin "nuget:?package=SharpZipLib&Version=1.3.1"
#addin "nuget:?package=Cake.Compression&Version=0.2.6"
#addin "nuget:?package=Cake.FileHelpers&Version=4.0.1"

var buildId = EnvironmentVariable("GITHUB_RUN_NUMBER") ?? EnvironmentVariable("APPVEYOR_BUILD_VERSION");
var buildRelease = EnvironmentVariable("APPVEYOR_REPO_TAG") == "true";
var buildVersion = FindRegexMatchGroupInFile("./src/Directory.Build.props", @"\<VersionPrefix\>(.*?)\<\/VersionPrefix\>", 1, System.Text.RegularExpressions.RegexOptions.None).Value;
var buildDir = MakeAbsolute(Directory("./build"));

var target = Argument("target", "Deploy");
var configuration = Argument("configuration", "Release");

var msbuildSettings = new DotNetCoreMSBuildSettings();

if (buildRelease) 
{
    msbuildSettings.Properties["Version"] = new[] { buildVersion };
}
else if (buildId != null) 
{
    msbuildSettings.Properties["VersionSuffix"] = new[] { "ci." + buildId };
    buildVersion += "-ci." + buildId;
}

//////////////////////////////////////////////////////////////////////
// UTILS
//////////////////////////////////////////////////////////////////////

// Remove unnecessary files for packaging.
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
        OutputDirectory = projBuildDir,
        MSBuildSettings = msbuildSettings
    });

    if (isServer) {
        CreateDirectory(projBuildDir.Combine("plugins"));
        CreateDirectory(projBuildDir.Combine("libraries"));

        if (runtime == "win-x64") {
            FileWriteText(projBuildDir.CombineWithFilePath("run.bat"), "@echo off\r\nImpostor.Server.exe\r\npause");
        }
    }

    Zip(projBuildDir, buildDir.CombineWithFilePath(projBuildName + ".zip"));
}

private void ImpostorPublishNF(string name, string project) {
    var runtime = "win-x64";
    var projBuildDir = buildDir.Combine(name + "_" + runtime);
    var projBuildZip = buildDir.CombineWithFilePath(name + "_" + buildVersion + "_" + runtime + ".zip");

    DotNetCorePublish(project, new DotNetCorePublishSettings {
        Configuration = configuration,
        NoRestore = true,
        Framework = "net472",
        OutputDirectory = projBuildDir,
        MSBuildSettings = msbuildSettings
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
    .IsDependentOn("Restore")
    .IsDependentOn("Replay")
    .Does(() => {
        // Tests.
        DotNetCoreBuild("./src/Impostor.Tests/Impostor.Tests.csproj", new DotNetCoreBuildSettings {
            Configuration = configuration,
        });

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
            IncludeSymbols = true,
            MSBuildSettings = msbuildSettings
        });
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