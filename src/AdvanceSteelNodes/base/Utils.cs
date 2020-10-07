using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public static class Utils
  {
    private static readonly string separator = "#@§@#";

    public static double RadToDegree(double rad)
    {
      return 180.0 * rad / System.Math.PI;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d GetMidPointBetween(this Point3d OriginPoint, Point3d SecondPoint)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d returnPnt = new Autodesk.AdvanceSteel.Geometry.Point3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d v = SecondPoint.Subtract(OriginPoint);
      v.Normalize();
      returnPnt = OriginPoint + ((OriginPoint.DistanceTo(SecondPoint) / 2) * v);
      return returnPnt;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d GetMidPointOnArc(this Point3d startPointOnArc, Point3d endPointOnArc, Point3d arcCentrePoint)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d returnPnt = new Autodesk.AdvanceSteel.Geometry.Point3d();
      double radius = arcCentrePoint.DistanceTo(startPointOnArc);
      Autodesk.AdvanceSteel.Geometry.Point3d biSectPoint = startPointOnArc.GetMidPointBetween(endPointOnArc);
      Autodesk.AdvanceSteel.Geometry.Vector3d v = biSectPoint.Subtract(arcCentrePoint);
      v.Normalize();
      returnPnt = startPointOnArc + (radius * v);
      return returnPnt;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d ToAstPoint(Autodesk.DesignScript.Geometry.Point pt, bool bConvertToAstUnits)
    {
      double factor = 1.0;
      if (bConvertToAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }

      return new Autodesk.AdvanceSteel.Geometry.Point3d(pt.X, pt.Y, pt.Z) * factor;
    }

    static public Double ToInternalUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      return (value * (1 / factor));
    }

    static public Double ToInternalAngleUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAngle.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalAngleUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAngle.Factor;
      }
      return (value * (1 / factor));
    }

    static public Autodesk.DesignScript.Geometry.Point ToDynPoint(Autodesk.AdvanceSteel.Geometry.Point3d pt, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      pt = pt * (1 / factor);
      return Autodesk.DesignScript.Geometry.Point.ByCoordinates(pt.x, pt.y, pt.z);
    }

    static public Autodesk.DesignScript.Geometry.Vector ToDynVector(Autodesk.AdvanceSteel.Geometry.Vector3d vect, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      vect = vect * (1 / factor);
      return Autodesk.DesignScript.Geometry.Vector.ByCoordinates(vect.x, vect.y, vect.z);
    }

    static public Autodesk.DesignScript.Geometry.CoordinateSystem ToDynCoordinateSys(Autodesk.AdvanceSteel.Geometry.Matrix3d matrix, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d origin = new Point3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d xAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d yAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d zAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      matrix.GetCoordSystem(out origin, out xAxis, out yAxis, out zAxis);

      return Autodesk.DesignScript.Geometry.CoordinateSystem.ByOriginVectors(ToDynPoint(origin, bConvertToAstUnits), ToDynVector(xAxis, bConvertToAstUnits), 
        ToDynVector(yAxis, bConvertToAstUnits), ToDynVector(zAxis, bConvertToAstUnits));
    }

    static public Autodesk.AdvanceSteel.Geometry.Vector3d ToAstVector3d(Autodesk.DesignScript.Geometry.Vector v, bool bConvertToAstUnits)
    {
      double factor = 1.0;
      if (bConvertToAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      return new Autodesk.AdvanceSteel.Geometry.Vector3d(v.X, v.Y, v.Z) * factor;
    }

    static public Autodesk.AdvanceSteel.Geometry.Matrix3d ToAstMatrix3d(Autodesk.DesignScript.Geometry.CoordinateSystem cs, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = new Autodesk.AdvanceSteel.Geometry.Matrix3d();
      matrix.SetCoordSystem(ToAstPoint(cs.Origin, bConvertToAstUnits), ToAstVector3d(cs.XAxis, bConvertToAstUnits), ToAstVector3d(cs.YAxis, bConvertToAstUnits), ToAstVector3d(cs.ZAxis, bConvertToAstUnits));
      return matrix;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d[] ToAstPoints(Autodesk.DesignScript.Geometry.Point[] pts, bool bConvertToAstUnits)
    {
      if (pts == null)
        return new Autodesk.AdvanceSteel.Geometry.Point3d[0];

      Autodesk.AdvanceSteel.Geometry.Point3d[] astPts = new Autodesk.AdvanceSteel.Geometry.Point3d[pts.Length];
      for (int nIdx = 0; nIdx < pts.Length; nIdx++)
      {
        astPts[nIdx] = ToAstPoint(pts[nIdx], bConvertToAstUnits);
      }

      return astPts;
    }

    static public Autodesk.DesignScript.Geometry.Point[] ToDynPoints(Autodesk.AdvanceSteel.Geometry.Point3d[] astPts, bool bConvertToAstUnits)
    {
      if (astPts == null)
        return new Autodesk.DesignScript.Geometry.Point[0];

      Autodesk.DesignScript.Geometry.Point[] pts = new Autodesk.DesignScript.Geometry.Point[astPts.Length];
      for (int nIdx = 0; nIdx < pts.Length; nIdx++)
      {
        pts[nIdx] = ToDynPoint(astPts[nIdx], bConvertToAstUnits);
      }

      return pts;
    }

    static public FilerObject GetObject(string handle)
    {
      return FilerObject.GetFilerObjectByHandle(handle);
    }

    internal static void SetOrientation(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Vector3d vOrientation)
    {
      beam.PhysCSStart.GetCoordSystem(out _, out Vector3d vXAxis, out Vector3d vYAxis, out Vector3d vZAxis);
      if (!vXAxis.IsParallelTo(vOrientation))
      {
        Vector3d vProj = vOrientation.OrthoProject(vXAxis);

        double dAngle = vZAxis.GetAngleTo(vProj, vXAxis);

        beam.SetXRotation(dAngle * 180 / Math.PI);
      }
    }

    internal static void AdjustBeamEnd(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Point3d newPtStart)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d beamPtStart = beam.GetPointAtStart();
      Autodesk.AdvanceSteel.Geometry.Point3d beamPtEnd = beam.GetPointAtEnd();

      if (beamPtEnd.IsEqualTo(newPtStart))
      {
        Autodesk.AdvanceSteel.Geometry.Point3d newBeamEnd = beamPtEnd + (beamPtEnd - beamPtStart) * 0.5;
        beam.SetSysEnd(newBeamEnd);
      }
    }

    internal static string Separator
    {
      get { return separator; }
    }

    internal static string[] SplitSectionName(string sectionName)
    {
      string[] result = sectionName.Split(new string[] { Separator }, System.StringSplitOptions.None);

      if (2 == result.Length)
      {
        return result;
      }
      else
      {
        throw new System.Exception("Invalid section name");
      }
    }

    internal static bool CompareCompoundSectionTypes(string first, string second)
    {
      if (first.Equals(second) || (first.Contains("Welded") && second.Contains("Welded")) || (first.Contains("Compound") && second.Contains("Compound")) || (first.Contains("Tapered") && second.Contains("Tapered")))
      {
        return true;
      }
      return false;
    }

    public static double GetDiagonalLength(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2)
    {
      return (point2 - point1).GetLength();
    }
    public static double GetRectangleAngle(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      return (point2 - point1).GetAngleTo(vx);
    }
    public static double GetRectangleLength(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      var diagLen = GetDiagonalLength(point1, point2);
      var alpha = GetRectangleAngle(point1, point2, vx);

      return diagLen * Math.Cos(alpha);
    }
    public static double GetRectangleHeight(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      var diagLen = GetDiagonalLength(point1, point2);
      var alpha = GetRectangleAngle(point1, point2, vx);

      return diagLen * Math.Sin(alpha);
    }

    public static FilerObject[] GetSteelObjectsToConnect(IEnumerable<string> handlesToConnect)
    {
      var ret = new List<FilerObject>();
      bool Objs = false;
      foreach (var objHandle in handlesToConnect)
      {
        FilerObject obj = Utils.GetObject(objHandle);
        if (obj != null && (obj.IsKindOf(FilerObject.eObjectType.kBeam) ||
                            obj.IsKindOf(FilerObject.eObjectType.kBentBeam) ||
                            obj.IsKindOf(FilerObject.eObjectType.kPlate)))
        {
          Objs = true;
          ret.Add(obj);
        }
        else
        {
          throw new System.Exception("Object is empty");
        }
      }
      return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
    }

    public static FilerObject[] GetFilerObjects(IEnumerable<string> handles)
    {
      var ret = new List<FilerObject>();
      bool Objs = false;
      foreach (var objHandle in handles)
      {
        Objs = true;
        ret.Add(Utils.GetObject(objHandle));
      }

      return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
    }

    public static List<string> GetSteelDbObjectsToConnect(IEnumerable<SteelDbObject> objectsToConnect)
    {
      List<string> handlesList = new List<string>();
      foreach (var obj in objectsToConnect)
      {
        if (obj is AdvanceSteel.Nodes.Beams.BentBeam ||
            obj is AdvanceSteel.Nodes.Beams.StraightBeam ||
            obj is AdvanceSteel.Nodes.Plates.Plate)
        {
          handlesList.Add(obj.Handle);
        }
        else
        {
          throw new Exception("Only beams and plates can be connected");
        }
      }
      return handlesList;
    }

    public static Dictionary<string, Property> GetBoltProperties()
    {
      return BuildBoltPropertyList();
    }

    public static Dictionary<string, Property> GetAnchorBoltPropertyList()
    {
      return BuildAnchorBoltPropertyList();
    }

    public static Dictionary<string, Property> GetShearStudPropertyList()
    {
      return BuildShearStudPropertyList();
    }

    public static Property GetProperty(string keyValue)
    {
      Dictionary<string, Property> searchData = BuildBoltPropertyList().Union(
                                                  BuildAnchorBoltPropertyList()).Union(
                                                  BuildShearStudPropertyList()).ToDictionary(s => s.Key, s => s.Value);
      Property retValue = null;
      searchData.TryGetValue(keyValue, out retValue);
      return retValue;
    }

    public static Property GetAnchorBoltProperty(string keyValue)
    {
      Dictionary<string, Property> lstData = BuildAnchorBoltPropertyList();
      Property retValue = null;
      lstData.TryGetValue(keyValue, out retValue);
      return retValue;
    }

    public static Property GetShearStudProperty(string keyValue)
    {
      Dictionary<string, Property> lstData = BuildShearStudPropertyList();
      Property retValue = null;
      lstData.TryGetValue(keyValue, out retValue);
      return retValue;
    }

    private static Dictionary<string, Property> BuildBoltPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Bolt Property...", new Property("none", typeof(string)));
      dictProps.Add("Bolt Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Bolt Assembly", new Property("BoltAssembly", typeof(string)));
      dictProps.Add("Bolt Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Bolt Diameter", new Property("ScrewDiameter", typeof(double)));
      dictProps.Add("Bolt Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Bolts Circle", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("X Bolt Count", new Property("Nx", typeof(int)));
      dictProps.Add("Y Bolt Count", new Property("Ny", typeof(int)));
      dictProps.Add("Bolt X Spacing", new Property("Dx", typeof(double)));
      dictProps.Add("Bolt Y Spacing", new Property("Dy", typeof(double)));
      dictProps.Add("Bolt Pattern Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Bolt Length Addition", new Property("BindingLengthAddition", typeof(double)));
      dictProps.Add("Bolt Inverted", new Property("IsInverted", typeof(bool)));

      return dictProps;
    }

    private static Dictionary<string, Property> BuildAnchorBoltPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Anchor Property...", new Property("none", typeof(string)));
      dictProps.Add("Anchor Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Anchor Assembly", new Property("BoltAssembly", typeof(string)));
      dictProps.Add("Anchor Grade", new Property("Grade", typeof(string))); 
      dictProps.Add("Anchor Length", new Property("ScrewLength", typeof(double)));
      dictProps.Add("Anchor Diameter", new Property("ScrewDiameter", typeof(double)));
      dictProps.Add("Anchor Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Anchor Circle", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("X Anchor Count", new Property("Nx", typeof(int)));
      dictProps.Add("Y Anchor Count", new Property("Ny", typeof(int)));
      dictProps.Add("Anchor X Spacing", new Property("Dx", typeof(double)));
      dictProps.Add("Anchor Y Spacing", new Property("Dy", typeof(double)));
      dictProps.Add("Anchor Pattern Radius", new Property("Radius", typeof(double))); 
      //dictProps.Add("Anchor Orientation", new ASProperty("OrientationType", typeof(int))); 
      dictProps.Add("Anchor Inverted", new Property("IsInverted", typeof(bool)));

      return dictProps;
    }

    private static Dictionary<string, Property> BuildShearStudPropertyList()
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Stud Property...", new Property("none", typeof(string)));
      dictProps.Add("Stud Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Stud Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Stud Length", new Property("Length", typeof(double)));
      dictProps.Add("Stud Diameter", new Property("Diameter", typeof(double)));
      dictProps.Add("Stud Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Shear Studs Circle", new Property("NumberOfElements", typeof(int), "Arranger"));
      dictProps.Add("Shear Stud Radius", new Property("Radius", typeof(double), "Arranger")); 
      dictProps.Add("X Stud Count", new Property("Nx", typeof(int), "Arranger"));
      dictProps.Add("Y Stud Count", new Property("Ny", typeof(int), "Arranger"));
      dictProps.Add("Stud X Spacing", new Property("Dx", typeof(double), "Arranger"));
      dictProps.Add("Stud Y Spacing", new Property("Dy", typeof(double), "Arranger"));
      dictProps.Add("Display Stud As Solid", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      return dictProps;
    }

    public static void CheckListUpdateOrAddValue(List<Property> listOfDataboltData, string propName, object propValue, string propLevel = "")
    {
      var foundItem = listOfDataboltData.FirstOrDefault<Property>(props => props.PropName == propName);
      if (foundItem != null)
      {
        foundItem.PropValue = propValue;
      }
      else
      {
        listOfDataboltData.Add(new Property(propName, propValue, propValue.GetType(), propLevel));
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
    
    public static void SetParameters(Autodesk.AdvanceSteel.Arrangement.Arranger objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.ConstructionTypes.AtomicElement objToMod, List<Property> properties)
    {
      if (properties != null) 
      { 
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.AnchorPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
  }
}