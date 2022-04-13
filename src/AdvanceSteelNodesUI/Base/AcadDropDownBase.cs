using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AdvanceSteel.DocumentManagement;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;

using CADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace AdvanceSteel.Nodes
{
  public abstract class AcadDropDownBase : DSDropDownBase
  {
    protected const string no_items = "No items available";

    /// <summary>
    /// Public property to save to Json then work on loading DYN File
    /// </summary>
    public string HandleSelected { get; set; }

    protected AcadDropDownBase(string value)
        : base(value)
    {
      SubscribeDocumentEvents();
    }

    [JsonConstructor]
    public AcadDropDownBase(string value, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(value, inPorts, outPorts)
    {
      SubscribeDocumentEvents();
    }

    #region Document Events

    private void SubscribeDocumentEvents()
    {
      this.PropertyChanged += AutocadDropDown_PropertyChanged;
      //DocumentController.Instance.ObjectUpdated += DocumentController_ObjectUpdated;
      //DocumentController.Instance.DocumentDetached += DocumentController_DocumentDetached;
    }


    private void UnsubscribeDocumentEvents()
    {
      this.PropertyChanged -= AutocadDropDown_PropertyChanged;
      //DocumentController.Instance.ObjectUpdated -= DocumentController_ObjectUpdated;
      //DocumentController.Instance.DocumentDetached -= DocumentController_DocumentDetached;
    }

    private void AutocadDropDown_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == (nameof(DSDropDownBase.SelectedIndex)) && SelectedIndex > -1 && Items.Any() && !Items[0].Name.Equals(no_items))
      {
        HandleSelected = ((CADObjectId)Items[SelectedIndex].Item).Handle.ToString();
      }
    }

    //private void DocumentController_ObjectUpdated(object sender, ObjectASCADEventArgs e)
    //{
    //  if (e.Operation == ObjectASCADEventArgs.UpdateType.Deleted)
    //  {
    //    Updater_ElementsDeleted(e.ListObjectId);
    //  }
    //  else if (e.Operation == ObjectASCADEventArgs.UpdateType.Modified)
    //  {
    //    Updater_ElementsUpdated(e.ListObjectId);
    //  }
    //}

    //private void DocumentController_DocumentDetached(object sender, DocumentEventArgs e)
    //{
    //  PopulateItems();
    //  OnNodeModified();
    //}

    private void Updater_ElementsUpdated(List<CADObjectId> listUpdated)
    {
      if (!listUpdated.Any())
        return;

      if (!Items.Any(x => x.Item is CADObjectId))
        return;

      bool hasElements = Items.Any(x => !((CADObjectId)x.Item).IsErased && listUpdated.Contains((CADObjectId)x.Item));

      if (hasElements)
      {
        PopulateItems();
        OnNodeModified();
      }
    }

    private void Updater_ElementsDeleted(List<CADObjectId> listDeleted)
    {
      if (!listDeleted.Any())
        return;

      if (!Items.Any(x => x.Item is CADObjectId))
        return;

      bool hasElements = Items.Any(x => ((CADObjectId)x.Item).IsErased && listDeleted.Contains((CADObjectId)x.Item));

      if (hasElements)
      {
        PopulateItems();
        OnNodeModified();
      }
    }

    public override void Dispose()
    {
      UnsubscribeDocumentEvents();

      base.Dispose();
    }

    #endregion

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      if (SelectedIndex > -1 && Items.Any() && !Items[0].Name.Equals(no_items))
      {
        HandleSelected = ((CADObjectId)Items[SelectedIndex].Item).Handle.ToString();
      }

      Items.Clear();

      List<(CADObjectId, string)> listItems = null;

      using (new DocContextTemporary())
      {
        listItems = GetListItems();
      }

      if (listItems == null || !listItems.Any())
      {
        Items.Add(new DynamoDropDownItem(no_items, null));
        SelectedIndex = 0;
        return SelectionState.Done;
      }

      foreach (var item in listItems)
      {
        Items.Add(new DynamoDropDownItem(item.Item2, item.Item1));
      }

      Items = Items.OrderBy(x => x.Name).ToObservableCollection();

      if (!string.IsNullOrEmpty(HandleSelected))
      {
        SelectedIndex = Items.IndexOf(Items.FirstOrDefault(x => ((CADObjectId)x.Item).Handle.ToString() == HandleSelected));

        if (SelectedIndex == -1)
        {
          HandleSelected = null;
        }

        return SelectionState.Done;
      }

      return SelectionState.Restore;
    }

    protected abstract List<(CADObjectId, string)> GetListItems();

    /// <summary>
    /// Whether it have valid Enumeration values to the output
    /// </summary>
    /// <param name="itemValueToIgnore"></param>
    /// <param name="selectedValueToIgnore"></param>
    /// <returns>true is that there are valid values to output,false is that only a null value to output</returns>
    public Boolean CanBuildOutputAst(string itemValueToIgnore = null, string selectedValueToIgnore = null)
    {
      if (Items.Count == 0 || SelectedIndex < 0)
        return false;
      if (Items[0].Name == no_items)
        return false;
      if (!string.IsNullOrEmpty(itemValueToIgnore) && Items[0].Name == itemValueToIgnore)
        return false;
      if (!string.IsNullOrEmpty(selectedValueToIgnore) && Items[SelectedIndex].Name == selectedValueToIgnore)
        return false;

      return true;
    }

  }

  /// <summary>
  /// TODO: Remove this class after pull request #69
  /// </summary>
  internal class DocContextTemporary : IDisposable
  {
    public DocContextTemporary()
    {
      EnsureInContext();
    }

    public void Dispose()
    {
      LeaveContext();
    }

    private Autodesk.AdvanceSteel.CADAccess.Transaction SteelTransaction = null;
    private bool DocumentLocked = false;

    private void EnsureInContext()
    {
      if (SteelTransaction != null || DocumentLocked == true)
        throw new System.Exception("Nested context");

      DocumentLocked = DocumentManager.LockCurrentDocument();

      if (DocumentLocked == true)
      {
        SteelTransaction = Autodesk.AdvanceSteel.CADAccess.TransactionManager.StartTransaction();
      }

      if (DocumentLocked == false || SteelTransaction == null)
      {
        throw new System.Exception("Failed to access Document");
      }
    }

    private void LeaveContext()
    {
      if (SteelTransaction != null)
      {
        SteelTransaction.Commit();
        SteelTransaction = null;
      }

      if (DocumentLocked == true)
      {
        DocumentLocked = DocumentManager.UnlockCurrentDocument();
        DocumentLocked = false;
      }
    }
  }
}