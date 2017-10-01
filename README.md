# BuildLib
a library to add versioned common cake code to a project via a nuget package

This is a simplified project which could be turned into a nuget package and added to solutions to build them using Cake code.

The common.csx file should be imported into the projects build.cake file and hold many stateless functions. These functions will have logic on things the company decides like application versioning logic or artifact publishing code.
The build.cake files in projects should have as little code in them and should always try to store the logic in the common.csx file.
