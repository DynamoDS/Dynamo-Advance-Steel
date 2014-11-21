Steps to get it work:

1.  install the Dynamo 0.7.3 
2.  install AutoCAD 2015
3.  install Advance Steel 2015.1
4.  replace ASMgd20x64.dll from "C:\Program Files\Autodesk\Advance Steel 2015.1\Kernel\Bin\" with the one from email(this step is required because we have done some changes that are included just in advance steel 2016)
5.  verify that the paths from your local computer correspond to the ones from $Github\AcadAdvancedSteel\setupenv.bat, if not modify the bat file
7.  run $Github\AcadAdvancedSteel\setupenv.bat (this will open visual studio 2013 and set the variables to it) and open $Github\AcadAdvancedSteel\DynamoAdvanceSteel.sln
8.  build the solution in debug
9.  go to dynamo installation folder and create a new folder named AdvanceSteel(Now you should have new directory: C:\Program Files\Dynamo 0.7\AdvanceSteel\)
10. copy resulted binaries from $Github\AcadAdvancedSteel\bin\Debug\AdvanceSteel in the new folder (C:\Program Files\Dynamo 0.7\AdvanceSteel\)
11. verify if the path of file from $Github\AcadAdvancedSteel\registerAddin.reg correspond to your local file, if not modify the reg
12. run $Github\AcadAdvancedSteel\registerAddin.reg
13. start advance steel 2015.1
14. run command RUNDYNAMO and the window should appear. Sometimes i need to wait more than 1 minute because Dynamo is looking for updates

You can use $Github\AcadAdvancedSteel\samples\sample1.dyn to test it
