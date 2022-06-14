using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("BodyResolutions")]
  [NodeDescription("Lists all options for body resolution")]
  [NodeCategory("AdvanceSteel.Nodes.Util.Geometry")]
  [OutPortNames("bodyResolution")]
  [OutPortTypes("int")]
  [OutPortDescriptions("body resolution")]
  [IsDesignScriptCompatible]
  public class BodyResolution : AstDropDownBase
  {
    private const string outputName = "bodyResolution";

    public BodyResolution()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public BodyResolution(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Body Resolution...", -1L),
                new DynamoDropDownItem("Normal", 0L),
                new DynamoDropDownItem("Detailed", 1L),
                new DynamoDropDownItem("Hull", 2L),
                new DynamoDropDownItem("UnNotched", 3L)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select Body Resolution..." ||
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
