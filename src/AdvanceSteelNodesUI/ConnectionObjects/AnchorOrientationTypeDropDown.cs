using System.Collections.Generic;
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
  public class AnchorOrientationType : ASListBase
  {
    protected override string GetListName => "Anchor Orientation";

    public AnchorOrientationType() : base() { }

    [JsonConstructor]
    public AnchorOrientationType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem("Normal Orientation", 0L),
        new DynamoDropDownItem("Diagonal Inside", 1L),
        new DynamoDropDownItem("Diagonal Outside", 2L),
        new DynamoDropDownItem("All Outside", 3L),
        new DynamoDropDownItem("All Inside", 4L),
        new DynamoDropDownItem("Inside Rotated", 5L),
        new DynamoDropDownItem("Outside Rotated", 6L)
      };

      return list;
    }
  }
}