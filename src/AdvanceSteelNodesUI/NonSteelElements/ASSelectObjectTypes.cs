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
  [NodeDescription("Get All Advance Steel objects by Type")]
  [NodeCategory("AdvanceSteel.Nodes.Selection.ObjectSelection")]
  [OutPortDescriptions("SteelObject")]
  [OutPortNames("SteelObject")]
  [OutPortTypes("AdvanceSteel.Nodes.SteelDbObject")]
  [IsDesignScriptCompatible]
  public class ASSelectObjecTypes : AstDropDownBase
  {
    private const string outputName = "Get All Advance Steel Objects by Type";

    public ASSelectObjecTypes()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public ASSelectObjecTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>();
      Dictionary<Autodesk.AdvanceSteel.CADAccess.FilerObject.eObjectType, string> filterItems = Utils.GetASObjectFilters();
      foreach (var item in filterItems)
      {
        newItems.Add(new DynamoDropDownItem(item.Value, (int)item.Key));
      }

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select As Object Type..." ||
          SelectedIndex < 0)
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }
      AssociativeNode node;
      IntNode intNode2 = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
      node = AstFactory.BuildFunctionCall(new Func<int, IEnumerable<SteelDbObject>>(Utils.GetDynObjects), new List<AssociativeNode>() { intNode2 });

      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node);
      return new List<AssociativeNode> { assign };

    }
  }
}
