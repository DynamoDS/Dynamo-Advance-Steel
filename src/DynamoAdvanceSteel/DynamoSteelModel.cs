using Dynamo.Graph.Workspaces;
using Dynamo.Migration.AdvanceSteel;
using Dynamo.Models;
using System.IO;
using System.Reflection;

namespace Dynamo.Applications.AdvanceSteel
{
  public class DynamoSteelModel : DynamoModel
  {
    public new static DynamoSteelModel Start()
    {
      return Start(new DefaultStartConfiguration());
    }

    public new static DynamoSteelModel Start(IStartConfiguration configuration)
    {
      // where necessary, assign defaults
      if (string.IsNullOrEmpty(configuration.Context))
      {
        configuration.Context = "Advance Steel";
      }

      if (string.IsNullOrEmpty(configuration.DynamoCorePath))
      {
        var asmLocation = Assembly.GetExecutingAssembly().Location;
        configuration.DynamoCorePath = Path.GetDirectoryName(asmLocation);
      }

      //if (configuration.Preferences == null)
      //    configuration.Preferences = new PreferenceSettings();

      return new DynamoSteelModel(configuration);
    }

    private DynamoSteelModel(IStartConfiguration configuration) :
        base(configuration)
    {
      Services.DisposeLogic.IsShuttingDown = false;

      MigrationManager.MigrationTargets.Add(typeof(WorkspaceMigrations));
      //SetupPython();
    }

    protected override void ShutDownCore(bool shutdownHost)
    {
      Services.DisposeLogic.IsShuttingDown = true;
      //Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentActivationEnabled = true;

      base.ShutDownCore(shutdownHost);

      RibbonUtils.SetEnabled(RibbonUtils.tabUIDDynamoAS, RibbonUtils.panelUIDDynamoAS, RibbonUtils.buttonUIDDynamoAS, true);
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
        Services.DisposeLogic.IsClosingHomeworkspace = false;
    }
  }
}