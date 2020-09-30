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

namespace AdvanceSteel.Nodes.ConnectionObjects.ShearStuds
{
	/// <summary>
	/// Advance Steel Circular Shear Stud Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class CircularShearStudsPattern : GraphicObject
	{

		internal CircularShearStudsPattern(string handleToConnect, SteelGeometry.Matrix3d coordSyst, PropertiesShearStuds shearStudData)
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
						shearStuds.Arranger = new Autodesk.AdvanceSteel.Arrangement.CircleArranger(Matrix2d.kIdentity, Utils.ToInternalUnits(shearStudData.Radius, true), shearStudData.XCount);
            shearStuds.Standard = shearStudData.Standard;
            shearStuds.Grade = shearStudData.Grade;
            shearStuds.Diameter = Utils.ToInternalUnits(shearStudData.Diameter, true);
            shearStuds.Length = Utils.ToInternalUnits(shearStudData.Length, true);
            shearStuds.WriteToDb();
            shearStuds.ReprMode = (shearStudData.DisplayAsSolid ? 2 : 1);
          }
					else
					{
						shearStuds = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Connector;
						if (shearStuds != null && shearStuds.IsKindOf(FilerObject.eObjectType.kConnector))
						{
							shearStuds.Arranger.Radius = Utils.ToInternalUnits(shearStudData.Radius, true);
              shearStuds.Arranger.NumberOfElements = shearStudData.XCount;
              shearStuds.Standard = shearStudData.Standard;
              shearStuds.Grade = shearStudData.Grade;
              shearStuds.Diameter = Utils.ToInternalUnits(shearStudData.Diameter, true);
              shearStuds.Length = Utils.ToInternalUnits(shearStudData.Length, true);
              shearStuds.ReprMode = (shearStudData.DisplayAsSolid ? 2 : 1);
            }
						else
							throw new System.Exception("Not a shear stud pattern");
					}
					FilerObject obj = Utils.GetObject(handleToConnect);
          Autodesk.AdvanceSteel.Modelling.WeldPoint weld = shearStuds.Connect(obj, coordSyst);
          weld.AssemblyLocation = (AtomicElement.eAssemblyLocation)shearStudData.ShearStudConnectionType;

          Handle = shearStuds.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(shearStuds);
				}
			}
		}

    /// <summary>
    /// Create an Advance Steel Shear Stud Pattern By Circle
    /// </summary>
    /// <param name="circle"> Input Dynamo Circle </param>
    /// <param name="referenceVector"> Input Dynamo Vector for alignment of circle</param>
    /// <param name="objectToConnect"> Input objects </param>
    /// <param name="shearStudData"> Input ShearStud Build Properties </param>
    /// <returns></returns>
    public static CircularShearStudsPattern ByCircle(Autodesk.DesignScript.Geometry.Circle circle,
                                                      Autodesk.DesignScript.Geometry.Vector referenceVector,
                                                      SteelDbObject objectToConnect,
                                                      PropertiesShearStuds shearStudData)
		{
			var norm = Utils.ToAstVector3d(circle.Normal, true);
      var vx = Utils.ToAstVector3d(referenceVector, true);
			var vy = norm.CrossProduct(vx);
			var vz = norm;

			vx = vx.Normalize();
			vy = vy.Normalize();
			vz = vz.Normalize();

      shearStudData.Radius = circle.Radius;

      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
			List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

			Matrix3d matrix3D = new Matrix3d();
			matrix3D.SetCoordSystem(Utils.ToAstPoint(circle.CenterPoint, true), vx, vy, vz);
	
			return new CircularShearStudsPattern(handlesList[0], matrix3D, shearStudData);
		}

    /// <summary>
    /// Create an Advance Steel Circular Shear Stud Pattern at a Point
    /// </summary>
    /// <param name="connectionPoint"> Input Insertion point of Bolt Pattern </param>
    /// <param name="shearStudCS"> Input Coordinate System </param>
    /// <param name="objectToConnect"> Object to attached ShearStud </param>
    /// <param name="shearStudData"> Input ShearStud Build Properties </param>
    /// <returns></returns>
    public static CircularShearStudsPattern AtCentrePoint(Autodesk.DesignScript.Geometry.Point connectionPoint,
                                                        Autodesk.DesignScript.Geometry.CoordinateSystem shearStudCS,
                                                        SteelDbObject objectToConnect,
                                                        PropertiesShearStuds shearStudData)
    {

      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

      var vx = Utils.ToAstVector3d(shearStudCS.XAxis, true);
      var vy = Utils.ToAstVector3d(shearStudCS.YAxis, true);
      var vz = Utils.ToAstVector3d(shearStudCS.ZAxis, true);

      Matrix3d matrix3D = new Matrix3d();
      matrix3D.SetCoordSystem(Utils.ToAstPoint(connectionPoint, true), vx, vy, vz);

      return new CircularShearStudsPattern(handlesList[0], matrix3D, shearStudData);
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
