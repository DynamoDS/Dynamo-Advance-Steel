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
	/// Advance Steel Rectangular Bolt Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class RectangularBoltPattern : GraphicObject
	{
		public AssemblyLocation AssemblyLocation
		{
			get
			{
				lock (access_obj)
				{
					using (var ctx = new SteelServices.DocContext())
					{
						var bolts = Utils.GetObject(this.Handle) as Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern;
						return ObjectsConnection.GetDynAssemblyLocation(bolts.AssemblyLocation);
					}
				}
			}
		}

		internal void UpdateBoltPattern(Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern toUpdate, int nx, int ny, Point3d point1, Point3d point2)
		{
			toUpdate.Nx = nx;
			toUpdate.Ny = ny;
			toUpdate.RefPoint = point1 + (point2 - point1) * 0.5;
			toUpdate.Length = ObjectsConnection.GetRectangleLength(point1, point2, toUpdate.XDirection);
			toUpdate.Height = ObjectsConnection.GetRectangleHeight(point1, point2, toUpdate.XDirection);
		}
		internal RectangularBoltPattern(SteelGeometry.Point3d astPoint1, SteelGeometry.Point3d astPoint2, IEnumerable<string> handlesToConnect, SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
																		int nx, int ny, AssemblyLocation assemblyLocation)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern bolts = null;

					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						bolts = new Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern(astPoint1, astPoint2, vx, vy);
						bolts.WriteToDb();
					}
					else
					{
						bolts = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern;

						if (bolts != null && bolts.IsKindOf(FilerObject.eObjectType.kFinitRectScrewBoltPattern))
						{
							bolts.XDirection = vx;
							bolts.YDirection = vy;
						}
						else
							throw new System.Exception("Not a rectangular pattern");
					}

					UpdateBoltPattern(bolts, nx, ny, astPoint1, astPoint2);
					HashSet<FilerObject> filerObjects = ObjectsConnection.GetSteelObjectsToConnect(handlesToConnect);
					bolts.Connect(filerObjects, ObjectsConnection.GetSteelAssemblyLocation(assemblyLocation));

					Handle = bolts.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(bolts);
				}
			}
		}	
		/// <summary>
		/// Create an Advance Steel Rectangular Pattern By Rectangle
		/// </summary>
		/// <param name="rectangle"> Input rectangle </param>
		/// <param name="objectsToConnect"> Input objects </param>
		/// <param name="nx"> Input nx </param>
		/// <param name="ny"> Input ny </param>
		/// <param name="location"> Input location </param>
		public static RectangularBoltPattern ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle, IEnumerable<SteelDbObject> objectsToConnect, int nx, int ny, AssemblyLocation location)
		{
			var norm = Utils.ToAstVector3d(rectangle.Normal, true);

			List<string> handlesList = new List<string>();
			handlesList = ObjectsConnection.GetSteelDbObjectsToConnect(objectsToConnect);

			var dynCorners = rectangle.Corners();
			var astCorners = Utils.ToAstPoints(dynCorners, true);
			var vx = astCorners[1] - astCorners[0];
			var vy = astCorners[3] - astCorners[0];

			return new RectangularBoltPattern(astCorners[0], astCorners[2], handlesList, vx, vy, nx, ny, location);
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var boltPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern;

					if (boltPattern == null)
					{
						throw new Exception("Null bolt pattern");
					}

					var temp1 = boltPattern.XDirection * boltPattern.Length / 2.0;
					var temp2 = boltPattern.YDirection * boltPattern.Height / 2.0;
					
					var pt1 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
					pt1.Add(temp1 + temp2);

					var pt2 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
					pt2.Add(temp1 - temp2);

					var pt3 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
					pt3.Add(-temp1 - temp2);

					var pt4 = new SteelGeometry.Point3d(boltPattern.CenterPoint);
					pt4.Add(-temp1 + temp2);

					{
						List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>
						{
							Utils.ToDynPoint(pt1, true),
							Utils.ToDynPoint(pt2, true),
							Utils.ToDynPoint(pt3, true),
							Utils.ToDynPoint(pt4, true)
						};

						return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
					}
				}
			}
		}
	}
}
