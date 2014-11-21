using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.Nodes;
using Dynamo.Utilities;
using Dynamo;
using DSIronPython;
using AdvanceSteel.Nodes;


namespace Dynamo.Applications.Models
{
    public class AdvanceSteelModel : DynamoModel
    {
        public new static AdvanceSteelModel Start()
        {
            return AdvanceSteelModel.Start(new StartConfiguration());
        }

        public new static AdvanceSteelModel Start(StartConfiguration configuration)
        {
            // where necessary, assign defaults
            if (string.IsNullOrEmpty(configuration.Context))
                configuration.Context = "advance steel";

            if (string.IsNullOrEmpty(configuration.DynamoCorePath))
            {
                var asmLocation = Assembly.GetExecutingAssembly().Location;
                configuration.DynamoCorePath = Path.GetDirectoryName(asmLocation);
            }

            if (configuration.Preferences == null)
                configuration.Preferences = new PreferenceSettings();

            return new AdvanceSteelModel(configuration);
        }

        private AdvanceSteelModel(StartConfiguration configuration) :
            base(configuration)
        {
            Context = configuration.Context;

            string corePath = configuration.DynamoCorePath;
            bool isTestMode = configuration.StartInTestMode;

            SetupPython();
        }
        protected override void ShutDownCore(bool shutdownHost)
        {
            DSNodeServices.DisposeLogic.IsShuttingDown = true;
            Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentActivationEnabled = true;

            base.ShutDownCore(shutdownHost);
        }
        #region Initialization

        private bool setupPython;
        private void SetupPython()
        {
            
            if (setupPython) return;
            
            //TO_DO
            
            setupPython = true;
           
        }
        #endregion
    }
}