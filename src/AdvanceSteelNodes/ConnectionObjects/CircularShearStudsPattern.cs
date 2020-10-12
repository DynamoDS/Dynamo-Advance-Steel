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

		internal CircularShearStudsPattern(string handleToConnect, 
                                        SteelGeometry.Matrix3d coordSyst,
                                        List<ASProperty> shearStudData,
                                        int boltCon)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{

          List<ASProperty> defaultShearStudData = shearStudData.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> arrangerShearStudData = shearStudData.Where(x => x.PropLevel == "Arranger").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = shearStudData.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          Autodesk.AdvanceSteel.Modelling.Connector shearStuds = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
            var temp_radius = (double)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "Radius").PropValue;
            var temp_noss = (int)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "NumberOfElements").PropValue;

            shearStuds = new Autodesk.AdvanceSteel.Modelling.Connector();
						shearStuds.Arranger = new Autodesk.AdvanceSteel.Arrangement.CircleArranger(Matrix2d.kIdentity, temp_radius, temp_noss);

            if (defaultShearStudData != null)
            {
              Utils.SetParameters(shearStuds, defaultShearStudData);
            }
            
            Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);

            shearStuds.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(shearStuds, postWriteDBData);
            }

          }
					else
					{
						shearStuds = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Connector;
						if (shearStuds != null && shearStuds.IsKindOf(FilerObject.eObjectType.kConnector))
						{

              if (defaultShearStudData != null)
              {
                Utils.SetParameters(shearStuds, defaultShearStudData);
              }

              Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(shearStuds, postWriteDBData);
              }

            }
						else
							throw new System.Exception("Not a shear stud pattern");
					}
					FilerObject obj = Utils.GetObject(handleToConnect);
          Autodesk.AdvanceSteel.Modelling.WeldPoint weld = shearStuds.Connect(obj, coordSyst);
          weld.AssemblyLocation = (AtomicElement.eAssemblyLocation)boltCon;

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
    /// <param name="objectToConnect"> Object to attached ShearStud </param>
    /// <param name="studLength"> Input Shear Stud Length</param>
    /// <param name="studDiameter"> Input Shear Stud Diameter</param>
    /// <param name="noOfShearStudsInCircle"> Input Number of Shear Stud to be placed in the Circle Pattern</param>
    /// <param name="shearStudConnectionType"> Input Shear Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalShearStudParameters"> Optional Input ShearStud Build Properties </param>
    /// <returns></returns>
    public static CircularShearStudsPattern ByCircle(Autodesk.DesignScript.Geometry.Circle circle,
                                                      Autodesk.DesignScript.Geometry.Vector referenceVector,
                                                      SteelDbObject objectToConnect,
                                                      double studLength,
                                                      double studDiameter,
                                                      [DefaultArgument("9;")]int noOfShearStudsInCircle,
                                                      [DefaultArgument("2;")]int shearStudConnectionType,
                                                      [DefaultArgument("null")]List<ASProperty> additionalShearStudParameters)
		{
      var norm = Utils.ToAstVector3d(circle.Normal, true);
      var vx = Utils.ToAstVector3d(referenceVector, true);
			var vy = norm.CrossProduct(vx);
			var vz = norm;

			vx = vx.Normalize();
			vy = vy.Normalize();
			vz = vz.Normalize();

      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
			List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

			Matrix3d matrix3D = new Matrix3d();
			matrix3D.SetCoordSystem(Utils.ToAstPoint(circle.CenterPoint, true), vx, vy, vz);

      PreSetValuesInListProps(additionalShearStudParameters, noOfShearStudsInCircle, Utils.ToInternalUnits(circle.Radius, true), Utils.ToInternalUnits(studLength, true), Utils.ToInternalUnits(studDiameter, true));

      return new CircularShearStudsPattern(handlesList[0], matrix3D, additionalShearStudParameters, shearStudConnectionType);
		}

    /// <summary>
    /// Create an Advance Steel Circular Shear Stud Pattern at a Point
    /// </summary>
    /// <param name="connectionPoint"> Input Insertion point of Bolt Pattern </param>
    /// <param name="shearStudCS"> Input Coordinate System </param>
    /// <param name="patternRadius"> Input Shear Stud pattern Radius</param>
    /// <param name="objectToConnect"> Object to attached ShearStud </param>
    /// <param name="studLength"> Input Shear Stud Length</param>
    /// <param name="studDiameter"> Input Shear Stud Diameter</param>
    /// <param name="noOfShearStudsInCircle"> Input Number of Shear Stud to be placed in the Circle Pattern</param>
    /// <param name="shearStudConnectionType"> Input Shear Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalShearStudParameters"> Optional Input ShearStud Build Properties </param>
    /// <returns></returns>
    public static CircularShearStudsPattern AtCentrePoint(Autodesk.DesignScript.Geometry.Point connectionPoint,
                                                        Autodesk.DesignScript.Geometry.CoordinateSystem shearStudCS,
                                                        double patternRadius,
                                                        SteelDbObject objectToConnect,
                                                        double studLength,
                                                        double studDiameter,
                                                        [DefaultArgument("9;")]int noOfShearStudsInCircle,
                                                        [DefaultArgument("2;")]int shearStudConnectionType,
                                                        [DefaultArgument("null")]List<ASProperty> additionalShearStudParameters)
    {
      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

      var vx = Utils.ToAstVector3d(shearStudCS.XAxis, true);
      var vy = Utils.ToAstVector3d(shearStudCS.YAxis, true);
      var vz = Utils.ToAstVector3d(shearStudCS.ZAxis, true);

      Matrix3d matrix3D = new Matrix3d();
      matrix3D.SetCoordSystem(Utils.ToAstPoint(connectionPoint, true), vx, vy, vz);

      PreSetValuesInListProps(additionalShearStudParameters, noOfShearStudsInCircle, Utils.ToInternalUnits(patternRadius, true), Utils.ToInternalUnits(studLength, true), Utils.ToInternalUnits(studDiameter, true));

      return new CircularShearStudsPattern(handlesList[0], matrix3D, additionalShearStudParameters, shearStudConnectionType);
    }

    private static void PreSetValuesInListProps(List<ASProperty> listOfBoltParameters, int noss, double radius, double studLength, double studDiameter)
    {
      if (listOfBoltParameters == null)
      {
        listOfBoltParameters = new List<ASProperty>() { };
      }

      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "NumberOfElements", noss, "Arranger");
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Radius", radius, "Arranger");
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Length", studLength, ".");
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Diameter", studDiameter, ".");
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
