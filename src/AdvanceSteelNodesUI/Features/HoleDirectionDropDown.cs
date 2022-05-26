using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;


namespace AdvanceSteel.Nodes
{
  [NodeName("HoleDirection")]
  [NodeDescription("Lists the Advance Steel Slotted Hole Direction options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("slotDirection")]
  [OutPortTypes("int")]
  [OutPortDescriptions("slotted hole direciton")]
  [IsDesignScriptCompatible]
  public class SlotDirection : ASListBase
  {
    protected override string GetListName => "Slotted Hole Direction";

    public SlotDirection() : base() { }

    [JsonConstructor]
    public SlotDirection(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem("Arc Along", 2L),
        new DynamoDropDownItem("X-Axis", 1L),
        new DynamoDropDownItem("Y-Axis", 2L)
      };
      return list;
    }
  }
}
