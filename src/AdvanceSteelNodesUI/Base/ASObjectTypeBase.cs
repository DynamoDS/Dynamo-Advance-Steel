using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using System;

namespace AdvanceSteel.Nodes
{
  public abstract class ASObjectTypeBase : AstDropDownBase
  {
    private const string outputName = nameof(outputName);

    public ASObjectTypeBase()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public ASObjectTypeBase(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>();
      newItems.Add(new DynamoDropDownItem(SelectObjectTypeString, null));

      Dictionary<string, Type> filterItems = Utils.GetASObjectFilters();
      foreach (var item in filterItems)
      {
        newItems.Add(new DynamoDropDownItem(item.Key, item.Value));
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

      var node = CreateAssociativeNode(Items[SelectedIndex]);

      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node);
      return new List<AssociativeNode> { assign };

    }

    protected abstract AssociativeNode CreateAssociativeNode(DynamoDropDownItem dynamoDropDownItem);
  }
}
