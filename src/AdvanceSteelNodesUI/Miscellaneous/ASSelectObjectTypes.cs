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
      AssociativeNode typeNode = AstFactory.BuildPrimitiveNodeFromObject((Type)Items[SelectedIndex].Item);
      AssociativeNode node = AstFactory.BuildFunctionCall(new Func<Type, IEnumerable<SteelDbObject>>(Utils.GetDynObjects), new List<AssociativeNode>() { typeNode });

      return node;
    }
  }
}
