using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using System;
using System.Linq;

using CADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using AdvanceSteel.Nodes.Util;

namespace AdvanceSteel.Nodes
{
  [NodeName("GetLayer")]
  [NodeCategory("AdvanceSteel.Nodes.Util.CAD")]
  [NodeDescription("Get model layers")]
  [IsDesignScriptCompatible]
  public class AcadLayer : AcadDropDownBase
  {
    private const string outputName = "layer";

    public AcadLayer() : base(outputName)
    {

    }

    [JsonConstructor]
    public AcadLayer(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {

    }

    protected override List<(CADObjectId, string)> GetListItems()
    {
      return LayerUtils.GetAllLayers().Select(x => (x.ObjectId, x.Name)).ToList();
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (!CanBuildOutputAst())
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildStringNode(Items[SelectedIndex].Name)) };
    }
  }
}
