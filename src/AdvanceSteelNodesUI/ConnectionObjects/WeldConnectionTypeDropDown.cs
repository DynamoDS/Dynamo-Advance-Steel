using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;


namespace AdvanceSteel.Nodes
{
	[NodeName("Weld Connection Type")]
	[NodeDescription("Set Weld Connection type - InShop or OnSite")]
  [NodeCategory("AdvanceSteel.Nodes.Properties.Properties-Type")]
  [OutPortNames("Weld Type")]
  [OutPortTypes("int")]
  [OutPortDescriptions("integer")]
  [IsDesignScriptCompatible]
	public class WeldConnectionType : AstDropDownBase
	{
		private const string outputName = "Weld Type";

		public WeldConnectionType()
				: base(outputName)
		{
			InPorts.Clear();
			OutPorts.Clear();
			RegisterAllPorts();
		}

		[JsonConstructor]
		public WeldConnectionType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
		: base(outputName, inPorts, outPorts)
		{
		}

		protected override SelectionState PopulateItemsCore(string currentSelection)
		{
			Items.Clear();

			var newItems = new List<DynamoDropDownItem>()
						{
								new DynamoDropDownItem("Select Weld Type...", -1),
								new DynamoDropDownItem("OnSite", 0),
								new DynamoDropDownItem("InShop", 2)
						};

			Items.AddRange(newItems);

			SelectedIndex = 0;
			return SelectionState.Restore;
		}

		public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
		{
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select Weld Type..." ||
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
