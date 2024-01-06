#addin "nuget:?package=SharpZipLib&Version=1.3.3"
#addin "nuget:?package=Cake.Compression&Version=0.3.0"
#addin "nuget:?package=Cake.FileHelpers&Version=5.0.0"

var workflow = BuildSystem.GitHubActions.Environment.Workflow;
var buildId = workflow.RunNumber;
var tag = workflow.RefType == GitHubActionsRefType.Tag ? workflow.RefName : null;

var buildVersion = FindRegexMatchGroupInFile("./src/Directory.Build.props", @"\<VersionPrefix\>(.*?)\<\/VersionPrefix\>", 1, System.Text.RegularExpressions.RegexOptions.None).Value;
var buildDir = MakeAbsolute(Directory("./build"));

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");

var msbuildSettings = new DotNetMSBuildSettings();

if (tag != null) 
{
    if (tag[1..] != buildVersion) throw new Exception("Tag version has to be the same as VersionPrefix in Directory.Build.props");
    msbuildSettings.Version = buildVersion;
}
else if (buildId != 0) 
{
    buildId += 500; 
    msbuildSettings.VersionSuffix = "ci." + buildId;
    buildVersion += "-ci." + buildId;
} 
else 
{
    buildVersion += "-dev";
}

//////////////////////////////////////////////////////////////////////
// UTILS
//////////////////////////////////////////////////////////////////////

// Remove unnecessary files for packaging.
private void ImpostorPublish(string name, string project, string runtime, bool isServer = false) {
    var projBuildDir = buildDir.Combine(name + "_" + runtime);
    var projBuildName = name + "_" + buildVersion + "_" + runtime;

    DotNetPublish(project, new DotNetPublishSettings {
        Configuration = configuration,
        NoRestore = true,
        Runtime = runtime,
        SelfContained = false,
        PublishSingleFile = true,
        PublishTrimmed = false,
        OutputDirectory = projBuildDir,
        MSBuildSettings = msbuildSettings
    });

    if (isServer) {
        CreateDirectory(projBuildDir.Combine("Plugins"));
        CreateDirectory(projBuildDir.Combine("Lib"));
        CreateDirectory(projBuildDir.Combine("Core"));

        if (runtime == "win-x64") {
            FileWriteText(projBuildDir.CombineWithFilePath("run.bat"), "@echo off\r\nImpostor.Server.exe\r\npause");
        }
    }

    if (runtime == "win-x64") {
        Zip(projBuildDir, buildDir.CombineWithFilePath(projBuildName + ".zip"));
    } else {
        GZipCompress(projBuildDir, buildDir.CombineWithFilePath(projBuildName + ".tar.gz"));
    }
    
    if (BuildSystem.GitHubActions.IsRunningOnGitHubActions) {
        BuildSystem.GitHubActions.Commands.UploadArtifact(projBuildDir, projBuildName);
    }
}

private void ImpostorPublishNF(string name, string project) {
    var runtime = "win-x64";
    var projBuildDir = buildDir.Combine(name + "_" + runtime);
    var projBuildZip = buildDir.CombineWithFilePath(name + "_" + buildVersion + "_" + runtime + ".zip");

    DotNetPublish(project, new DotNetPublishSettings {
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
        DotNetRestore("./src/Impostor.sln");
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        // Tests.
        DotNetBuild("./src/Impostor.Tests/Impostor.Tests.csproj", new DotNetBuildSettings {
            Configuration = configuration,
        });
            
        // Server.
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "win-x64", true);
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "osx-x64", true);
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-x64", true);
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm", true);
        ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm64", true);

        // API.
        DotNetPack("./src/Impostor.Api/Impostor.Api.csproj", new DotNetPackSettings {
            Configuration = configuration,
            OutputDirectory = buildDir,
            IncludeSource = true,
            IncludeSymbols = true,
            MSBuildSettings = msbuildSettings
        });

        if (BuildSystem.GitHubActions.IsRunningOnGitHubActions) {
            foreach (var file in GetFiles(buildDir + "/*.{nupkg,snupkg}"))
            {
                BuildSystem.GitHubActions.Commands.UploadArtifact(file, "Impostor.Api");
            }
        }
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
