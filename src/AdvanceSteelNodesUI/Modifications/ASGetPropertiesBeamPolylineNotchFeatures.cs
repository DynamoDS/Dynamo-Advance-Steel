﻿using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
	[NodeName("Beam Multi Notch Feature Properties - Get")]
	[NodeDescription("Select Advance Steel Beam Multi Notch Feature Property Type to Get")]
  [NodeCategory("AdvanceSteel.Nodes.Properties.Properties-Read")]
  [OutPortNames("Get Beam Multi Notch Feature Property")]
  [OutPortTypes("string")]
  [OutPortDescriptions("string")]
  [IsDesignScriptCompatible]
	public class ASGetPropertiesBeamPolylineNotchFeatures : AstDropDownBase
	{
		private const string outputName = "Advance Steel Beam Multi Notch Feature Properties to Get";

    public ASGetPropertiesBeamPolylineNotchFeatures()
				: base(outputName)
		{
			InPorts.Clear();
			OutPorts.Clear();
			RegisterAllPorts();
		}

		[JsonConstructor]
		public ASGetPropertiesBeamPolylineNotchFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
		: base(outputName, inPorts, outPorts)
		{
		}

		protected override SelectionState PopulateItemsCore(string currentSelection)
		{
			Items.Clear();

      var newItems = new List<DynamoDropDownItem>() { };
      foreach (var item in Utils.GetBeamMultiNotchPropertyList(ePropertyDataOperator.Get))
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
