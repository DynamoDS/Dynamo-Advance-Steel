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
            return AdvanceSteelModel.Start(new DefaultStartConfiguration());
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

            if (configuration.Preferences == null)
                configuration.Preferences = new PreferenceSettings();

            return new AdvanceSteelModel(configuration);
        }

        private AdvanceSteelModel(IStartConfiguration configuration) :
            base(configuration)
        {
            string corePath = configuration.DynamoCorePath;
            bool isTestMode = configuration.StartInTestMode;

            SetupPython();
        }
        protected override void ShutDownCore(bool shutdownHost)
        {
            DynamoServices.DisposeLogic.IsShuttingDown = true;
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