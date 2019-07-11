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
	public class Gratings : GraphicObject
	{
		internal Gratings(string strClass, string strName, Plane plane, Point3d ptCenter, double dLength, double dWidth, double angle)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					Autodesk.AdvanceSteel.Modelling.Grating gratings = null;

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						gratings = new Autodesk.AdvanceSteel.Modelling.Grating(strClass, strName, plane, ptCenter, dLength, dWidth);
						gratings.RotateDirectionBy(angle);
						gratings.WriteToDb();
					}
					else
					{
						gratings = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Grating;
						if (gratings != null && gratings.IsKindOf(FilerObject.eObjectType.kGrating))
						{
							gratings.SetLength(dLength, true);
							gratings.SetWidth(dWidth, true);
							gratings.GratingClass = strClass;
							gratings.ConnectorName = strName;
							gratings.RotateDirectionBy(angle);
						}
						else
						{
							throw new System.Exception("Not a grating pattern");
						}
					}
					Handle = gratings.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel grating
		/// </summary>

		/// <returns></returns>
		public static Gratings ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle, double angle)
		{
			string gratingClass = "McNichols Variable GAA (19P4)";
			string gratingName = "McNichols GAA (19P4) 1-1/3x3/16";

			var dynCorners = rectangle.Corners();
			var astCorners = Utils.ToAstPoints(dynCorners, true);
			var refPoint = astCorners[0] + (astCorners[2] - astCorners[0]) * 0.5;

			Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(refPoint, Utils.ToAstVector3d(rectangle.Normal, true));
			return new Gratings(gratingClass, gratingName, plane, refPoint, Utils.ToInternalUnits(rectangle.Height, true), Utils.ToInternalUnits(rectangle.Width, true), Utils.ToInternalUnits(angle, true));
		}
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var grating = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Grating;

					if (grating == null)
					{
						throw new Exception("Null grating");
					}

					var coordSystem = grating.CS;
					// Vx and Vy direction
					var tempVx = new Vector3d(coordSystem.Values[0][0], coordSystem.Values[1][0], coordSystem.Values[2][0]);
					var tempVy = new Vector3d(coordSystem.Values[0][1], coordSystem.Values[1][1], coordSystem.Values[2][1]);

					tempVx.Normalize();
					tempVy.Normalize();
					var temp1 = tempVx * grating.Width / 2.0;
					var temp2 = tempVy * grating.Length / 2.0;

					var pt1 = new SteelGeometry.Point3d(grating.CenterPoint);
					pt1.Add(temp1 + temp2);

					var pt2 = new SteelGeometry.Point3d(grating.CenterPoint);
					pt2.Add(temp1 - temp2);

					var pt3 = new SteelGeometry.Point3d(grating.CenterPoint);
					pt3.Add(-temp1 - temp2);

					var pt4 = new SteelGeometry.Point3d(grating.CenterPoint);
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
}

