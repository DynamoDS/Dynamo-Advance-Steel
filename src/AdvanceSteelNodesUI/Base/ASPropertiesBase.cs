using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using System;

namespace AdvanceSteel.Nodes
{
  public abstract class ASPropertiesBase : AstDropDownBase
  {
    private const string outputName = "propertyName";

    protected abstract Type GetObjectType { get; }

    private string _selectionText = null;

    private string SelectionText
    {
      get 
      {
        if(string.IsNullOrEmpty(_selectionText))
        {
          _selectionText = string.Format("Select {0} Property...", Utils.GetDescriptionObject(GetObjectType));
        }

        return _selectionText; 
      }
    }


    public ASPropertiesBase()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();

      OutPorts.Add(new PortModel(PortType.Output, this, new PortData("propertyName", "name of the selected property")));

      RegisterAllPorts();
    }

    [JsonConstructor]
    public ASPropertiesBase(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>() { };

      var properties = Utils.GetAllProperties(GetObjectType);

      newItems.Add(new DynamoDropDownItem(SelectionText, null));

      foreach (var item in properties)
      {
        newItems.Add(new DynamoDropDownItem(item.Key, item.Value));
      }
      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          SelectedIndex < 0 ||
          Items[SelectedIndex].Name.Equals(SelectionText))
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildPrimitiveNodeFromObject((string)Items[SelectedIndex].Name);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

      return new List<AssociativeNode> { assign };

    }
  }
}