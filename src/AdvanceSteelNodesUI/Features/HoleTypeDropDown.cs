using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;


namespace AdvanceSteel.Nodes
{
  [NodeName("HoleTypes")]
  [NodeDescription("Lists the Advance Steel Hole type options")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  [OutPortNames("holeType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("hole type")]
  [IsDesignScriptCompatible]
  public class HoleType : ASListBase
  {
    protected override string GetListName => "Hole Type";

    public HoleType() : base() { }

    [JsonConstructor]
    public HoleType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem("Hole", 1L),
        new DynamoDropDownItem("Slotted Hole", 2L),
        new DynamoDropDownItem("Counter Sunk Hole", 3L),
        new DynamoDropDownItem("Blind Hole", 4L),
        new DynamoDropDownItem("Threaded Hole", 5L),
        new DynamoDropDownItem("Sunk Hole", 6L),
        new DynamoDropDownItem("Punch Mark", 7L)
      };
      return list;
    }
  }
}
