using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using System;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.GratingFunctions
{
  public class Grating
  {
    internal Grating()
    {
    }

    /// <summary>
    /// Get Bar Grating Product Name
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns></returns>
    public static string GetBarGratingProductName(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      string ret = "";
      //lock the document and start transaction
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kGrating))
        {
          Autodesk.AdvanceSteel.Modelling.Grating grating = obj as Autodesk.AdvanceSteel.Modelling.Grating;
          ret = grating.GetBarGratingProductName();
        }
        else
          throw new System.Exception("Not a Grating Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Grating Centre Point on Top
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Point GetCenterOnTop(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      Autodesk.DesignScript.Geometry.Point ret;
      //lock the document and start transaction
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kGrating))
        {
          Autodesk.AdvanceSteel.Modelling.Grating grating = obj as Autodesk.AdvanceSteel.Modelling.Grating;
          Autodesk.AdvanceSteel.Geometry.Point3d point = grating.GetCenterOnTop();
          ret = Utils.ToDynPoint(point, true);
        }
        else
          throw new System.Exception("Not a Grating Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Bar Grating Top Normal Vector
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Vector GetTopNormal(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      Autodesk.DesignScript.Geometry.Vector ret;
      //lock the document and start transaction
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kGrating))
        {
          Autodesk.AdvanceSteel.Modelling.Grating grating = obj as Autodesk.AdvanceSteel.Modelling.Grating;
          Autodesk.AdvanceSteel.Geometry.Vector3d vec = grating.GetTopNormal();
          ret = Utils.ToDynVector(vec, true);
        }
        else
          throw new System.Exception("Not a Grating Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plane on top of Grating Object
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns></returns>
    public static Autodesk.DesignScript.Geometry.Plane GetTopPlane(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      Autodesk.DesignScript.Geometry.Plane ret;
      //lock the document and start transaction
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kGrating))
        {
          Autodesk.AdvanceSteel.Modelling.Grating grating = obj as Autodesk.AdvanceSteel.Modelling.Grating;
          Autodesk.AdvanceSteel.Geometry.Plane plane = grating.GetTopPlane();
          ret = Utils.ToDynPlane(plane, true);
        }
        else
          throw new System.Exception("Not a Grating Object");
      }
      return ret;
    }

    /// <summary>
    /// Get the Grating type. 0 - Standard, 1 - Variable, 2 - Bar Grating
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns></returns>
    public static int GetGratingType(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      int ret;
      //lock the document and start transaction
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kGrating))
        {
          Autodesk.AdvanceSteel.Modelling.Grating grating = obj as Autodesk.AdvanceSteel.Modelling.Grating;
          ret = (int)grating.GratingType;
        }
        else
          throw new System.Exception("Not a Grating Object");
      }
      return ret;
    }
  }
}
