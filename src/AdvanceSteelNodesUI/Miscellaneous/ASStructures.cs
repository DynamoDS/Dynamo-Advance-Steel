using AdvanceSteel.Nodes.Util;
using Autodesk.AutoCAD.DatabaseServices;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace AdvanceSteel.Nodes
{
  [NodeName("GetStructure")]
  [NodeCategory("AdvanceSteel.Nodes.Util.ProjectExplorer")]
  [NodeDescription("Get model structure")]
  [IsDesignScriptCompatible]
  [CLSCompliant(false)]
  public class ASStructures : AcadDropDownBase
  {
    private const string outputName = "structure handle";

    public ASStructures() : base(outputName)
    {
      OutPorts.Add(new PortModel(PortType.Output, this, new PortData("name", "Structure Name")));

      RegisterAllPorts();
    }

    [JsonConstructor]
    public ASStructures(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {

    }

    protected override List<(CADObjectId, string)> GetListItems()
    {
      return StructureUtils.GetListStructures();
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (!CanBuildOutputAst())
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()), AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildNullNode()) };
      }

      return new[]  {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildStringNode(((CADObjectId)Items[SelectedIndex].Item).Handle.ToString())),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildStringNode(Items[SelectedIndex].Name))
                    };
    }
  }
}