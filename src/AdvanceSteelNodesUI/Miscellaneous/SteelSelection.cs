using Autodesk.DesignScript.Runtime;
using CoreNodeModels;
using Dynamo.Applications.AdvanceSteel.Services;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public abstract class SteelSelection<TSelection, TResult> : SelectionBase<TSelection, TResult>
  {
    private DynamoModel dynamoModel;

    #region public properties

    public DynamoModel DynamoModel
    {
      get
      {
        return dynamoModel;
      }
      set
      {
        if (dynamoModel != null)
        {
          var hwm = dynamoModel.CurrentWorkspace as HomeWorkspaceModel;
          if (hwm != null)
          {
            hwm.RunSettings.PropertyChanged -= model_PropertyChanged;
          }
        }

        dynamoModel = value;

        if (dynamoModel != null)
        {
          var hwm = dynamoModel.CurrentWorkspace as HomeWorkspaceModel;
          if (hwm != null)
          {
            hwm.RunSettings.PropertyChanged += model_PropertyChanged;
          }
        }
      }
    }

    public override bool CanSelect
    {
      get
      {
        if (dynamoModel != null)
        {
          // Different document, disable selection button.
          //if (!dynamoModel.IsInMatchingDocumentContext)
          //  return false;

          var hwm = dynamoModel.CurrentWorkspace as HomeWorkspaceModel;
          if (hwm != null)
          {
            return base.CanSelect && hwm.RunSettings.RunEnabled;
          }
          else
          {
            return false;
          }
        }
        else
        {
          return base.CanSelect;
        }
      }
      set { base.CanSelect = value; }
    }


    /// <summary>
    /// Handler for the DynamoModel's PropertyChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      // Use the RunEnabled flag on the dynamo model
      // to set the CanSelect flag, enabling or disabling
      // any bound UI when the dynamo model is not
      // in a runnable state.
      if (e.PropertyName == "RunEnabled")
      {
        RaisePropertyChanged("CanSelect");
      }
    }


    #endregion

    protected SteelSelection(SelectionType selectionType,
        SelectionObjectType selectionObjectType, string message, string prefix)
        : base(selectionType, selectionObjectType, message, prefix)
    {
      AppResolver.Resolve<IAppInteraction>().DocumentOpened += Controller_DocumentChanged;
    }

    [JsonConstructor]
    public SteelSelection(SelectionType selectionType,
        SelectionObjectType selectionObjectType, string message, string prefix,
        IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
        : base(selectionType, selectionObjectType, message, prefix, selectionIdentifier, inPorts, outPorts)
    {
      AppResolver.Resolve<IAppInteraction>().DocumentOpened += Controller_DocumentChanged;
    }

    private void Controller_DocumentChanged(object sender, EventArgs e)
    {
      ClearSelections();
    }

    #region public methods

    public override void Dispose()
    {
      base.Dispose();

      AppResolver.Resolve<IAppInteraction>().DocumentOpened -= Controller_DocumentChanged;

      if (dynamoModel != null)
      {
        var hwm = dynamoModel.CurrentWorkspace as HomeWorkspaceModel;
        if (hwm != null)
        {
          hwm.RunSettings.PropertyChanged -= model_PropertyChanged;
        }
      }
    }

    public override void UpdateSelection(IEnumerable<TSelection> rawSelection)
    {
      base.UpdateSelection(rawSelection);
    }

    #endregion

  }
}
