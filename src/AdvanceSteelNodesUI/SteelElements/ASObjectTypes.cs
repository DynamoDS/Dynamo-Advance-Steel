using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using System;

namespace AdvanceSteel.Nodes
{
  [NodeName("AdvanceSteelObjectTypes")]
  [NodeDescription("Lists all the Advance Steel object types")]
  [NodeCategory("AdvanceSteel.Nodes.Selection")]
  [OutPortNames("objectType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("advance steel object type")]
  [IsDesignScriptCompatible]
  public class ASObjecTypes : ASObjectTypeBase
  {
    public ASObjecTypes() : base() { }

    [JsonConstructor]
    public ASObjecTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }


    protected override AssociativeNode CreateAssociativeNode(DynamoDropDownItem dynamoDropDownItem)
    {
      var stringNode = AstFactory.BuildStringNode(((Type)dynamoDropDownItem.Item).Name);
      return stringNode;
    }
  }
}
