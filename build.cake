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

private void ImpostorPublish(string name, string project, string runtime) {
    var projBuildDir = buildDir.Combine(name + "_" + runtime);
    var projBuildZip = buildDir.CombineWithFilePath(name + "_" + buildVersion + "_" + runtime + ".zip");

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

    ImpostorClean(projBuildDir.ToString());

    Zip(projBuildDir, projBuildZip);
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

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Patch")
    .IsDependentOn("Restore")
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

            // Excluding these because currently it results in a ~17MB package for each.
            // ImpostorPublish("Impostor-Patcher-Cli", "./src/Impostor.Patcher/Impostor.Patcher.Cli/Impostor.Patcher.Cli.csproj", "win-x64");
            // ImpostorPublish("Impostor-Patcher-Cli", "./src/Impostor.Patcher/Impostor.Patcher.Cli/Impostor.Patcher.Cli.csproj", "osx-x64");
            // ImpostorPublish("Impostor-Patcher-Cli", "./src/Impostor.Patcher/Impostor.Patcher.Cli/Impostor.Patcher.Cli.csproj", "linux-x64");
            
            // Server.
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "win-x64");
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "osx-x64");
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-x64");
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm");
            ImpostorPublish("Impostor-Server", "./src/Impostor.Server/Impostor.Server.csproj", "linux-arm64");
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