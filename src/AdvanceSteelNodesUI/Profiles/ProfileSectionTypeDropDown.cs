using AdvanceSteel.Nodes.Util;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteel.Nodes
{
  [NodeName("GetProfileSectionType")]
  [NodeCategory("AdvanceSteel.Nodes.Util.Profiles")]
  [NodeDescription("Get profile section type")]
  [IsDesignScriptCompatible]
  public class ProfileSectionTypeDropDown : ProfileFilterDropDown<ProfileSubTypeDropDown>
  {
    private const string outputName = "sectionType";

    public ProfileSectionTypeDropDown() : base(outputName)
    {
      InPorts.Add(new PortModel(PortType.Input, this, new PortData("subTypeName", "Sub Type Profile")));
    }

    [JsonConstructor]
    public ProfileSectionTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {

    }

    protected override List<(string, string)> GetListItems()
    {
      try
      {
        this.ClearErrorsAndWarnings();
        return DataBaseUtils.GetTablesBySubTypeName(Filter);
      }
      catch (Exception ex)
      {
        base.Warning(ex.Message);
        return null;
      }

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
