using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;
using System;

namespace AdvanceSteel.Nodes.ConnectionObjects
{
  [IsVisibleInDynamoLibrary(false)]
  public class ObjectsConnection
  {
		
		/// <summary>
		/// Set Assembly Location 
		/// </summary>
		/// <param name="location">Input assembly location</param>
		/// <param name="aux">Input aux </param>
		/// <returns></returns>
		public static void SetAssemblyLocation(Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern aux, int location)
		{
			var temp = Utils.GetObject(aux.Handle) as Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern;
			if (temp.IsKindOf(FilerObject.eObjectType.kCircleScrewBoltPattern) || temp.IsKindOf(FilerObject.eObjectType.kFinitRectScrewBoltPattern))
			{
				temp.AssemblyLocation = (AtomicElement.eAssemblyLocation)location;
			}
		}
  }
}



