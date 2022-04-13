using AdvanceSteel.Nodes.Util;
using Autodesk.AdvanceSteel.BuildingStructure;
using Autodesk.AdvanceSteel.CADAccess;
using CoreNodeModels;
using Dynamo.Applications.AdvanceSteel.Services;
using Dynamo.Engine;
using Dynamo.Graph;
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

using CADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace AdvanceSteel.Nodes
{
  [NodeName("GetSteelObjectsByGroup")]
  [NodeCategory("AdvanceSteel.Nodes.Util.ProjectExplorer")]
  [NodeDescription("Select all steel objects by group")]
  [IsDesignScriptCompatible]
  [CLSCompliant(false)]
  public class ASGroups : AcadDropDownBase
  {
    private const string outputName = "name";

    private string StructureHandleFilter { get; set; }

    private const int ASStructuresPort = 0;
    private ASStructures ASStructuresFilter { get; set; }

    //internal EngineController EngineController { get; set; }

    public ASGroups() : base(outputName)
    {
      Initialize();

      InPorts.Add(new PortModel(PortType.Input, this, new PortData("structure handle", "Structure of Project Explorer")));

      OutPorts.Add(new PortModel(PortType.Output, this, new PortData("asObject", "List of Advance Steel Objects")));

      RegisterAllPorts();
    }

    [JsonConstructor]
    public ASGroups(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {
      Initialize();
    }

    private void Initialize()
    {
      this.PortConnected += StructuresDropDownFilter_PortConnected;
      this.PortDisconnected += StructuresDropDownFilter_PortDisconnected;
    }

    private void StructuresDropDownFilter_PortConnected(PortModel arg1, Dynamo.Graph.Connectors.ConnectorModel arg2)
    {
      if (arg1.PortType == PortType.Output || arg1.Index != ASStructuresPort || !this.InputNodes.TryGetValue(ASStructuresPort, out var value))
      {
        return;
      }

      if (value.Item2 is ASStructures)
      {
        //There is nodes (Dropdown) at InPort
        ASStructuresFilter = value.Item2 as ASStructures;
        ASStructuresFilter.Modified += StructuresDropDownFilter_Modified;

        ApplyFilter();
      }
    }

    private void StructuresDropDownFilter_PortDisconnected(PortModel obj)
    {
      if (obj.PortType == PortType.Output || obj.Index != ASStructuresPort || ASStructuresFilter == null)
      {
        return;
      }

      ASStructuresFilter.Modified -= StructuresDropDownFilter_Modified;
      ASStructuresFilter = null;

      if (DisposeLogic.IsClosingHomeworkspace)
      {
        return;
      }

      StructureHandleFilter = null;

      PopulateItems();
      OnNodeModified();
    }

    private void StructuresDropDownFilter_Modified(NodeModel obj)
    {
      ApplyFilter();
    }

    private void ApplyFilter()
    {
      var oldfilter = StructureHandleFilter;

      if (ASStructuresFilter.SelectedIndex == -1)
      {
        StructureHandleFilter = null;
        PopulateItems();

        OnNodeModified();

        return;
      }

      var itemSelected = ASStructuresFilter.Items[ASStructuresFilter.SelectedIndex].Item;
      StructureHandleFilter = itemSelected == null ? null : ((CADObjectId)itemSelected).Handle.ToString();

      if (!string.IsNullOrEmpty(oldfilter) && oldfilter.Equals(StructureHandleFilter))
        return;

      PopulateItems();
      OnNodeModified();
    }

    protected override List<(CADObjectId, string)> GetListItems()
    {
      if (string.IsNullOrEmpty(this.StructureHandleFilter))
      {
        return new List<(CADObjectId, string)>();
      }

      return GroupUtils.GetListGroups(this.StructureHandleFilter);
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (!CanBuildOutputAst())
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()),
                               AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildNullNode())};
      }

      var argsNodes = new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(Items[SelectedIndex].Name),
                AstFactory.BuildStringNode(((CADObjectId)Items[SelectedIndex].Item).Handle.ToString())
            };

      var functionCallGetObjects = AstFactory.BuildFunctionCall<System.String, System.String, IEnumerable<SteelDbObject>>
      (GroupUtils.GetASObjectsByGroupHandle, argsNodes);

      return new[]
      {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildStringNode(Items[SelectedIndex].Name)),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), functionCallGetObjects),
            };
    }

    public override void Dispose()
    {
      this.PortConnected -= StructuresDropDownFilter_PortConnected;
      this.PortDisconnected -= StructuresDropDownFilter_PortDisconnected;

      base.Dispose();
    }
  }
}
