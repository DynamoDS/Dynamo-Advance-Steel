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

namespace AdvanceSteel.Nodes.Gratings
{
	/// <summary>
	/// Advance Steel Variable Grating Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class VariableGrating : GraphicObject
	{

		internal VariableGrating(string strClass, string strName, Point3d ptCenter, double dWidth, double dLength, Vector3d vNormal)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(ptCenter, vNormal);
					Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						gratings = new Autodesk.AdvanceSteel.Modelling.Grating(strClass, strName, plane, ptCenter, dWidth, dLength);
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
							gratings.SetLength(dWidth, true);
							gratings.SetWidth(dLength, true);
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
		/// Create an Advance Steel Variable Grating
		/// </summary>
		/// <returns></returns>
		public static VariableGrating ByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, string strClass, string strName, double dWidth, double dLength)
		{
	  	return new VariableGrating(strClass, strName, Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToInternalUnits(dWidth, true), Utils.ToInternalUnits(dLength, true),
																 Utils.ToAstVector3d(coordinateSystem.ZAxis, true));
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

					List<DynGeometry.Point> polyPoints = GratingDraw.GetPointsToDraw(grating);

					return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
				}
			}
		}
	}
}

