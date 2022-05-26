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
  public abstract class ASListBase : AstDropDownBase
  {
    private const string outputName = nameof(outputName);

    protected abstract string GetListName { get; }

    private string _selectionText = null;

    private string SelectionText
    {
      get
      {
        if (string.IsNullOrEmpty(_selectionText))
        {
          _selectionText = string.Format("Select {0}...", GetListName);
        }

        return _selectionText;
      }
    }

    public ASListBase()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public ASListBase(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
      {
        new DynamoDropDownItem(SelectionText, -1L)
      };

      var listDropDown = GetListDropDown();
      newItems.AddRange(listDropDown);

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    protected abstract List<DynamoDropDownItem> GetListDropDown();

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          SelectedIndex < 0 ||
          Items[SelectedIndex].Name == SelectionText)
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((long)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}
