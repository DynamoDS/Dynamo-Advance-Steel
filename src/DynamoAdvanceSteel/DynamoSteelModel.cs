using AdvanceSteel.Nodes;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AutoCAD.ApplicationServices;
using DSCPython;
using DSIronPython;
using Dynamo.Applications.AdvanceSteel.Services;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Migration.AdvanceSteel;
using Dynamo.Models;
using Dynamo.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Dynamo.Applications.AdvanceSteel
{
  [CLSCompliant(false)]
  public class DynamoSteelModel : DynamoModel
  {
    private const string contextAS = "Advance Steel";

    public new static DynamoSteelModel Start(IStartConfiguration configuration)
    {
      if (string.IsNullOrEmpty(configuration.Context))
        configuration.Context = contextAS;

      return new DynamoSteelModel(configuration);
    }

    private DynamoSteelModel(IStartConfiguration configuration) :
        base(configuration)
    {
      Services.DisposeLogic.IsShuttingDown = false;

      this.HostName = "Dynamo AS";
      this.HostVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      this.UpdateManager.RegisterExternalApplicationProcessId(Process.GetCurrentProcess().Id);

      SubscribeDocumentManagerEvents();

      InitializeDocumentController();

      MigrationManager.MigrationTargets.Add(typeof(WorkspaceMigrations));

      //SetupPython();
    }

    #region Initialization

    private void InitializeDocumentController()
    {
      // Set the intitial document.
      var currentDocument = acadApp.DocumentManager.MdiActiveDocument;
      if (currentDocument != null)
      {
        DocumentController.Instance.HandleDocumentActivation(currentDocument);

        OnASDocumentChanged(DocumentController.Instance.CurrentDocumentName);
      }
    }

    #endregion

    #region Document Manager(CAD) - Event subscribe/unsubscribe - Event handler

    private bool hasRegisteredDocumentManagerEvents;
    private void SubscribeDocumentManagerEvents()
    {
      if (hasRegisteredDocumentManagerEvents)
      {
        return;
      }

      acadApp.DocumentManager.DocumentToBeActivated += DocumentManager_DocumentToBeActivated;
      acadApp.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
      acadApp.DocumentManager.DocumentToBeDeactivated += DocumentManager_DocumentToBeDeactivated;
      acadApp.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;
      acadApp.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;

      hasRegisteredDocumentManagerEvents = true;
    }

    private void UnsubscribeDocumentManagerEvents()
    {
      if (!hasRegisteredDocumentManagerEvents)
      {
        return;
      }

      acadApp.DocumentManager.DocumentToBeActivated -= DocumentManager_DocumentToBeActivated;
      acadApp.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
      acadApp.DocumentManager.DocumentToBeDeactivated -= DocumentManager_DocumentToBeDeactivated;
      acadApp.DocumentManager.DocumentToBeDestroyed -= DocumentManager_DocumentToBeDestroyed;
      acadApp.DocumentManager.DocumentDestroyed -= DocumentManager_DocumentDestroyed;

      hasRegisteredDocumentManagerEvents = false;
    }

    private void DocumentManager_DocumentToBeActivated(object sender, DocumentCollectionEventArgs e)
    {

    }

    private void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
    {
      HandleDocumentOpened(e.Document);
    }


    private void DocumentManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
    {

    }

    private bool CurrentDocumentDestroyed = false;

    private void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
    {
      CurrentDocumentDestroyed = HandleDocumentClosed(e.Document);
    }

    private void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
    {
      if (CurrentDocumentDestroyed)
      {
        CurrentDocumentDestroyed = false;
        OnASDocumentLost(DocumentController.Instance.CurrentDocumentName);
      }
    }

    /// <summary>
    /// Handler Advance Steel's DocumentOpened event.
    /// It is called when a document is opened
    /// </summary>
    /// <param name="document"></param>
    private void HandleDocumentOpened(Document document)
    {
      // If the current document is null, for instance if there are
      // no documents open, then set the current document, and 
      // present a message telling us where Dynamo is pointing.

      bool activatedDocument = DocumentController.Instance.HandleDocumentActivation(document);

      EnableRunButton(activatedDocument);

      if (activatedDocument)
      {
        MarkNodesAsModified();
        OnASDocumentChanged(DocumentController.Instance.CurrentDocumentName);
      }
      else
      {
        if (document == null)
        {
          if (acadApp.DocumentManager.Count == 0)
          {
            OnASNoDocumentOpened();
          }
          else
          {
            OnInvalidASDocumentActivated(DocumentController.Instance.CurrentDocumentName);
            Logger.Log(ResourceStrings.Application_LogRunButtonDisabled);
          }
        }
        else
        {
          OnInvalidASDocumentActivated(DocumentController.Instance.CurrentDocumentName);
          Logger.Log(ResourceStrings.Application_LogRunButtonDisabled);
        }
      }
    }

    /// <summary>
    /// Handle Advance Steel's DocumentClosed event.
    /// It is called when a document is closed.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    private bool HandleDocumentClosed(Document document)
    {
      bool deactivatedDocument = DocumentController.Instance.HandleDocumentDeactivation(document);
      if (deactivatedDocument)
      {
        EnableRunButton(false);
      }

      return deactivatedDocument;
    }

    private void MarkNodesAsModified()
    {
      foreach (var ws in Workspaces.OfType<HomeWorkspaceModel>())
      {
        if (ws.Nodes.Any())
        {
          foreach (var node in ws.Nodes)
          {
            node.MarkNodeAsModified();
          }
        }
      }//for
    }

    private void EnableRunButton(bool enable)
    {
      foreach (HomeWorkspaceModel ws in Workspaces.OfType<HomeWorkspaceModel>())
      {
        ws.RunSettings.RunEnabled = enable;
      }
    }

    #endregion

    #region Events Handler and Workspace override functions

    public event Action<string> ASDocumentChanged;
    private void OnASDocumentChanged(string document)
    {
      ASDocumentChanged?.Invoke(document);
    }

    public event Action<string> ASDocumentLost;
    private void OnASDocumentLost(string document)
    {
      ASDocumentLost?.Invoke(document);
    }

    public event Action<string> InvalidASDocumentActivated;
    private void OnInvalidASDocumentActivated(string document)
    {
      InvalidASDocumentActivated?.Invoke(document);
    }

    public event Action ASNoDocumentOpened;
    private void OnASNoDocumentOpened()
    {
      ASNoDocumentOpened?.Invoke();
    }

    protected override void OnWorkspaceRemoveStarted(WorkspaceModel workspace)
    {
      base.OnWorkspaceRemoveStarted(workspace);

      if (workspace is HomeWorkspaceModel)
        Services.DisposeLogic.IsClosingHomeworkspace = true;
    }

    protected override void OnWorkspaceRemoved(WorkspaceModel workspace)
    {
      base.OnWorkspaceRemoved(workspace);

      if (workspace is HomeWorkspaceModel)
      {
        Services.DisposeLogic.IsClosingHomeworkspace = false;

        HomeWorkspaceModel homeWorkspaceModel = workspace as HomeWorkspaceModel;
        homeWorkspaceModel.EvaluationStarted -= homeWorkspaceModel_EvaluationStarted;
      }

      //Unsubscribe the event
      foreach (var node in workspace.Nodes.ToList())
      {
        node.PropertyChanged -= node_PropertyChanged;
      }
    }

    protected override void OnWorkspaceAdded(WorkspaceModel workspace)
    {
      base.OnWorkspaceAdded(workspace);

      if (workspace is HomeWorkspaceModel)
      {
        HomeWorkspaceModel homeWorkspaceModel = workspace as HomeWorkspaceModel;
        homeWorkspaceModel.EvaluationStarted += homeWorkspaceModel_EvaluationStarted;
      }

      foreach (var node in workspace.Nodes.ToList())
      {
        node.PropertyChanged += node_PropertyChanged;
      }

      //if (DocumentController.Instance.CurrentDocument != null)
      //{
      //    SetRunEnabledBasedOnContext(DocumentController.Instance.CurrentDocument);
      //}
    }

    public override void OnWorkspaceClearing()
    {
      base.OnWorkspaceClearing();

      if (this.CurrentWorkspace is HomeWorkspaceModel)
      {
        foreach (NodeModel el in this.CurrentWorkspace.Nodes)
        {
          if (el.CachedValue != null && el.CachedValue.Data is SteelDbObject)
          {
            SteelDbObject steelDbObject = el.CachedValue.Data as SteelDbObject;
            LifecycleManager.GetInstance().NotifyOfUnbinding(steelDbObject.Handle);
          }
        }
      }
    }
  
    private void homeWorkspaceModel_EvaluationStarted(object sender, EventArgs e)
    {
      DisposeLogic.RunningDynamo = true;
    }

    private void node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "IsFrozen":
          //TODO: Set Element freeze state to false in SteelDbObject property to do not cleanup Advance Steel elements
          break;
      }
    }

    #endregion

    #region Properties/Fields

    public bool IsInMatchingDocumentContext
    {
      get
      {
        if (DocumentController.Instance.CurrentDocument == null)
          return false; // There's no current document stored.

        var activeDocument = acadApp.DocumentManager.MdiActiveDocument;
        if (activeDocument == null)
          return false;

        var activeDocumentHashCode = activeDocument.GetHashCode();
        return activeDocumentHashCode == DocumentController.Instance.ActiveDocumentHashCode;
      }
    }

    #endregion

    #region Overridable Methods

    protected override void ShutDownCore(bool shutdownHost)
    {
      Services.DisposeLogic.IsShuttingDown = true;
      //Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentActivationEnabled = true;

      base.ShutDownCore(shutdownHost);

      UnsubscribeDocumentManagerEvents();

      DocumentController.DisposeInstance();

      LifecycleManager.DisposeInstance();

      ModelController.CleanControls();
    }

    //public override void OnEvaluationCompleted(object sender, EvaluationCompletedEventArgs e)
    //{
    //  //Debug.WriteLine(LifecycleManager<int>.GetInstance());
    //  base.OnEvaluationCompleted(sender, e);

    //}


    public override void OnRefreshCompleted(object sender, EventArgs e)
    {
      base.OnRefreshCompleted(sender, e);

      // finally close the transaction
      DocContext.ForceCloseTransaction();

      UpdateDisplay();

      DisposeLogic.RunningDynamo = false;
    }

    /// <summary>
    /// Update Advance Steel display after run graph
    /// </summary>
    private void UpdateDisplay()
    {
      if (DocumentController.Instance.CurrentDocument != null)
      {
        //It's work with this Regen - At AS 2017 it only works with REGEN3

        //Don't use this because it doesn't update the structures project explorer
        //DocumentController.Instance.CurrentDocument.Editor.Regen();
        DocumentController.Instance.CurrentDocument.SendStringToExecute(CommandsCAD.REGEN + " ", true, false, false);
      }
    }

    #endregion

  }
}