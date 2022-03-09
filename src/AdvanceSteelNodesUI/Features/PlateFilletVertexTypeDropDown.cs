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
  public class PlateFilletVertexType : AstDropDownBase
  {
    private const string outputName = "vertexType";

    public PlateFilletVertexType()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public PlateFilletVertexType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Plate Corener Cut Type...", -1L),
                new DynamoDropDownItem("Convex", 0L),
                new DynamoDropDownItem("Concave", 1L),
                new DynamoDropDownItem("Striaght", 2L)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select Plate Corener Cut Type..." ||
          SelectedIndex < 0)
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((long)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}
