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
	/// <summary>
	/// Advance Steel Weld Line
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class WeldLine : GraphicObject
	{
		internal WeldLine(SteelGeometry.Point3d[] astPoints, IEnumerable<string> handlesToConnect, bool isClosed = false)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();

					FilerObject existingObject = Utils.GetObject(handle);
					existingObject?.DelFromDb();

					var weld = new Autodesk.AdvanceSteel.Modelling.WeldLine(astPoints, Vector3d.kXAxis, Vector3d.kYAxis);
					weld.IsClosed = isClosed;
					weld.WriteToDb();
					HashSet<FilerObject> objectsToConnect = new HashSet<FilerObject>(ObjectsConnection.GetSteelObjectsToConnect(handlesToConnect));
					weld.Connect(objectsToConnect, AtomicElement.eAssemblyLocation.kOnSite);

					Handle = weld.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(weld);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel Weld Line By PolyCurve
		/// </summary>
		/// <param name="polyCurve"> Input polyCurve </param>
		/// <param name="objectsToConnect"> Input objects </param>
		public static WeldLine ByPolyCurve(DynGeometry.PolyCurve polyCurve, IEnumerable<SteelDbObject> objectsToConnect)
		{
			List<string> handlesList = ObjectsConnection.GetSteelDbObjectsToConnect(objectsToConnect);

			var temp = polyCurve.Curves();
			SteelGeometry.Point3d[] astArr = new SteelGeometry.Point3d[temp.Length + 1];
			for (int i = 0; i < temp.Length; i++)
			{
				Point3d startPoint = Utils.ToAstPoint(temp[i].StartPoint, true);
				astArr[i] = startPoint;
			}

			Point3d endPoint = Utils.ToAstPoint(temp[temp.Length - 1].EndPoint, true);
			astArr[temp.Length] = endPoint;

			return new WeldLine(astArr, handlesList, polyCurve.IsClosed);
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var weld = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.WeldLine;
				 
					if (weld == null)
						throw new Exception("Null weld line");
					
					weld.GetWeldPoints(out Point3d[] arrPoints, Autodesk.AdvanceSteel.Modelling.WeldPattern.eSeamPosition.kUpper);
					DynGeometry.Point[] dynPoints = Utils.ToDynPoints(arrPoints, true);
					return Autodesk.DesignScript.Geometry.PolyCurve.ByPoints(new HashSet<DynGeometry.Point> (dynPoints), weld.IsClosed);
				}
			}
		}
	}
}


