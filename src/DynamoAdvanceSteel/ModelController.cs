using Dynamo.Controls;
using Dynamo.ViewModels;

namespace Dynamo.Applications.AdvanceSteel
{
  public static class ModelController
  {
    public static DynamoViewModel ViewModel { get; set; }

    public static DynamoSteelModel DynamoModel { get; set; }
    public static DynamoView DynamoView { get; set; }

    /// <summary>
    /// Clean all controls
    /// Call at shutdown event dynamo
    /// </summary>
    public static void CleanControls()
    {
      DynamoModel = null;
      ViewModel = null;
      DynamoView = null;
    }
  }
}
