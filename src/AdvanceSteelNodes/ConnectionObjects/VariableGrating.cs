using System;
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

namespace AdvanceSteel.Nodes.ConnectionObjects
{
	/// <summary>
	/// Advance Steel Variable Grating Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class VariableGrating : GraphicObject
	{

		internal VariableGrating(string strClass, string strName, Plane plane, Point3d ptCenter, double dWidth, double dLength, SteelGeometry.Point3d point1, SteelGeometry.Point3d point2, Vector3d vx, Vector3d vy)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{

					Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						gratings = new Autodesk.AdvanceSteel.Modelling.Grating(strClass, strName, plane, ptCenter, dLength, dWidth);
						gratings.WriteToDb();
					}
					else
					{
						gratings = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Grating;
						if (gratings != null && gratings.IsKindOf(FilerObject.eObjectType.kGrating))
						{
							gratings.GratingClass = strClass;
							gratings.GratingSize = strName;
							gratings.DefinitionPlane = plane;
							gratings.SetLength(ObjectsConnection.GetRectangleLength(point1, point2, vx), true);
							gratings.SetWidth(ObjectsConnection.GetRectangleHeight(point1, point2, vx), true);
						}
						else
						{
							throw new System.Exception("Not a Variable Grating pattern");
						}
					}

					Handle = gratings.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel Variable grating
		/// </summary>

		/// <returns></returns>
		public static VariableGrating ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle, string strClass, string strName)
		{
			var dynCorners = rectangle.Corners();
			var astCorners = Utils.ToAstPoints(dynCorners, true);
			var refPoint = astCorners[0] + (astCorners[2] - astCorners[0]) * 0.5;
			var vx = astCorners[1] - astCorners[0];
			var vy = astCorners[3] - astCorners[0];

			Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(refPoint, vx, vy);
			return new VariableGrating(strClass, strName, plane, refPoint, Utils.ToInternalUnits(rectangle.Width, true), Utils.ToInternalUnits(rectangle.Height, true), astCorners[0], astCorners[2], vx, vy);
		}
		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var grating = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Grating;

					if (grating == null)
					{
						throw new Exception("Null Variable Grating pattern");
					}

					var coordSystem = grating.CS;
					var tempVx = new Vector3d(coordSystem.Values[0][0], coordSystem.Values[1][0], coordSystem.Values[2][0]);
					var tempVy = new Vector3d(coordSystem.Values[0][1], coordSystem.Values[1][1], coordSystem.Values[2][1]);

					var temp1 = tempVx * grating.Length / 2.0;
					var temp2 = tempVy * grating.Width / 2.0;

					var pt1 = new SteelGeometry.Point3d(grating.CenterPoint);
					pt1.Add(temp1 + temp2);

					var pt2 = new SteelGeometry.Point3d(grating.CenterPoint);
					pt2.Add(temp1 - temp2);

					var pt3 = new SteelGeometry.Point3d(grating.CenterPoint);
					pt3.Add(-temp1 - temp2);

					var pt4 = new SteelGeometry.Point3d(grating.CenterPoint);
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

