using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ContentBuilder
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("Expect:");
        Console.WriteLine("1. directory path to generated XML Documentation files");
        Console.WriteLine("2. full path to .md file to update");
        return;
      }

      string strFullAssemblyDocDir = args[0];              // full path to assembly documentation file ( xml)
      string strFullDocName = args[1];                     // full path to .md that will be created

      
      var nodes = NodeParser.GetNodesFromAssemblies(new string[] { "AdvanceSteelNodes.dll", "AdvanceSteelNodesUI.dll" });
      MDGenerator.GenerateFromAssembly(strFullAssemblyDocDir, nodes, strFullDocName);



    }
  }
}
