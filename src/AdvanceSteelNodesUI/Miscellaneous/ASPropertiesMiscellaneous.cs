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
using Autodesk.AdvanceSteel.ConstructionHelper;
using Autodesk.AdvanceSteel.ConstructionTypes;

namespace AdvanceSteel.Nodes
{
  [NodeName("CameraProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Camera")]
  [NodeCategory("AdvanceSteel.Nodes.Miscellaneous.Camera")]
  [IsDesignScriptCompatible]
  public class ASPropertiesCamera : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(Camera);

    public ASPropertiesCamera() : base() { }

    [JsonConstructor]
    public ASPropertiesCamera(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("SpecialPartsProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Special Part")]
  [NodeCategory("AdvanceSteel.Nodes.Miscellaneous.SpecialPart")]
  [IsDesignScriptCompatible]
  public class ASPropertiesSpecialParts : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(SpecialPart);

    public ASPropertiesSpecialParts() : base() { }

    [JsonConstructor]
    public ASPropertiesSpecialParts(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("JointProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Joint")]
  [NodeCategory("AdvanceSteel.Nodes.Miscellaneous.Joint")]
  [IsDesignScriptCompatible]
  public class ASPropertiesJoint : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(UserAutoConstructionObject);

    public ASPropertiesJoint() : base() { }

    [JsonConstructor]
    public ASPropertiesJoint(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }
}
