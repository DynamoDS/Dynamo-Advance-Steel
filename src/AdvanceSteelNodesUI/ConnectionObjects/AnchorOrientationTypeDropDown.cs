﻿using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("AnchorOrientationTypes")]
  [NodeDescription("Lists the Advance Steel Anchor orientation type options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("orientationType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("anchor orientation type")]
  [IsDesignScriptCompatible]
  public class AnchorOrientationType : AstDropDownBase
  {
    private const string outputName = "orientationType";

    public AnchorOrientationType()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public AnchorOrientationType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Anchor Orientation...", -1),
                new DynamoDropDownItem("Normal Orientation", 0),
                new DynamoDropDownItem("Diagonal Inside", 1),
                new DynamoDropDownItem("Diagonal Outside", 2),
                new DynamoDropDownItem("All Outside", 3),
                new DynamoDropDownItem("All Inside", 4),
                new DynamoDropDownItem("Inside Rotated", 5),
                new DynamoDropDownItem("Outside Rotated", 6)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select Anchor Orientation..." ||
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
