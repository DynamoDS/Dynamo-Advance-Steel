# Dynamo Extension for Advance Steel

This repository contains the source files for the Dynamo Extension for Advance Steel, that connects Dynamo with Advance Steel.

### To build the sources:

1. Prerequisites needed:
  - Visual Studio 2017;
  - AutoCAD 2020;
  - Advance Steel 2020;
  - Dynamo Core Runtime 2.1.0 from https://dynamobuilds.com/;
  - Make sure you have [.Net Framework 4.7 SDK](https://www.microsoft.com/en-us/download/details.aspx?id=55168) installed on your computer

2. Update the paths from \src\Config\user_local.props with the ones from your machine

3. Build the All.sln solution in Release configuration.

4. Copy all files & folders from "Dynamo Core Runtime 2.1.0" to bin\AnyCPU\Release\steel-pkg\bin\Core

5. Register DynamoAdvanceSteel.dll as an addon for Advance Steel (see "Register addon" section of Advance Steel [online help](http://help.autodesk.com/view/ADSTPR/2020/ENU/?guid=GUID-A4DA627E-6680-4388-9C04-79F5F3D9D075))


### To build the installer:

1. Prerequisites needed:
  - Wix v3.11 or later;

2. Build All.sln solution;

3. Build DynamoAdvanceSteelInstall.sln solution;
