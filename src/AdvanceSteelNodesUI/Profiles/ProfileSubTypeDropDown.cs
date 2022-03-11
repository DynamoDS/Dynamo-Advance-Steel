using AdvanceSteel.Nodes.Util;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;

namespace AdvanceSteel.Nodes
{
  [NodeName("GetProfileSubType")]
  [NodeCategory("AdvanceSteel.Nodes.Util.Profiles")]
  [NodeDescription("Get profile section type")]
  [IsDesignScriptCompatible]
  public class ProfileSubTypeDropDown : ProfileDropDown
  {
    private const string outputName = "subTypeName";

    public ProfileSubTypeDropDown() : base(outputName)
    {

    }

    [JsonConstructor]
    public ProfileSubTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {

    }

    protected override List<(string, string)> GetListItems()
    {
      return DataBaseUtils.GetSubTypes();
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (!CanBuildOutputAst())
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildStringNode(Items[SelectedIndex].Item.ToString())) };
    }
  }
}
