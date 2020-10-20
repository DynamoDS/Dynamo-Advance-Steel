using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
	[NodeName("Plate Feature Properties")]
	[NodeDescription("Select Advance Steel Plate Feature Property Type to Set")]
  [NodeCategory("AdvanceSteel.Nodes.Properties.Properties-Write")]
  [OutPortNames("Writable Plate Feature Property")]
  [OutPortTypes("string")]
  [OutPortDescriptions("string")]
  [IsDesignScriptCompatible]
	public class ASPropertiesPlateFeatures : AstDropDownBase
	{
		private const string outputName = "Advance Steel Plate Feature Property";

    public ASPropertiesPlateFeatures()
				: base(outputName)
		{
			InPorts.Clear();
			OutPorts.Clear();
			RegisterAllPorts();
		}

		[JsonConstructor]
		public ASPropertiesPlateFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
		: base(outputName, inPorts, outPorts)
		{
		}

		protected override SelectionState PopulateItemsCore(string currentSelection)
		{
			Items.Clear();

      var newItems = new List<DynamoDropDownItem>() { };
      foreach (var item in Utils.GetPlateFeaturePropertyList(ePropertyDataOperator.Set))
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
          Items[SelectedIndex].Name == "None" ||
          SelectedIndex < 0)
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildPrimitiveNodeFromObject((string)Items[SelectedIndex].Name);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

      return new List<AssociativeNode> { assign };

    }
	}
}
