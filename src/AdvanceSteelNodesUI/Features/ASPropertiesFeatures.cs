﻿using System.Collections.Generic;
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
  [NodeName("BeamCutPlaneFeatureProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Beam Cut Plane Feature")]
  [NodeCategory("AdvanceSteel.Nodes.Features.BeamPlaneCut")]
  public class ASPropertiesBeamCutPlaneFeatures : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(BeamShortening);

    public ASPropertiesBeamCutPlaneFeatures() : base() { }

    [JsonConstructor]
    public ASPropertiesBeamCutPlaneFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("BeamRotatedNotchFeatureProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Beam Rotated Notch Feature")]
  [NodeCategory("AdvanceSteel.Nodes.Features.BeamCope")]
  public class ASPropertiesBeamNotchExFeatures : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(BeamNotchEx);

    public ASPropertiesBeamNotchExFeatures() : base() { }

    [JsonConstructor]
    public ASPropertiesBeamNotchExFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("BeamOrthoNotchFeatureProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Beam Ortho Notch Feature")]
  [NodeCategory("AdvanceSteel.Nodes.Features.BeamCope")]
  public class ASPropertiesBeamNotchSqFeatures : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(BeamNotch2Ortho);

    public ASPropertiesBeamNotchSqFeatures() : base() { }

    [JsonConstructor]
    public ASPropertiesBeamNotchSqFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("BeamMultiNotchFeatureProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Beam Multi Notch Feature")]
  [NodeCategory("AdvanceSteel.Nodes.Features.BeamPolycut")]
  public class ASPropertiesBeamPolylineNotchFeatures : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(BeamMultiContourNotch);

    public ASPropertiesBeamPolylineNotchFeatures() : base() { }

    [JsonConstructor]
    public ASPropertiesBeamPolylineNotchFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("PlateHoleProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Holes")]
  [NodeCategory("AdvanceSteel.Nodes.Features.PlateHoles")]
  public class ASPropertiesHoles : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(ConnectionHolePlate);

    public ASPropertiesHoles() : base() { }

    [JsonConstructor]
    public ASPropertiesHoles(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("PlateNotchContourFeatureProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Plate Notch Contour Feature")]
  [NodeCategory("AdvanceSteel.Nodes.Features.PlatePolycut")]
  public class ASPropertiesPlatePolylineNotchFeatures : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(PlateContourNotch);

    public ASPropertiesPlatePolylineNotchFeatures() : base() { }

    [JsonConstructor]
    public ASPropertiesPlatePolylineNotchFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("PlateVertexFeatureProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Plate Vertex Feature")]
  [NodeCategory("AdvanceSteel.Nodes.Features.PlateVertexCut")]
  public class ASPropertiesPlateVertexFeatures : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(PlateFeatVertex);

    public ASPropertiesPlateVertexFeatures() : base() { }

    [JsonConstructor]
    public ASPropertiesPlateVertexFeatures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }
}
