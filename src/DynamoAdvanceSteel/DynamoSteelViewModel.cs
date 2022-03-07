using System;
using Autodesk.AutoCAD.ApplicationServices;
using Dynamo.Applications.AdvanceSteel.Services;
using Dynamo.Interfaces;
using Dynamo.ViewModels;
using Dynamo.Wpf.ViewModels.Core;
using Dynamo.Wpf.ViewModels.Watch3D;

namespace Dynamo.Applications.AdvanceSteel
{
  public class DynamoSteelViewModel : DynamoViewModel
  {
    private DynamoSteelViewModel(StartConfiguration startConfiguration) :
     base(startConfiguration)
    {
      var model = (DynamoSteelModel)Model;

      model.ASDocumentChanged += Model_ASDocumentChanged;
      model.ASDocumentLost += Model_ASDocumentLost;
      model.InvalidASDocumentActivated += Model_InvalidASDocumentActivated;
      model.ASNoDocumentOpened += Model_ASNoDocumentOpened;
    }

    public new static DynamoSteelViewModel Start(StartConfiguration startConfiguration)
    {
      if (startConfiguration.DynamoModel == null)
      {
        startConfiguration.DynamoModel = DynamoSteelModel.Start();
      }
      else
      {
        if (startConfiguration.DynamoModel.GetType() != typeof(DynamoSteelModel))
          throw new Exception(ResourceStrings.Application_ConstructorViewModel);
      }

      if (startConfiguration.WatchHandler == null)
      {
        startConfiguration.WatchHandler = new DefaultWatchHandler(startConfiguration.DynamoModel.PreferenceSettings);
      }

      if (startConfiguration.Watch3DViewModel == null)
      {
        startConfiguration.Watch3DViewModel =
            HelixWatch3DViewModel.TryCreateHelixWatch3DViewModel(
                null,
                new Watch3DViewModelStartupParams(startConfiguration.DynamoModel),
                startConfiguration.DynamoModel.Logger);
      }

      return new DynamoSteelViewModel(startConfiguration);
    }

    private void Model_ASDocumentChanged(string document)
    {
      var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
      hsvm.CurrentNotificationLevel = NotificationLevel.Moderate;
      hsvm.CurrentNotificationMessage = String.Format(ResourceStrings.Application_DocumentRunning, document);
    }

    private void Model_ASDocumentLost(string document)
    {
      var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
      hsvm.CurrentNotificationLevel = NotificationLevel.Error;

      hsvm.CurrentNotificationMessage = String.Format(ResourceStrings.Application_DocumentClosed, document);

      CloseHomeWorkspaceCommand.Execute(null);
      ExitCommand.Execute(null);
    }

    private void Model_InvalidASDocumentActivated(string document)
    {
      var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
      hsvm.CurrentNotificationLevel = NotificationLevel.Error;

      hsvm.CurrentNotificationMessage = String.Format(ResourceStrings.Application_DocumentInvalid, document);
    }

    private void Model_ASNoDocumentOpened()
    {
      var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
      hsvm.CurrentNotificationLevel = NotificationLevel.Error;
      hsvm.CurrentNotificationMessage = ResourceStrings.Application_NoDocument;
    }

    protected override void UnsubscribeAllEvents()
    {
      var model = (DynamoSteelModel)Model;

      model.ASDocumentChanged -= Model_ASDocumentChanged;
      model.ASDocumentLost -= Model_ASDocumentLost;
      model.InvalidASDocumentActivated -= Model_InvalidASDocumentActivated;
      model.ASNoDocumentOpened -= Model_ASNoDocumentOpened;

      base.UnsubscribeAllEvents();
    }

  }
}
