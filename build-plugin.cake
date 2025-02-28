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
        DotNetBuild("./src/Plugins/SelfHttpMatchmaker/SelfHttpMatchmaker.csproj", new DotNetBuildSettings {
            OutputDirectory = buildDir,
            Configuration = configuration,
            MSBuildSettings = msbuildSettings
        });

        DotNetBuild("./src/Plugins/GameCodePlugin/GameCodePlugin.csproj", new DotNetBuildSettings {
            OutputDirectory = buildDir,
            Configuration = configuration,
            MSBuildSettings = msbuildSettings
        });

        if (BuildSystem.GitHubActions.IsRunningOnGitHubActions) {
            BuildSystem.GitHubActions.Commands.UploadArtifact(buildDir + "/SelfHttpMatchmaker.dll", "SelfHttpMatchmaker");

            BuildSystem.GitHubActions.Commands.UploadArtifact(buildDir + "/GameCodePlugin.dll", "GameCodePlugin");
        }
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
