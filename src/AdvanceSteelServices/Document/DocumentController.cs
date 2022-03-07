using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  public class DocumentController
  {
    #region Properties

    /// <summary>
    /// This property represents the hash code for the current active AS
    /// Document object. If there is no active document, this value will be 
    /// set to zero. This property is made an 'int' because it is primarily
    /// meant to indicate the active document for comparison purposes, no 
    /// Document specific operations should be allowed on it. This property 
    /// is updated when a document switch or a document closure happens. The
    /// reason this property is needed because the following property cannot
    /// reliably indicate the current active document:
    /// 
    ///     acadApp.DocumentManager.MdiActiveDocument.GetHashCode()
    /// 
    /// </summary>
    public int ActiveDocumentHashCode { get; private set; }

    /// <summary>
    /// Current document name opened - Don't erase this property after lost document
    /// </summary>
    public string CurrentDocumentName { get; private set; }

    public Document CurrentDocument { get; private set; }

    #endregion

    #region Document Events

    public event EventHandler<DocumentEventArgs> DocumentAttached;
    public event EventHandler<DocumentEventArgs> DocumentDetached;
    public event EventHandler<ObjectCADEventArgs> ObjectUpdated;

    private void OnDocumentAttached(object sender, DocumentEventArgs documentEventArgs)
    {
      DocumentAttached?.Invoke(sender, documentEventArgs);
    }

    private void OnDocumentDetached(object sender, DocumentEventArgs documentEventArgs)
    {
      DocumentDetached?.Invoke(sender, documentEventArgs);
    }

    private void OnObjectUpdated(object sender, ObjectCADEventArgs objectCADEventArgs)
    {
      ObjectUpdated?.Invoke(sender, objectCADEventArgs);
    }

    public void StructureRenamed(ObjectId objectIdStructure)
    {
      OnObjectUpdated(this, new ObjectCADEventArgs(ObjectCADEventArgs.UpdateType.Modified, new List<ObjectId>() { objectIdStructure }));
    }

    #endregion

    #region Constructor and Singleton

    private DocumentController()
    {

    }

    private static DocumentController instance;

    public static DocumentController Instance
    {
      get
      {
        //Verify if is it necessary to use lock - In Revit is necessary
        return instance ?? (instance = new DocumentController());
      }
    }

    /// <summary>
    /// Dispose the singleton instance
    /// </summary>
    public static void DisposeInstance()
    {
      if (instance != null)
      {
        if (Instance.CurrentDocument != null)
        {
          Instance.HandleDocumentDeactivation(Instance.CurrentDocument, false);
        }

        instance = null;
      }
    }

    #endregion


    /// <summary>
    /// Handle document activation, closure. See ActiveDocumentHashCode 
    /// for more details.
    /// </summary>
    /// <param name="document">The Autodesk.AutoCAD.ApplicationServices.Document object that is being 
    /// activated. This parameter will be null if the currently active 
    /// document is closed. If there is another document when the current 
    /// document is closed, the next document will be activated again 
    /// after active document is invalidated here.</param>
    /// <returns></returns>
    public bool HandleDocumentActivation(Document document)
    {
      if (document == null)
      {
        if (acadApp.DocumentManager.Count == 0)
        {
          //Last document closed - Reset hash code after the last document closed
          ActiveDocumentHashCode = 0;
        }

        return false;
      }

      var documentHashCode = document.GetHashCode();

      //If there is a active document don't active another one
      if (ActiveDocumentHashCode != 0)
      {
        if (ActiveDocumentHashCode == documentHashCode)
        {
          return true;
        }

        return false;
      }

      //Set hash code and document
      ActiveDocumentHashCode = documentHashCode;
      CurrentDocument = document;
      CurrentDocumentName = Path.GetFileNameWithoutExtension(document.Name);

      //document.Database.ObjectAppended += Database_ObjectAppended;
      document.Database.ObjectErased += Database_ObjectErased;
      document.Database.ObjectOpenedForModify += Database_ObjectOpenedForModify;
      document.Database.ObjectModified += Database_ObjectModified;

      document.CommandEnded += Document_CommandEnded;
      document.CommandWillStart += Document_CommandWillStart;
      document.CommandCancelled += Document_CommandCancelled;
      document.CommandFailed += Document_CommandFailed;

      OnDocumentAttached(this, new DocumentEventArgs() { Document = document });

      return true;
    }

    public bool HandleDocumentDeactivation(Document document, bool triggerEvents = true)
    {
      if (document == null)
      {
        return false;
      }

      if (ActiveDocumentHashCode == 0)
      {
        return false;
      }

      if (ActiveDocumentHashCode != document.GetHashCode())
      {
        return false;
      }

      //Reset hash code
      CurrentDocument = null;

      //document.Database.ObjectAppended -= Database_ObjectAppended;
      document.Database.ObjectErased -= Database_ObjectErased;
      document.Database.ObjectOpenedForModify -= Database_ObjectOpenedForModify;
      document.Database.ObjectModified -= Database_ObjectModified;

      document.CommandEnded -= Document_CommandEnded;
      document.CommandWillStart -= Document_CommandWillStart;
      document.CommandCancelled -= Document_CommandCancelled;
      document.CommandFailed -= Document_CommandFailed;

      if (triggerEvents)
      {
        OnDocumentDetached(this, new DocumentEventArgs() { Document = document });
      }

      return true;
    }

    private bool commandStarted = false;
    //private List<ObjectId> listAddedObject = new List<ObjectId>();
    private List<ObjectId> listEditedObject = new List<ObjectId>();
    private List<ObjectId> listDeletedObject = new List<ObjectId>();

    private void ClearLists()
    {
      commandStarted = false;

      //listAddedObject.Clear();
      listEditedObject.Clear();
      listDeletedObject.Clear();
    }

    private void Document_CommandFailed(object sender, CommandEventArgs e)
    {
      //Debug.WriteLine("Comando Falhou {0} - {1}", e.GlobalCommandName, "*");

      ClearLists();
    }

    private void Document_CommandCancelled(object sender, CommandEventArgs e)
    {
      //Debug.WriteLine("Comando Cancelado {0} - {1}", e.GlobalCommandName, "*");

      ClearLists();
    }

    private void Document_CommandWillStart(object sender, CommandEventArgs e)
    {
      if (DisposeLogic.RunningDynamo)
        return;

      if (e.GlobalCommandName.Equals(CommandsCAD.REGEN))
        return;

      if (e.GlobalCommandName.Equals(CommandsCAD.ASTM4COMMMULTIEDIT) && CurrentDocument != null)
      {
        //TODO: Insert updated Advance Steel objects in listEditedObject
      }

      //Debug.WriteLine("Command will start {0} - {1}", e.GlobalCommandName, "*");

      commandStarted = true;
    }

    private void Document_CommandEnded(object sender, CommandEventArgs e)
    {
      //Se tiver rodando o dynamo não deve perturbar os Nodes para avisar que os nodes foram modificados para não rodar novamentes os nodes seguintes
      if (DisposeLogic.RunningDynamo)
        return;

      if (!commandStarted)
        return;

      //Debug.WriteLine("Comando {0} - {1}", e.GlobalCommandName, "*");

      //Só roda depois que executar o dynamo mas também só trata os elementos que forem apagados

      TriggerEvents();
    }

    private void TriggerEvents()
    {
      //if (listAddedObject.Any())
      //{
      //    OnObjectsUpdated(this, new ObjectASCADEventArgs(ObjectASCADEventArgs.UpdateType.Added, new List<ObjectId>(listAddedObject.ToArray())));
      //}

      if (listDeletedObject.Any())
      {
        OnObjectUpdated(this, new ObjectCADEventArgs(ObjectCADEventArgs.UpdateType.Deleted, listDeletedObject.ToList()));
      }

      if (listEditedObject.Any(x => !x.IsErased))
      {
        OnObjectUpdated(this, new ObjectCADEventArgs(ObjectCADEventArgs.UpdateType.Modified, listEditedObject.Where(x => !x.IsErased).ToList()));
      }

      ClearLists();
    }

    private bool structureGroupUpdated = false;

    private void Database_ObjectOpenedForModify(object sender, Autodesk.AutoCAD.DatabaseServices.ObjectEventArgs e)
    {
      if (DisposeLogic.RunningDynamo)
        return;

      //Debug.WriteLine("Object is opened for Edit {0} - {1} - {2} - {3}", e.DBObject.GetType().ToString(), e.DBObject.GetRXClass().DxfName, e.DBObject.ObjectId.Handle.ToString(), "*");

      if (e.DBObject.ObjectId.ObjectClass.DxfName.Equals(DxfCodeAdvanceSteel.GroupFilerObject))
      {
        //Group (Check before commandStarted, because it's change without command call)
        listEditedObject.Add(e.DBObject.ObjectId);
        structureGroupUpdated = true;
      }

      if (!commandStarted)
        return;

      if (!e.DBObject.ObjectId.IsErased && !listEditedObject.Contains(e.DBObject.ObjectId))
        listEditedObject.Add(e.DBObject.ObjectId);
    }

    //private void Database_ObjectAppended(object sender, Autodesk.AutoCAD.DatabaseServices.ObjectEventArgs e)
    //{
    //    //if (DisposeLogic.RunningDynamo)
    //    //    return;

    //    //To add an object in list the command must be opened
    //    //if (commandStarted)
    //    //    listAddedObject.Add(e.DBObject.ObjectId);
    //}

    private void Database_ObjectModified(object sender, Autodesk.AutoCAD.DatabaseServices.ObjectEventArgs e)
    {
      //Debug.WriteLine("Edited Object {0} - {1} - {2} - {3}", e.DBObject.GetType().ToString(), e.DBObject.GetRXClass().DxfName, e.DBObject.ObjectId.Handle.ToString(), "*");

      if (structureGroupUpdated)
      {
        //It must wait this event Database_ObjectModified for the group object to be modified

        structureGroupUpdated = false;
        TriggerEvents();
      }
    }

    private void Database_ObjectErased(object sender, ObjectErasedEventArgs e)
    {
      if (DisposeLogic.RunningDynamo)
        return;

      //Debug.WriteLine("Erased Object {0} - {1} - {2} - {3}", e.DBObject.GetType().ToString(), e.DBObject.GetRXClass().DxfName, e.DBObject.ObjectId.Handle.ToString(), "*");

      if (e.Erased)
      {
        if (commandStarted)
        {
          listDeletedObject.Add(e.DBObject.ObjectId);
        }
        else if (e.DBObject.ObjectId.ObjectClass.DxfName.Equals(DxfCodeAdvanceSteel.StructureFilerObject) ||
                 e.DBObject.ObjectId.ObjectClass.DxfName.Equals(DxfCodeAdvanceSteel.GroupFilerObject))
        {
          listDeletedObject.Add(e.DBObject.ObjectId);
          TriggerEvents();
        }
      }
    }

  }
}
