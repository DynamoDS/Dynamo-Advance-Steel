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
	/// Advance Steel Circular Bolt Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class CircularBoltPattern : GraphicObject
	{
		internal void UpdateBoltPattern(ref Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern toUpdate, int noScrews, double radius)
		{
			toUpdate.NumberOfScrews = noScrews;
			toUpdate.Radius = Utils.ToInternalUnits(radius, true);
		}
		internal CircularBoltPattern(SteelGeometry.Point3d astPointRef, double radius, IEnumerable<string> handlesToConnect, int nBolts, SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern bolts = null;

					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						bolts = new Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern(astPointRef, vx, vy);
						UpdateBoltPattern(ref bolts, nBolts, radius);

						bolts.WriteToDb();

						HashSet<FilerObject> objectsToConnect = new HashSet<FilerObject>();
						objectsToConnect = ObjectsConnection.GetSteelObjectsToConnect(handlesToConnect);

						bolts.Connect(objectsToConnect, AtomicElement.eAssemblyLocation.kOnSite);
					}
					else
					{
						bolts = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern;

						if (bolts != null && bolts.IsKindOf(FilerObject.eObjectType.kCircleScrewBoltPattern))
						{
							bolts.RefPoint = astPointRef;
							bolts.XDirection = vx;
							bolts.YDirection = vy;
							UpdateBoltPattern(ref bolts, nBolts, radius);

							HashSet<FilerObject> filerObjects = new HashSet<FilerObject>();
							filerObjects = ObjectsConnection.GetFilerObjects(handlesToConnect);

							bolts.Connect(filerObjects, AtomicElement.eAssemblyLocation.kOnSite);
						}
						else
							throw new System.Exception("Not a circular pattern");
					}

					Handle = bolts.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(bolts);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel Circular Pattern By Circle
		/// </summary>
		/// <param name="circle">Input circle</param>
		/// <param name="objectsToConnect">Input objects</param>
		/// <param name="nBolts">Input no. of bolts</param>
		/// <returns></returns>
		public static CircularBoltPattern ByCircle(Autodesk.DesignScript.Geometry.Circle circle, IEnumerable<SteelDbObject> objectsToConnect, int nBolts)
		{
			var norm = Utils.ToAstVector3d(circle.Normal, true);
			var vx = norm.GetPerpVector();
			var vy = norm.CrossProduct(vx);

			List<string> handlesList = new List<string>();
			handlesList = ObjectsConnection.GetSteelDbObjectsToConnect(objectsToConnect);

			return new CircularBoltPattern(Utils.ToAstPoint(circle.CenterPoint, true), circle.Radius, handlesList, nBolts, vx, vy);
		}

		/// <summary>
		/// Create an Advance Steel Circular Pattern By Point
		/// </summary>
		/// <param name="point">Input circle center point</param>
		/// <param name="radius">Input objects</param>
		/// <param name="normal">Input norm </param>
		/// <param name="objectsToConnect">Input objects</param>
		/// <param name="nBolts">Input no. of bolts</param>
		/// <returns></returns>
		public static CircularBoltPattern ByCenterPointRadiusNormal(DynGeometry.Point point, double radius, IEnumerable<SteelDbObject> objectsToConnect, int nBolts, DynGeometry.Vector normal)
		{
			SteelGeometry.Point3d astPointRef = Utils.ToAstPoint(point, true);

			var norm = Utils.ToAstVector3d(normal, true);
			var vx = norm.GetPerpVector();
			var vy = norm.CrossProduct(vx);

			IEnumerable<string> handles = objectsToConnect.Select(obj => obj.Handle);

			return new CircularBoltPattern(Utils.ToAstPoint(point, true), radius, handles, nBolts, vx, vy);
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var boltPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern;
					if (boltPattern == null)
					{
						throw new Exception("Null bolt pattern");
					}
						
					using (var point = Utils.ToDynPoint(boltPattern.RefPoint, true))
					using (var norm = Utils.ToDynVector(boltPattern.Normal, true))
					{
						return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalUnits(boltPattern.Radius, true), norm);
					}
				}
			}
		}
	}
}