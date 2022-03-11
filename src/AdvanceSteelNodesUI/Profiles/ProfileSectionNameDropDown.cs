using System;
using System.Collections.Generic;
using AdvanceSteel.Nodes.Util;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;

namespace AdvanceSteel.Nodes
{
  [NodeName("GetProfileSectionName")]
  [NodeCategory("AdvanceSteel.Nodes.Util.Profiles")]
  [NodeDescription("Get profile section name")]
  [IsDesignScriptCompatible]
  public class ProfileSectionNameDropDown : ProfileFilterDropDown<ProfileSectionTypeDropDown>
  {
    private const string outputName = "sectionName";

    public ProfileSectionNameDropDown() : base(outputName)
    {
      InPorts.Add(new PortModel(PortType.Input, this, new PortData("sectionType", "Profile Section Type")));
    }

    [JsonConstructor]
    public ProfileSectionNameDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {

    }

    protected override List<(string, string)> GetListItems()
    {
      try
      {
        this.ClearErrorsAndWarnings();
        return DataBaseUtils.GetProfileSectionsByTypeNameText(Filter);
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

      return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildStringNode(Beam.CreateSectionString(Filter, Items[SelectedIndex].Item.ToString()))) };
    }

  }
}
