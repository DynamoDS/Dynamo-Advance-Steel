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
	public class RectangularAnchorPattern : GraphicObject
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
		internal double GetDiagonalLength(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2)
		{
			return (point2 - point1).GetLength();
		}
		internal double GetRectangleAngle(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2, SteelGeometry.Vector3d vx)
		{
			return (point2 - point1).GetAngleTo(vx);
		}
		internal double GetRectangleLength(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2, SteelGeometry.Vector3d vx)
		{
			var diagLen = GetDiagonalLength(point1, point2);
			var alpha = GetRectangleAngle(point1, point2, vx);

			return diagLen * Math.Cos(alpha);
		}
		internal double GetRectangleHeight(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2, SteelGeometry.Vector3d vx)
		{
			var diagLen = GetDiagonalLength(point1, point2);
			var alpha = GetRectangleAngle(point1, point2, vx);

			return diagLen * Math.Sin(alpha);
		}
		internal void UpdateAnchorPattern(Autodesk.AdvanceSteel.Modelling.AnchorPattern toUpdate, int nx, int ny, Point3d point1, Point3d point2, double dx, double dy)
		{
			toUpdate.Nx = nx;
			toUpdate.Ny = ny;
			toUpdate.Dx = dx;
			toUpdate.Dy = dy;
			toUpdate.RefPoint = point1 + (point2 - point1) * 0.5;
		}
		internal RectangularAnchorPattern(SteelGeometry.Point3d astPoint1, SteelGeometry.Point3d astPoint2, IEnumerable<string> handlesToConnect, SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
																			int nx, int ny, AssemblyLocation assemblyLocation, double dx, double dy)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.AnchorPattern anchors = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					var astPointRef = astPoint1 + (astPoint2 - astPoint1) * 0.5;

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						anchors = new Autodesk.AdvanceSteel.Modelling.AnchorPattern(astPointRef, vx, vy);
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
							throw new System.Exception("Not an anchor pattern");
					}

					UpdateAnchorPattern(anchors, nx, ny, astPoint1, astPoint2, dx, dy);
					HashSet<FilerObject> filerObjects = ObjectsConnection.GetFilerObjects(handlesToConnect);
					anchors.Connect(filerObjects, ObjectsConnection.GetSteelAssemblyLocation(assemblyLocation));

					Handle = anchors.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
				}
			}
		}
		/// <summary>
		/// Create an Advance Steel Rectangular Anchor Pattern By Rectangle
		/// </summary>
		/// <param name="rectangle"> Input rectangle </param>
		/// <param name="objectsToConnect"> Input objects </param>
		/// <param name="nx"> Input nx </param>
		/// <param name="ny"> Input ny </param>
		/// <param name="location"> Input location </param>
		public static RectangularAnchorPattern ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle, IEnumerable<SteelDbObject> objectsToConnect,
																											int nx, int ny, AssemblyLocation location)
		{
			List<string> handlesList = ObjectsConnection.GetSteelDbObjectsToConnect(objectsToConnect);

			var dynCorners = rectangle.Corners();
			var astCorners = Utils.ToAstPoints(dynCorners, true);
			var vx = astCorners[1] - astCorners[0];
			var vy = astCorners[3] - astCorners[0];

			return new RectangularAnchorPattern(astCorners[0], astCorners[2], handlesList, vx, vy, nx, ny, location,
																					Utils.ToInternalUnits(rectangle.Height, true), Utils.ToInternalUnits(rectangle.Width, true));
		}
		public override DynGeometry.Curve GetDynCurve()
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

					var temp1 = anchorPattern.XDirection * anchorPattern.Dx / 2.0;
					var temp2 = anchorPattern.YDirection * anchorPattern.Dy / 2.0;

					var pt1 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt1.Add(temp1 + temp2);
					
					var pt2 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt2.Add(temp1 - temp2);

					var pt3 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt3.Add(-temp1 - temp2);

					var pt4 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt4.Add(-temp1 + temp2);

					{
						List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>();

						polyPoints.Add(Utils.ToDynPoint(pt1, true));
						polyPoints.Add(Utils.ToDynPoint(pt2, true));
						polyPoints.Add(Utils.ToDynPoint(pt3, true));
						polyPoints.Add(Utils.ToDynPoint(pt4, true));

						return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
					}
				}
			}
		}

	}
	/// <summary>
	/// Anchor Orientation Types
	/// </summary>
	public class AnchorOrientationTypes
	{
		public enum OrientationType
		{
			kNormalOrientation = 0,
			kDiagonalInside = 1,
			kDiagonalOutside = 2,
			kAllOutside = 3,
			kAllInside = 4,
			kInsideRotated = 5,
			kOutsideRotated = 6
		}
	}



}

