using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("BeamReferenceAxis")]
  [NodeDescription("Lists the Advance Steel Beam Reference Axis options - e.g. Top Left, Middle Middle, Countour Center")]
  [NodeCategory("AdvanceSteel.Nodes.Beams")]
  [OutPortNames("referenceAxis")]
  [OutPortTypes("int")]
  [OutPortDescriptions("beam reference axis")]
  [IsDesignScriptCompatible]
  public class BeamRefAxisType : AstDropDownBase
  {
    private const string outputName = "referenceAxis";

    public BeamRefAxisType()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public BeamRefAxisType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Beam Ref Axis...", -1L),
                new DynamoDropDownItem("Upper Left", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kUpperLeft),
                new DynamoDropDownItem("Upper Middle", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kUpperSys),
                new DynamoDropDownItem("Upper Right", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kUpperRight),
                new DynamoDropDownItem("Middle Left", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kMidLeft),
                new DynamoDropDownItem("Middle Middle", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kSysSys),
                new DynamoDropDownItem("Middle Right", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kMidRight),
                new DynamoDropDownItem("Lower Left", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kLowerLeft),
                new DynamoDropDownItem("Lower Middle", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kLowerSys),
                new DynamoDropDownItem("Lower Right", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kLowerRight),
                new DynamoDropDownItem("Contour Center", (long)Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis.kContourCenter)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select Beam Ref Axis..." ||
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
