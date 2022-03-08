using Autodesk.Windows;
using System.Linq;

namespace Dynamo.Applications.AdvanceSteel
{
  public class RibbonUtils
  {
    private const string DynamoASTabUID = "Add-ins";
    private const string DynamoASPanelUID = "ID_PanelOnlineDocuments";
    private const string DynamoASButtonUID = "DYNAMOAS";

    public static void SetEnabledDynamoButton(bool pEnable)
    {
      SetEnabledButton(DynamoASButtonUID, pEnable);
    }

    private static void SetEnabledButton(string pButtonUID, bool pEnable)
    {
      RibbonTabCollection tabs = Autodesk.Windows.ComponentManager.Ribbon.Tabs;

      RibbonButton item = tabs.FirstOrDefault(x => x.Title == DynamoASTabUID)?.Panels.FirstOrDefault(x => x.UID == DynamoASPanelUID)?.Source.Items.FirstOrDefault(x => x.UID == pButtonUID) as RibbonButton;
    
      if (item != null)
      {
        item.IsEnabled = pEnable;
      }

      //Autodesk.Windows.ComponentManager.Ribbon.UpdateLayout();
    }

  }
}