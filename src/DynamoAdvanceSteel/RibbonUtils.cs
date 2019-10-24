using Autodesk.Windows;

namespace Dynamo.Applications.AdvanceSteel
{
  public class RibbonUtils
  {
    public static string DynamoASTabUID = "Add-ins";
    public static string DynamoASPanelUID = "ID_PanelOnlineDocuments";
    public static string DynamoASButtonUID = "DYNAMOAS";

    public static void SetEnabled(string tabName, string panelUID, string buttonUID, bool enabled)
    {
      RibbonTabCollection oTabs = Autodesk.Windows.ComponentManager.Ribbon.Tabs;
      foreach (RibbonTab oTab in oTabs)
      {
        if (oTab.Title == tabName)
        {
          RibbonPanelCollection bPans = oTab.Panels;

          foreach (RibbonPanel oPanel in bPans)
          {
            if (oPanel.UID == panelUID)
            {
              foreach (RibbonItem item in oPanel.Source.Items)
              {
                if (item.UID == buttonUID)
                {
                  item.IsEnabled = enabled;
                }
              }
            }
          }
          break;
        }
      }
    }
  }
}