# Dynamo Extension for Advance Steel

This repository contains the source files for the Dynamo Extension for Advance Steel.

**Dynamo Extension for Advance Steel** has different branches for different versions of Advance Steel. For example, to run it on Advance Steel 2020 you must use **AS2020_2.1.0** branch.

## How to build and use this extension:

1. Prerequisites needed:
   - Advance Steel 2024;
   - Visual Studio 2022;
   - Dynamo Core 2.17.0 runtime from http://www.github.com/DynamoDS/Dynamo;
   - [.Net Framework 4.8 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
1. Update the paths from [`\src\Config\user_local.props`](/src/Config/user_local.props) with the ones from your machine
1. Build the DynamoAdvanceSteel.sln solution in Release configuration.
1. Copy all files and folders from "Dynamo Core 2.17.0" build to .\bin\AnyCPU\Release\steel-pkg\bin\Core
1. Register DynamoAdvanceSteel.dll as an addon for Advance Steel (see "Register addon" section from Advance Steel [online help](http://help.autodesk.com/view/ADSTPR/2023/ENU/?guid=GUID-A4DA627E-6680-4388-9C04-79F5F3D9D075))
1. (Optional) If Dynamo fails to load under Advance Steel, run the the PowerShell command `Get-ChildItem *.* -Recurse | Unblock-File` inside .\bin\AnyCPU\Release\steel-pkg\bin\Core

## To build the installer:

1. Prerequisites needed:
   - Wix v3.11 or newer;
1. Build DynamoAdvanceSteel.sln solution in Release configuration;
1. Build DynamoAdvanceSteelInstall.sln solution in Release configuration;

## List of available nodes:

Click [here](nodes.md) to view the list of the available dynamo-advance steel nodes
