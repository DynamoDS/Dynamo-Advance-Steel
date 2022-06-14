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
  public class BeamRefAxisType : ASListBase
  {
    protected override string GetListName => "Beam Ref Axis";

    public BeamRefAxisType() : base() { }

    [JsonConstructor]
    public BeamRefAxisType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

    protected override List<DynamoDropDownItem> GetListDropDown()
    {
      var list = new List<DynamoDropDownItem>()
      {
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
      return list;
    }
  }
}
