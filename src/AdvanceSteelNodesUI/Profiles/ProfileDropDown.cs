using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteel.Nodes
{
  public abstract class ProfileDropDown : DSDropDownBase
  {
    protected const string no_items = "No items available";

    /// <summary>
    /// Public to Save in Json to working in DYN File loading
    /// </summary>
    public string NameSelected { get; set; }

    protected ProfileDropDown(string outputName) : base(outputName)
    {
      Initialize();
    }

    [JsonConstructor]
    public ProfileDropDown(string outputName, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
    {
      Initialize();
    }


    public override bool IsInputNode
    {
      get { return true; }
    }

    private void Initialize()
    {
      this.PropertyChanged += ProfileDropDown_PropertyChanged;
    }

    private void ProfileDropDown_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == (nameof(DSDropDownBase.SelectedIndex)) && SelectedIndex > -1 && Items.Any() && !Items[0].Name.Equals(no_items))
      {
        NameSelected = Items[SelectedIndex].Name;
      }
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      //string nameSelected = null;
      if (SelectedIndex > -1 && Items.Any() && !Items[0].Name.Equals(no_items))
      {
        NameSelected = Items[SelectedIndex].Name;
      }

      Items.Clear();

      List<(string, string)> listItems = GetListItems();

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

      if (!string.IsNullOrEmpty(NameSelected))
      {
        SelectedIndex = Items.IndexOf(Items.FirstOrDefault(x => x.Name == NameSelected));

        if (SelectedIndex == -1)
        {
          NameSelected = null;
        }

        return SelectionState.Done;
      }

      return SelectionState.Restore;
    }

    protected abstract List<(string, string)> GetListItems();

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

    public override void Dispose()
    {
      this.PropertyChanged -= ProfileDropDown_PropertyChanged;

      base.Dispose();
    }

  }
}
