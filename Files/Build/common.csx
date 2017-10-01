// This is the public facing file, any changes to this file is probably a breaking change.
// build.cake scripts should ONLY use functions in this file and not ones that are in other files.

public void cleanAllBuildDirs(string buildDir, string configuration)
{
    Information("Cleaning the directory where artifacts are produced"); 
    CleanDirectory(buildDir);
    CleanDirectories("./**/bin/" + configuration);
}
public void restorePackagesInSolution(string slnFilePath)
{
    Information("Restoring nuget packages in {0}", slnFilePath);
    NuGetRestore(slnFilePath, new NuGetRestoreSettings { 
        Source = getNugetSources()
    });
}

public List<string> getNugetSources()
{
    return new List<string>(){
            "https://api.nuget.org/v3/index.json"
    };
}
public void buildSolution(string slnFilePath, string configuration)
{
    Information("Building sln file {0}", slnFilePath);
    
    MSBuild(slnFilePath,
        settings =>
            settings.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .UseToolVersion(toolVersion)
                .SetMSBuildPlatform(MSBuildPlatform.x64)
                .SetPlatformTarget(PlatformTarget.MSIL));
}
public void runNUnit2Tests(string testFilePattern = "./**/bin/**/*.Tests.dll", string outputFilePath = "./.build/TestResults.xml")
{
    Information("Running NUnit(2) tests at pattern {0}", testFilePattern);
	
    NUnit(testFilePattern, new NUnitSettings {
        ResultsFile = outputFilePath
    });
}