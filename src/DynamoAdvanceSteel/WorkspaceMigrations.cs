using System;
using System.Collections.Generic;
using System.Xml;
using Dynamo.Migration;
using Dynamo.Models;
using Dynamo.Utilities;

namespace Dynamo.Migration.AdvanceSteel
{
  public class WorkspaceMigrations
  {
    [WorkspaceMigration("0.8.1.0", "1.0.0.0")]
    public static void Migrate_0_8_1_to_1_0_0_0(XmlDocument doc)
    {
      XmlNodeList elNodes = doc.GetElementsByTagName("Elements");

      if (elNodes.Count == 0)
        elNodes = doc.GetElementsByTagName("dynElements");

      var elementsRoot = elNodes[0];

      var corrections = new List<Tuple<XmlNode, XmlNode>>();

      foreach (XmlElement elNode in elementsRoot.ChildNodes)
      {
        if (elNode.Name == "Dynamo.Nodes.DSFunction" && elNode.Attributes["assembly"].Value == "AsNodes.dll")
        {
          XmlElement newNode = elNode.Clone() as XmlElement;

          string oldFunction = elNode.Attributes["function"].Value;
          string newFunction = oldFunction.Replace("AdvanceSteel.Nodes.StraightBeam", "AdvanceSteel.Nodes.Beams.StraightBeam");
          newFunction = newFunction.Replace("AdvanceSteel.Nodes.BentBeam", "AdvanceSteel.Nodes.Beams.BentBeam");

          newNode.SetAttribute("assembly", "AdvanceSteelNodes.dll");
          newNode.SetAttribute("function", newFunction);
          corrections.Add(new Tuple<XmlNode, XmlNode>(newNode, elNode));
        }

      }

      foreach (var correction in corrections)
      {
        elementsRoot.InsertBefore(correction.Item1, correction.Item2);
        elementsRoot.RemoveChild(correction.Item2);
      }
    }
  }
}
