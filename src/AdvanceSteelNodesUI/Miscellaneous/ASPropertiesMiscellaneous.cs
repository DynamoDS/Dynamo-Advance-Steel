using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using Autodesk.AdvanceSteel.CADAccess;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;

namespace AdvanceSteel.Nodes
{
  [NodeName("CameraProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Camera")]
  [NodeCategory("AdvanceSteel.Nodes.Miscellaneous.Camera")]
  public class ASPropertiesCamera : ASPropertiesBase
  {
    protected override eObjectType GetObjectType => eObjectType.kCamera;

    public ASPropertiesCamera() : base() { }

    [JsonConstructor]
    public ASPropertiesCamera(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("SpecialPartsProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Special Part")]
  [NodeCategory("AdvanceSteel.Nodes.Miscellaneous.SpecialPart")]
  public class ASPropertiesSpecialParts : ASPropertiesBase
  {
    protected override eObjectType GetObjectType => eObjectType.kSpecialPart;

    public ASPropertiesSpecialParts() : base() { }

    [JsonConstructor]
    public ASPropertiesSpecialParts(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }
}
