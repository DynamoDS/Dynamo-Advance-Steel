using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.ConnectionObjectsFunctions
{
  public class ConnectionObject
  {
    internal ConnectionObject() { }

    /// <summary>
    /// Set assembly location 
    /// </summary>
    /// <param name="screwBolts">Input connection object </param>
    /// <param name="connectionType">Input assembly location</param>
    /// <returns name="screwBolts">The updated screwBolts object</returns>
    public static SteelDbObject SetBoltAssemblyLocation(SteelDbObject screwBolts, int connectionType)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern obj = Utils.GetObject(screwBolts.Handle) as Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern;
        if (obj != null)
        {
          obj.AssemblyLocation = (AtomicElement.eAssemblyLocation)connectionType;
          return screwBolts;
        }
        else
        {
          throw new System.Exception("failed to get the connection object");
        }
      }
    }

    /// <summary>
    /// Get assembly location: Unkown = -1, 0 = On Site, 1 = Site Drilled, 2 = In Shop
    /// </summary>
    /// <param name="screwBolts">Input connection object </param>
    /// <returns name="assemblyLocation">An integer that represents the assembly location</returns>
    public static int GetBoltAssemblyLocation(AdvanceSteel.Nodes.SteelDbObject screwBolts)
    {
      int ret = -1;
      using (var ctx = new SteelServices.DocContext())
      {
        if (screwBolts != null)
        {
          Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern obj = Utils.GetObject(screwBolts.Handle) as Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern;
          if (obj != null)
          {
            ret = (int)obj.AssemblyLocation;
          }
          else
          {
            throw new System.Exception("failed to get the connection object");
          }
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    /// <summary>
    /// Set orientation for anchol bolt object
    /// </summary>
    /// <param name="anchorBolts">Input anchor bolts object </param>
    /// <param name="orientation">Input Anchor Bolt Orientation location</param>
    /// <returns name="anchorBolts">The updated object</returns>
    public static SteelDbObject SetAnchorBoltOrientation(SteelDbObject anchorBolts, int orientation)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        Autodesk.AdvanceSteel.Modelling.AnchorPattern obj = Utils.GetObject(anchorBolts.Handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;
        if (obj != null)
        {
          obj.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)orientation;
          return anchorBolts;
        }
        else
        {
          throw new System.Exception("failed to get the connection object");
        }
      }
    }

    /// <summary>
    /// Get anchor bolt orientation type: NormalOrientation = 0, 1 = DiagonalInside, 2 = DiagonalOutside, 3 = AllOutside, 4 = AllInside, 5 = InsideRotated, 6 = OutsideRotated
    /// </summary>
    /// <param name="anchorBolts">Input anchor bolts object </param>
    /// <returns name="orientation">An integer that represents the orientation</returns>
    public static int GetAnchorBoltOrientation(AdvanceSteel.Nodes.SteelDbObject anchorBolts)
    {
      int ret = -1;
      using (var ctx = new SteelServices.DocContext())
      {
        if (anchorBolts != null)
        {
          Autodesk.AdvanceSteel.Modelling.AnchorPattern obj = Utils.GetObject(anchorBolts.Handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;
          if (obj != null)
          {
            ret = (int)obj.OrientationType;
          }
          else
          {
            throw new System.Exception("failed to get the connection object");
          }
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    /// <summary>
    /// Get weight from the steel object
    /// </summary>
    /// <param name="steelObject">Input steel object</param>
    /// <returns name="weight">The weight from the steel object</returns>
    public static double GetWeight(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      double ret = 0;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          ret = Utils.GetWeight(steelObject.Handle, 0);
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return Utils.FromInternalWeightUnits(ret, true);
    }
  }
}