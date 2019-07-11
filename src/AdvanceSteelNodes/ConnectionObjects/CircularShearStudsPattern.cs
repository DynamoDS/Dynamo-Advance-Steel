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
		internal void UpdateShearStudPattern(ref Autodesk.AdvanceSteel.Modelling.Connector toUpdate, int noElements, double diameter, double length)
		{
			toUpdate.Diameter = diameter;
			toUpdate.Length = length;
			toUpdate.Arranger.NumberOfElements = noElements;
		}
		internal CircularShearStudsPattern(double diameter, string handleToConnect, SteelGeometry.Matrix3d coordSyst, int noElements, double radius, double length)
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
						shearStuds.Arranger = new Autodesk.AdvanceSteel.Arrangement.CircleArranger(Utils.ToInternalUnits(radius, true));
						UpdateShearStudPattern(ref shearStuds, noElements, diameter, length);
						FilerObject obj = Utils.GetObject(handleToConnect);
						Matrix3d matrixCS = new Matrix3d(coordSyst);
						shearStuds.Connect(obj, matrixCS);
						shearStuds.WriteToDb();
					}
					else
					{
						shearStuds = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Connector;
						if (shearStuds != null && shearStuds.IsKindOf(FilerObject.eObjectType.kConnector))
						{
							
							UpdateShearStudPattern(ref shearStuds, noElements, diameter, length);
							shearStuds.Arranger.Radius = Utils.ToInternalUnits(radius, true);
							FilerObject obj = Utils.GetObject(handleToConnect);
							Matrix3d matrixCS = new Matrix3d(coordSyst);
							shearStuds.Connect(obj, matrixCS);
						}
						else
							throw new System.Exception("Not a shear stud pattern");
					}
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
		/// <param name="diameter">Input diameter</param>
		/// <param name="noElements">Input number of elements</param>
		/// <param name="length">Input length</param>
		/// <returns></returns>
		public static CircularShearStudsPattern ByCircle(Autodesk.DesignScript.Geometry.Circle circle, SteelDbObject objectToConnect, double diameter, int noElements, double length)
		{
			var norm = Utils.ToAstVector3d(circle.Normal, true);
			var vx = norm.GetPerpVector();
			var vy = norm.CrossProduct(vx);
			var vz = norm;

			vx = vx.Normalize();
			vy = vy.Normalize();
			vz = vz.Normalize();

			List<string> handlesList = new List<string>();
			List<SteelDbObject> tempList = new List<SteelDbObject>();
			tempList.Add(objectToConnect);
			handlesList = ObjectsConnection.GetSteelDbObjectsToConnect(tempList);

			Matrix3d matrix3D = new Matrix3d();
			matrix3D.SetCoordSystem(Utils.ToAstPoint(circle.CenterPoint, true), vx, vy, vz);


			return new CircularShearStudsPattern(Utils.ToInternalUnits(diameter, true), handlesList[0], matrix3D, noElements, circle.Radius, Utils.ToInternalUnits(length, true));
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
					{
						return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadius(point, Utils.FromInternalUnits(shearStud.Arranger.Radius, true));
					}
				}
			}
		}

	}
}
