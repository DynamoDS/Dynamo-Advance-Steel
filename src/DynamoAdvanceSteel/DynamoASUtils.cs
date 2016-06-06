using Autodesk.Windows;

namespace Dynamo.Applications.Models
{
  public class DynamoASUtils
  {
    public static string tabUIDDynamoAS = "Add-ins";
    public static string panelUIDDynamoAS = "ID_PanelOnlineDocuments";
    public static string buttonUIDDynamoAS = "DYNAMOAS";

    public static void modifyRibbon(string strTabName, string strPanelUID, string strButtonUID, bool bEnable)
    {
      RibbonTabCollection oTabs = Autodesk.Windows.ComponentManager.Ribbon.Tabs;
      foreach (RibbonTab oTab in oTabs)
      {
        if (oTab.Title == strTabName)
        {
          RibbonPanelCollection bPans = oTab.Panels;

          foreach (RibbonPanel oPanel in bPans)
          {
            if (oPanel.UID == strPanelUID)
            {
              foreach (RibbonItem item in oPanel.Source.Items)
              {
                if (item.UID == strButtonUID)
                {
                  item.IsEnabled = bEnable;
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