Steps to get it work:

1.  install Dynamo Studio 2016
2.  install AutoCAD 2016
3.  install Advance Steel Ember

5.  verify that the paths from your local computer correspond to the ones from $Github\AcadAdvancedSteel\setupenv.bat, if not modify the bat file
7.  run $Github\AcadAdvancedSteel\setupenv.bat (this will open visual studio 2013 and set the variables to it) and open $Github\AcadAdvancedSteel\DynamoAdvanceSteel.sln
8.  build the solution in debug
9.  Modify ASSettings...xml to load the addin (DynamoAdvanceSteel.dll)
10. Start Advance Steel
14. run command RUNDYNAMO and the window should appear. Sometimes i need to wait more than 1 minute because Dynamo is looking for updates

You can use $Github\AcadAdvancedSteel\samples\sample1.dyn to test it
