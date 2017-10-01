#tool "nuget:?package=NUnit.Runners&version=2.6.4"
#tool "nuget:?package=GitVersion.CommandLine"
// All of the above are required for the common code in common.csx


// import the common functions
#load "Build/common.csx"

var buildNumber = Argument<int>("buildNumber");
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildDir = "./.build";

// location of sln file to build
var slnLocation = "./REPLACE_WITH_REPO_NAME.sln";

Task("Clean").IsDependentOn("Update_Environment").Does(() =>
{
    cleanAllBuildDirs(buildDir, configuration);
});

Task("Restore_NuGet_Packages").IsDependentOn("Clean").Does(() =>
{
    restorePackagesInSolution(slnLocation);
});

Task("Build").IsDependentOn("Restore_NuGet_Packages").Does(() => 
{
    buildSolution(slnLocation, configuration);
});

Task("Unit_Test").IsDependentOn("Build").Does(() =>
{   
    Information("Run unit tests");
    runNUnit2Tests();
});

Task("Default").IsDependentOn("Unit_Test");


// Other build targets
Task("Run_Integration_Tests").IsDependentOn("Build").Does(() =>
{
    runNUnit2Tests("./**/bin/**/*.IntegrationTests.dll", "./.build/TestResults.IntegrationTests.xml");
});

RunTarget(target);
