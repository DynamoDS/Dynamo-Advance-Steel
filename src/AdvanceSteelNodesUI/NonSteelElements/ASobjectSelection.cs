using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Logging;
using ProtoCore.AST.AssociativeAST;
using CoreNodeModelsWpf.Nodes;
using Dynamo.Wpf;
using Dynamo.Controls;
using Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// Used to extract steel beams
  /// </summary>
  [IsDesignScriptCompatible]
  [IsVisibleInDynamoLibrary(true)]
  [NodeCategory("AdvanceSteel.Nodes.ObjectSelection")]
  [NodeName("SelectSteelObjects")]
  [NodeDescription("Select the Steel Objects")]
  [OutPortNames("SteelObject")]
  [OutPortTypes("AdvanceSteel.Nodes.SteelDbObject")]
  public class ASObjectSelection : SteelSelection<string, string>
  {
    /// <summary userName="Input.SelectStructureData">
    /// Provides a way to manually select model elements.
    /// </summary>
    /// <remarks>
    /// (Selection button for model elements)
    /// </remarks>
    /// <returns name="Elements">The resulting list of elements.</returns>
    public ASObjectSelection()
           : base(
               SelectionType.Many,
               SelectionObjectType.Element,
               "Structure Data",
               "Structure Data")
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectionIdentifier"></param>
    /// <param name="inPorts"></param>
    /// <param name="outPorts"></param>
    [JsonConstructor]
    public ASObjectSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                  SelectionType.Many,
                  SelectionObjectType.Element,
                  "Structure Data",
                  "Structure Data",
                  selectionIdentifier,
                  inPorts,
                  outPorts)
    { }

    /// <summary>
    /// 
    /// </summary>
    public override IModelSelectionHelper<string> SelectionHelper
    {
      get { return new SteelSelectionHelper(); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputAstNodes"></param>
    /// <returns></returns>
    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      AssociativeNode node;

      if (SelectionResults == null || !SelectionResults.Any())
      {
        node = AstFactory.BuildNullNode();
      }
      else
      {
        IEnumerable<AssociativeNode> strInputs = SelectionResults.Select(res => AstFactory.BuildStringNode(res) as AssociativeNode);
        ExprListNode inputNode1 = AstFactory.BuildExprList(strInputs.ToList());
        node = AstFactory.BuildFunctionCall(new Func<IEnumerable<string>, IEnumerable<Steel>>(Model.StructureDataByIds), new List<AssociativeNode>() { inputNode1 });
      }

      return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected override string GetModelObjectFromIdentifer(string id)
    {
      return id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelObject"></param>
    /// <returns></returns>
    protected override string GetIdentifierFromModelObject(string modelObject)
    {
      return modelObject;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selection"></param>
    /// <returns></returns>
    protected override IEnumerable<string> ExtractSelectionResults(string selection)
    {
      return new string[] { selection };
    }
  }

  public class ASObjectSelectionNodeViewCustomization :
        SelectionBaseNodeViewCustomization<string, string>,
        INodeViewCustomization<ASObjectSelection>
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="nodeView"></param>
    public void CustomizeView(ASObjectSelection model, NodeView nodeView)
    {
      base.CustomizeView(model, nodeView);
      model.DynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
    }
  }
  internal class SteelSelectionHelper : CoreNodeModels.IModelSelectionHelper<string>
  {
    public event Action<ILogMessage> MessageLogged;

    public IEnumerable<string> RequestSelectionOfType(string selectionMessage, SelectionType selectionType, SelectionObjectType objectType)
    {
      IAppInteraction appInteraction = AppResolver.Resolve<IAppInteraction>();
      var descs = appInteraction.PickElements();
      return descs;
    }
  }
}
