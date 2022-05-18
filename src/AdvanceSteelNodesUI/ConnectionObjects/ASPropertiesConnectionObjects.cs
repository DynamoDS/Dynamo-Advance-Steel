using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using Autodesk.AdvanceSteel.CADAccess;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using System;
using Autodesk.AdvanceSteel.Modelling;

namespace AdvanceSteel.Nodes
{
  [NodeName("AnchorBoltProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Anchor Bolt")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  public class ASPropertiesAnchorBolt : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(AnchorPattern);

    public ASPropertiesAnchorBolt() : base() { }

    [JsonConstructor]
    public ASPropertiesAnchorBolt(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("CircularBoltProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Circular Bolt Pattern")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects.CircularBoltPattern")]
  public class CircleScrewBoltPattern : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(CircleScrewBoltPattern);

    public CircleScrewBoltPattern() : base() { }

    [JsonConstructor]
    public CircleScrewBoltPattern(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("RectangularBoltProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Rectangular Bolt")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects.RectangularBoltPattern")]
  public class ASPropertiesBolts : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(InfinitMidScrewBoltPattern);

    public ASPropertiesBolts() : base() { }

    [JsonConstructor]
    public ASPropertiesBolts(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("ShearStudProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Shear Stud")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  public class ASPropertiesShearStud : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(Connector);

    public ASPropertiesShearStud() : base() { }

    [JsonConstructor]
    public ASPropertiesShearStud(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("WeldLineProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Weld Lines")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  public class ASPropertiesWeldLine : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(WeldLine);

    public ASPropertiesWeldLine() : base() { }

    [JsonConstructor]
    public ASPropertiesWeldLine(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("WeldPointProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Weld Points")]
  [NodeCategory("AdvanceSteel.Nodes.ConnectionObjects")]
  public class ASPropertiesWeldPoint : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(WeldPoint);

    public ASPropertiesWeldPoint() : base() { }

    [JsonConstructor]
    public ASPropertiesWeldPoint(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

}
