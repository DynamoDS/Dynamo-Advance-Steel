using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using System;

namespace AdvanceSteel.Nodes
{
  [NodeName("GetSteelObjectsByType")]
  [NodeDescription("Select all steel objects by type")]
  [NodeCategory("AdvanceSteel.Nodes.Selection")]
  [OutPortDescriptions("list with the selected steel objects")]
  [OutPortNames("steelObjects")]
  [OutPortTypes("AdvanceSteel.Nodes.SteelDbObject")]
  [IsDesignScriptCompatible]
  public class ASSelectObjecTypes : ASObjectTypeBase
  {
    public ASSelectObjecTypes() : base() { }

    [JsonConstructor]
    public ASSelectObjecTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }


    protected override AssociativeNode CreateAssociativeNode(DynamoDropDownItem dynamoDropDownItem)
    {
      StringNode stringNode = AstFactory.BuildStringNode(Items[SelectedIndex].Name);
      AssociativeNode node = AstFactory.BuildFunctionCall(new Func<string, IEnumerable<SteelDbObject>>(Utils.GetDynObjectsByType), new List<AssociativeNode>() { stringNode });

      return node;
    }
  }
}
