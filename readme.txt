
This repository contains the source files of DynamoAdvanceSteel addin, that connects Dynamo with Advance Steel.


How to build the sources:

1. To build the sources you need to have on your machine installed:
- Visual Studio 2013
- Autocad 2017
- Advance Steel 2017 (or 2017 R2)
- Dynamo Core 1.1.0

2. Update the paths from \src\Config\user_local.props with the ones from your machine

3. Open in visual studio \src\All.sln, build the solution and register DynamoAdvanceSteel.dll as an addin of Advance Steel(see Register Addin section of Advance Steel docs: http://help.autodesk.com/view/ADSTPR/2017/ENU/?guid=GUID-A4DA627E-6680-4388-9C04-79F5F3D9D075)


How to build the install:

1. To build the msi you need to have on your machine Wix v3.7
2. Build \src\All.sln
3. Build \src\DynamoAdvanceSteelInstall.sln