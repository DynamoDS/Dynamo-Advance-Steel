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
	/// Advance Steel Circular Anchor Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class CircularAnchorPattern : GraphicObject
	{
		public AssemblyLocation AssemblyLocation
		{
			get
			{
				lock (access_obj)
				{
					using (var ctx = new SteelServices.DocContext())
					{
						var anchors = Utils.GetObject(this.Handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;
						return ObjectsConnection.GetDynAssemblyLocation(anchors.AssemblyLocation);

					}
				}
			}
		}

		internal void UpdateAnchorPattern(Autodesk.AdvanceSteel.Modelling.AnchorPattern toUpdate, int noScrews, double radius)
		{
			toUpdate.NumberOfScrews = noScrews;
			toUpdate.Radius = Utils.ToInternalUnits(radius, true);
		}
		internal CircularAnchorPattern(SteelGeometry.Point3d astPointRef, double radius, IEnumerable<string> handlesToConnect, int nAnchors, SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy, AssemblyLocation assemblyLocation)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.AnchorPattern anchors = null;

					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						anchors = new Autodesk.AdvanceSteel.Modelling.AnchorPattern(astPointRef, vx, vy);
						anchors.ArrangerType = Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle;
						anchors.WriteToDb();
					}
					else
					{
						anchors = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;

						if (anchors != null && anchors.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
						{
							anchors.RefPoint = astPointRef;
							anchors.XDirection = vx;
							anchors.YDirection = vy;
							
						}
						else
							throw new System.Exception("Not a circular pattern");
					}

					UpdateAnchorPattern(anchors, nAnchors, radius);

					HashSet<FilerObject> filerObjects = ObjectsConnection.GetFilerObjects(handlesToConnect);
					anchors.Connect(filerObjects, ObjectsConnection.GetSteelAssemblyLocation(assemblyLocation));

					Handle = anchors.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel Circular Anchor Pattern By Circle
		/// </summary>
		/// <param name="circle">Input circle</param>
		/// <param name="objectsToConnect">Input objects</param>
		/// <param name="nAnchors">Input no. of anchors</param>
		/// <param name="location">Input location</param>
		/// <returns></returns>
		public static CircularAnchorPattern ByCircle(Autodesk.DesignScript.Geometry.Circle circle, IEnumerable<SteelDbObject> objectsToConnect, int nAnchors, AssemblyLocation location)
		{
			var norm = Utils.ToAstVector3d(circle.Normal, true);
			var vx = norm.GetPerpVector();
			var vy = norm.CrossProduct(vx);
			List<string> handlesList = ObjectsConnection.GetSteelDbObjectsToConnect(objectsToConnect);
			return new CircularAnchorPattern(Utils.ToAstPoint(circle.CenterPoint, true), circle.Radius, handlesList, nAnchors, vx, vy, location);
		}

		/// <summary>
		/// Create an Advance Steel Circular Anchor Pattern By Center Point Radius Normal
		/// </summary>
		/// <param name="point">Input circle center point</param>
		/// <param name="radius">Input objects</param>
		/// <param name="normal">Input norm </param>
		/// <param name="objectsToConnect">Input objects</param>
		/// <param name="nAnchors">Input no. of anchors</param>
		/// <param name="location">Input location</param>
		/// <returns></returns>
		public static CircularAnchorPattern ByCenterPointRadiusNormal(DynGeometry.Point point, double radius, IEnumerable<SteelDbObject> objectsToConnect, int nAnchors, DynGeometry.Vector normal, AssemblyLocation location)
		{
			SteelGeometry.Point3d astPointRef = Utils.ToAstPoint(point, true);

			var norm = Utils.ToAstVector3d(normal, true);
			var vx = norm.GetPerpVector();
			var vy = norm.CrossProduct(vx);

			IEnumerable<string> handles = objectsToConnect.Select(obj => obj.Handle);

			return new CircularAnchorPattern(astPointRef, radius, handles, nAnchors, vx, vy, location);
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var anchorPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;
					if (anchorPattern == null)
					{
						throw new Exception("Null anchor pattern");
					}

					using (var point = Utils.ToDynPoint(anchorPattern.RefPoint, true))
					using (var norm = Utils.ToDynVector(anchorPattern.Normal, true))
					{
						return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalUnits(anchorPattern.Radius, true), norm);
					}
				}
			}
		}
	}
}