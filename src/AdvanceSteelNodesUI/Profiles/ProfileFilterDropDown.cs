using Dynamo.Applications.AdvanceSteel.Services;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;

namespace AdvanceSteel.Nodes
{
  public abstract class ProfileFilterDropDown<T> : ProfileDropDown
       where T : ProfileDropDown
  {
    protected string Filter { get; set; }

    private const int ProfileDropDownPort = 0;

    private T ProfileDropDownFilter { get; set; }

    //internal EngineController EngineController { get; set; }

    public ProfileFilterDropDown(string outputName) : base(outputName)
    {
      Initialize();
    }

    [JsonConstructor]
    public ProfileFilterDropDown(string outputName, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {
      Initialize();
    }

    private void Initialize()
    {
      this.PortConnected += ProfileDropDownFilter_PortConnected;
      this.PortDisconnected += ProfileDropDownFilter_PortDisconnected;
    }

    private void ProfileDropDownFilter_PortConnected(PortModel arg1, Dynamo.Graph.Connectors.ConnectorModel arg2)
    {
      if (arg1.PortType == PortType.Output || arg1.Index != ProfileDropDownPort || !this.InputNodes.TryGetValue(ProfileDropDownPort, out var value))
      {
        return;
      }

      if (value.Item2 is T)
      {
        //There is nodes (Dropdown) at InPort
        ProfileDropDownFilter = value.Item2 as T;
        ProfileDropDownFilter.Modified += ProfileDropDownFilter_Modified;

        ApplyFilter();
      }
    }

    private void ProfileDropDownFilter_PortDisconnected(PortModel obj)
    {
      if (obj.PortType == PortType.Output || obj.Index != ProfileDropDownPort || ProfileDropDownFilter == null)
      {
        return;
      }

      ProfileDropDownFilter.Modified -= ProfileDropDownFilter_Modified;
      ProfileDropDownFilter = null;

      if (DisposeLogic.IsClosingHomeworkspace)
      {
        return;
      }

      Filter = null;

      PopulateItems();

      OnNodeModified();
    }

    private void ProfileDropDownFilter_Modified(NodeModel obj)
    {
      ApplyFilter();
    }

    private void ApplyFilter()
    {
      var oldfilter = Filter;

      if (ProfileDropDownFilter.SelectedIndex == -1)
      {
        Filter = null;
        PopulateItems();

        OnNodeModified();

        return;
      }

      var itemSelected = ProfileDropDownFilter.Items[ProfileDropDownFilter.SelectedIndex].Item;
      Filter = itemSelected == null ? null : itemSelected.ToString();

      if (!string.IsNullOrEmpty(oldfilter) && oldfilter.Equals(Filter))
        return;

      PopulateItems();
      OnNodeModified();
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (!CanBuildOutputAst())
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildStringNode(Items[SelectedIndex].Item.ToString())) };
    }

    public override void Dispose()
    {
      this.PortConnected -= ProfileDropDownFilter_PortConnected;
      this.PortDisconnected -= ProfileDropDownFilter_PortDisconnected;

      base.Dispose();
    }
  }
}