#addin "nuget:?package=SharpZipLib&Version=1.3.3"
#addin "nuget:?package=Cake.Compression&Version=0.3.0"
#addin "nuget:?package=Cake.FileHelpers&Version=5.0.0"

var workflow = BuildSystem.GitHubActions.Environment.Workflow;
var buildId = workflow.RunNumber;
var tag = workflow.RefType == GitHubActionsRefType.Tag ? workflow.RefName : null;

var buildVersion = FindRegexMatchGroupInFile("./src/Directory.Build.props", @"\<VersionPrefix\>(.*?)\<\/VersionPrefix\>", 1, System.Text.RegularExpressions.RegexOptions.None).Value;
var buildDir = MakeAbsolute(Directory("./build"));

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var msbuildSettings = new DotNetMSBuildSettings();

if (tag != null) 
{
    if (tag[1..] != buildVersion) throw new Exception("Tag version has to be the same as VersionPrefix in Directory.Build.props");
    msbuildSettings.Version = buildVersion;
}
else if (buildId != 0) 
{
    msbuildSettings.VersionSuffix = "ci." + buildId;
    buildVersion += "_ci." + buildId;
} 
else 
{
    buildVersion += "_dev";
}

//////////////////////////////////////////////////////////////////////
// UTILS
//////////////////////////////////////////////////////////////////////

// Remove unnecessary files for packaging.
private void ImpostorPublish(string runtime) {
    var name = "Impostor_Server";
    var projBuildDir = buildDir.Combine(name + "_" + runtime);
    var projBuildName = name + "_" + buildVersion + "_" + runtime;

    DotNetPublish("./src/Impostor.Server/Impostor.Server.csproj", new DotNetPublishSettings {
        Configuration = configuration,
        NoRestore = true,
        Runtime = runtime,
        SelfContained = false,
        PublishSingleFile = true,
        PublishTrimmed = false,
        OutputDirectory = projBuildDir,
        MSBuildSettings = msbuildSettings
    });


    var pluginsDir = projBuildDir.Combine("plugins");
    var librariesDir = projBuildDir.Combine("libraries");

    CreateDirectory(pluginsDir);
    CreateDirectory(librariesDir);

    FileWriteText(pluginsDir.CombineWithFilePath("1"), "1");
    FileWriteText(librariesDir.CombineWithFilePath("1"), "1");

    if (runtime == "win-x64") {
        FileWriteText(projBuildDir.CombineWithFilePath("run.bat"), "@echo off\r\nImpostor.Server.exe\r\npause");
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
        // Server.
        ImpostorPublish("win-x64");
        ImpostorPublish("osx-x64");
        ImpostorPublish("linux-x64");
        ImpostorPublish("linux-arm64");

        // API.
        DotNetPack("./src/Impostor.Api/Impostor.Api.csproj", new DotNetPackSettings {
            Configuration = configuration,
            OutputDirectory = buildDir + "/api",
            IncludeSource = true,
            IncludeSymbols = true,
            MSBuildSettings = msbuildSettings
        });

        // API Exension.
        DotNetPack("./src/Impostor.Api.Extension/Impostor.Api.Extension.csproj", new DotNetPackSettings
        {
            Configuration = configuration,
            OutputDirectory = buildDir + "/extension",
            IncludeSource = true,
            IncludeSymbols = true,
            MSBuildSettings = msbuildSettings
        });


        if (BuildSystem.GitHubActions.IsRunningOnGitHubActions) {
            foreach (var file in GetFiles(buildDir + "/api" + "/*.{nupkg,snupkg}"))
            {
                BuildSystem.GitHubActions.Commands.UploadArtifact(file, "Impostor.Api");
            }

            foreach (var file in GetFiles(buildDir + "/extension" + "/*.{nupkg,snupkg}"))
            {
                BuildSystem.GitHubActions.Commands.UploadArtifact(file, "Impostor.Api");
            }
        }
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
