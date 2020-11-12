using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using System;

namespace AdvanceSteel.Nodes
{
	[NodeName("Advance Steel Object Types")]
	[NodeDescription("Select Advance Steel object to select filter")]
  [NodeCategory("AdvanceSteel.Nodes.Properties.Properties-Type")]
  [OutPortNames("Advance Steel Object Types")]
  [OutPortTypes("int")]
  [OutPortDescriptions("integer")]
  [IsDesignScriptCompatible]
	public class ASObjecTypes : AstDropDownBase
	{
		private const string outputName = "Advance Steel Object Types";

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

      var intNode = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
	}
}
