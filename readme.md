# Dynamo Extension for Advance Steel

This repository contains the source files for the Dynamo Extension for Advance Steel, that connects Dynamo with Advance Steel.

### To build the sources:

1. Prerequisites needed:
  - Visual Studio 2019;
  - AutoCAD 2022;
  - Advance Steel 2022;
  - Dynamo Core 2.10.1 from http://www.github.com/DynamoDS/Dynamo;
  - Make sure you have [.Net Framework 4.8 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks) installed on your computer

2. Update the paths from \src\Config\user_local.props with the ones from your machine

3. Build the DynamoAdvanceSteel.sln solution in Release configuration.

4. Copy all files & folders from "Dynamo Core 2.10.1" build to .\bin\AnyCPU\Release\steel-pkg\bin\Core

5. Register DynamoAdvanceSteel.dll as an addon for Advance Steel (see "Register addon" section of Advance Steel [online help](http://help.autodesk.com/view/ADSTPR/2022/ENU/?guid=GUID-A4DA627E-6680-4388-9C04-79F5F3D9D075))


### To build the installer:

1. Prerequisites needed:
  - Wix v3.11 or later;

2. Build DynamoAdvanceSteel.sln solution;

3. Build DynamoAdvanceSteelInstall.sln solution;

### List of available nodes:

Click [here](nodes.md) to view the list of the available dynamo-advance steel nodes