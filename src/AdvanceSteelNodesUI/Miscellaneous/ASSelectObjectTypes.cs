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
  public class ASSelectObjecTypes : AstDropDownBase
  {
    private const string outputName = "steelObjects";

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
      newItems.Add(new DynamoDropDownItem(SelectObjectTypeString, null));

      Dictionary<Type, string> filterItems = Utils.GetASObjectFilters();
      foreach (var item in filterItems)
      {
        newItems.Add(new DynamoDropDownItem(item.Value, item.Key));
      }

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          SelectedIndex < 0 ||
          Items[SelectedIndex].Name.Equals(SelectObjectTypeString))
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      AssociativeNode typeNode = AstFactory.BuildPrimitiveNodeFromObject((Type)Items[SelectedIndex].Item);
      AssociativeNode node = AstFactory.BuildFunctionCall(new Func<Type, IEnumerable<SteelDbObject>>(Utils.GetDynObjects), new List<AssociativeNode>() { typeNode });

      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node);
      return new List<AssociativeNode> { assign };

    }
  }
}
