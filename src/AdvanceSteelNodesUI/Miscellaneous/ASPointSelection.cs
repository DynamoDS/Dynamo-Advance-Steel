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
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// Used to extract steel beams
  /// </summary>
  [IsDesignScriptCompatible]
  [IsVisibleInDynamoLibrary(true)]
  [NodeCategory("AdvanceSteel.Nodes.Selection")]
  [NodeName("Select Point")]
  [NodeDescription("Select a point in Model Space")]
  [OutPortNames("point")]
  [OutPortTypes("Autodesk.DesignScript.Geometry.Point")]
  public class ASPointSelection : SteelSelection<string, Autodesk.DesignScript.Geometry.Point>
  {
    /// <summary userName="Input.SelectStructureData">
    /// Provides a way to manually select model elements.
    /// </summary>
    /// <remarks>
    /// (Selection button for model elements)
    /// </remarks>
    /// <returns name="Elements">The resulting list of elements.</returns>
    public ASPointSelection()
           : base(
               SelectionType.One,
               SelectionObjectType.PointOnFace,
               "Pick Point",
               "Picked Point")
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectionIdentifier"></param>
    /// <param name="inPorts"></param>
    /// <param name="outPorts"></param>
    [JsonConstructor]
    public ASPointSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                  SelectionType.One,
                  SelectionObjectType.PointOnFace,
                  "Pick Point",
                  "Picked Point",
                  selectionIdentifier,
                  inPorts,
                  outPorts)
    { }

    /// <summary>
    /// 
    /// </summary>
    public override IModelSelectionHelper<string> SelectionHelper
    {
      get { return new SteelPointSelectionHelper(); }
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
        //IEnumerable<AssociativeNode> strInputs = SelectionResults.Select(res => AstFactory.BuildStringNode(res) as AssociativeNode);
        //ExprListNode inputNode1 = AstFactory.BuildExprList(strInputs.ToList());
        //node = AstFactory.BuildFunctionCall(new Func<IEnumerable<string>, IEnumerable<SteelDbObject>>(Utils.GetDynObjects), new List<AssociativeNode>() { inputNode1 });

        var newInputs = new List<AssociativeNode>();

        foreach (var resP in SelectionResults)
        {

          //this is a selected point on a face
          var ptArgs = new List<AssociativeNode>
                    {
                        AstFactory.BuildDoubleNode(resP.X),
                        AstFactory.BuildDoubleNode(resP.Y),
                        AstFactory.BuildDoubleNode(resP.Z)
                    };

          var functionCallNode =
              AstFactory.BuildFunctionCall(
                  new Func<double, double, double, Autodesk.DesignScript.Geometry.Point>(Autodesk.DesignScript.Geometry.Point.ByCoordinates),
                  ptArgs);

          newInputs.Add(functionCallNode);
        }

        node = AstFactory.BuildExprList(newInputs);
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
    protected override IEnumerable<Autodesk.DesignScript.Geometry.Point> ExtractSelectionResults(string selection)
    {
      var retVal = selection.Split(';');
      double x = Convert.ToDouble(retVal[0]);
      double y = Convert.ToDouble(retVal[1]);
      double z = Convert.ToDouble(retVal[2]);
      return new Autodesk.DesignScript.Geometry.Point[] { Autodesk.DesignScript.Geometry.Point.ByCoordinates(x, y, z) };
    }
  }

  public class ASPointSelectionNodeViewCustomization :
        SelectionBaseNodeViewCustomization<string, Autodesk.DesignScript.Geometry.Point>,
        INodeViewCustomization<ASPointSelection>
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="nodeView"></param>
    public void CustomizeView(ASPointSelection model, NodeView nodeView)
    {
      base.CustomizeView(model, nodeView);
      model.DynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
    }
  }
  internal class SteelPointSelectionHelper : CoreNodeModels.IModelSelectionHelper<string>
  {
    public event Action<ILogMessage> MessageLogged;

    public IEnumerable<string> RequestSelectionOfType(string selectionMessage, SelectionType selectionType, SelectionObjectType objectType)
    {
      IAppInteraction appInteraction = AppResolver.Resolve<IAppInteraction>();
      Point3d pt = appInteraction.PickPoint();
      return new string[] { string.Format("{0};{1};{2}", pt.x, pt.y, pt.z) };
    }
  }
}
