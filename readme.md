# Dynamo Extension for Advance Steel

This repository contains the source files for the Dynamo Extension for Advance Steel, that connects Dynamo with Advance Steel.

### To build the sources:

1. Prerequisites needed:
  - Visual Studio 2013;
  - Autocad 2017;
  - Advance Steel 2017 (or 2017.1);
  - Dynamo Core 1.2.0 or later;

2. Update the paths from \src\Config\user_local.props with the ones from your machine

3. Build the All.sln solution.

4. Register DynamoAdvanceSteel.dll as an Addin for Advance Steel (see "Register addin" section of Advance Steel [online help](http://help.autodesk.com/view/ADSTPR/2017/ENU/?guid=GUID-A4DA627E-6680-4388-9C04-79F5F3D9D075))


### To build the installer:

1. Prerequisites needed:
  - Wix v3.7 or later;

2. Build All.sln solution;

3. Build DynamoAdvanceSteelInstall.sln solution;