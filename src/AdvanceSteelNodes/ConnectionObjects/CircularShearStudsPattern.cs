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
	public class CircularShearStudsPattern : GraphicObject
	{
		internal void UpdateShearStudPattern(Autodesk.AdvanceSteel.Modelling.Connector toUpdate, int noElements, double diameter, double length)
		{
			toUpdate.Diameter = diameter;
			toUpdate.Length = length;
			toUpdate.Arranger.NumberOfElements = noElements;
		}
		internal CircularShearStudsPattern(double studDiameter, string handleToConnect, SteelGeometry.Matrix3d coordSyst, int noElements, double patternRadius, double studLen)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.Connector shearStuds = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						shearStuds = new Autodesk.AdvanceSteel.Modelling.Connector();
						shearStuds.Arranger = new Autodesk.AdvanceSteel.Arrangement.CircleArranger(Utils.ToInternalUnits(patternRadius, true));
						shearStuds.WriteToDb();
					}
					else
					{
						shearStuds = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Connector;
						if (shearStuds != null && shearStuds.IsKindOf(FilerObject.eObjectType.kConnector))
						{
							shearStuds.Arranger.Radius = Utils.ToInternalUnits(patternRadius, true);
							
						}
						else
							throw new System.Exception("Not a shear stud pattern");
					}
					UpdateShearStudPattern(shearStuds, noElements, studDiameter, studLen);
					FilerObject obj = Utils.GetObject(handleToConnect);
					shearStuds.Connect(obj, coordSyst);

					Handle = shearStuds.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(shearStuds);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel Shear Stud Pattern By Circle
		/// </summary>
		/// <param name="circle">Input circle</param>
		/// <param name="objectToConnect">Input objects</param>
		/// <param name="studDiameter">Input diameter</param>
		/// <param name="noElements">Input number of elements</param>
		/// <param name="studLen">Input length</param>
		/// <returns></returns>
		public static CircularShearStudsPattern ByCircle(Autodesk.DesignScript.Geometry.Circle circle, SteelDbObject objectToConnect, double studDiameter, int noElements, double studLen)
		{
			var norm = Utils.ToAstVector3d(circle.Normal, true);
			var vx = norm.GetPerpVector();
			var vy = norm.CrossProduct(vx);
			var vz = norm;

			vx = vx.Normalize();
			vy = vy.Normalize();
			vz = vz.Normalize();

			List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
			List<string> handlesList = ObjectsConnection.GetSteelDbObjectsToConnect(tempList);

			Matrix3d matrix3D = new Matrix3d();
			matrix3D.SetCoordSystem(Utils.ToAstPoint(circle.CenterPoint, true), vx, vy, vz);
	
			return new CircularShearStudsPattern(Utils.ToInternalUnits(studDiameter, true), handlesList[0], matrix3D, noElements, circle.Radius, Utils.ToInternalUnits(studLen, true));
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock(access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var shearStud = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Connector;
					if(shearStud == null)
					{
						throw new Exception("Null shear stud pattern");
					}
					using (var point = Utils.ToDynPoint(shearStud.CenterPoint, true))
					using (var norm = Utils.ToDynVector(shearStud.Normal, true))
					{
						return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalUnits(shearStud.Arranger.Radius, true),norm);
					}
				}
			}
		}

	}
}
