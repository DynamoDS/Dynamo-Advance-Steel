using AdvanceSteel.Services;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using System.IO;
using System.Reflection;

namespace Dynamo.Applications.Models
{
  public class AdvanceSteelModel : DynamoModel
  {
    public new static AdvanceSteelModel Start()
    {
      return Start(new DefaultStartConfiguration());
    }

    public new static AdvanceSteelModel Start(IStartConfiguration configuration)
    {
      // where necessary, assign defaults
      if (string.IsNullOrEmpty(configuration.Context))
        configuration.Context = "Advance Steel";

      if (string.IsNullOrEmpty(configuration.DynamoCorePath))
      {
        var asmLocation = Assembly.GetExecutingAssembly().Location;
        configuration.DynamoCorePath = Path.GetDirectoryName(asmLocation);
      }

      //if (configuration.Preferences == null)
      //    configuration.Preferences = new PreferenceSettings();

      return new AdvanceSteelModel(configuration);
    }

    private AdvanceSteelModel(IStartConfiguration configuration) :
        base(configuration)
    {
      DisposeLogic.IsShuttingDown = false;

      //SetupPython();
    }

    protected override void ShutDownCore(bool shutdownHost)
    {
      DisposeLogic.IsShuttingDown = true;
      //Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentActivationEnabled = true;

      base.ShutDownCore(shutdownHost);

      DynamoASUtils.modifyRibbon(DynamoASUtils.tabUIDDynamoAS, DynamoASUtils.panelUIDDynamoAS, DynamoASUtils.buttonUIDDynamoAS, true);
    }

    protected override void OnWorkspaceRemoveStarted(WorkspaceModel workspace)
    {
      base.OnWorkspaceRemoveStarted(workspace);

      if (workspace is HomeWorkspaceModel)
        DisposeLogic.IsClosingHomeworkspace = true;
    }

    protected override void OnWorkspaceRemoved(WorkspaceModel workspace)
    {
      base.OnWorkspaceRemoved(workspace);

      if (workspace is HomeWorkspaceModel)
        DisposeLogic.IsClosingHomeworkspace = false;
    }
  }
}