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
  public class ASObjecTypes : AstDropDownBase
  {
    private const string outputName = "objectType";
 
    public ASObjecTypes()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public ASObjecTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
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

      var stringNode = AstFactory.BuildStringNode(((Type)Items[SelectedIndex].Item).Name);
     
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), stringNode);
      return new List<AssociativeNode> { assign };

    }
  }
}
