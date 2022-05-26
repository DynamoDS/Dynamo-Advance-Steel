using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("PlateFilletVertexTypes")]
  [NodeDescription("Lists the Advance Steel Plate Fillet Vertex type options")]
  [NodeCategory("AdvanceSteel.Nodes.Features.PlateVertexCut")]
  [OutPortNames("vertexType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("type of vertex")]
  [IsDesignScriptCompatible]
  public class PlateFilletVertexType : ASListBase
  {
    protected override string GetListName => "Plate Corner Cut Type";

    public PlateFilletVertexType() : base() { }

    [JsonConstructor]
    public PlateFilletVertexType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem("Convex", 0L),
        new DynamoDropDownItem("Concave", 1L),
        new DynamoDropDownItem("Striaght", 2L)
      };
      return list;
    }
  }
}
