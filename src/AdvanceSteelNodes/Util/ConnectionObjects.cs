using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  public class ConnectionObjects
  {
    internal ConnectionObjects() { }

    /// <summary>
    /// Set Assembly Location 
    /// </summary>
    /// <param name="connectionType">Input assembly location</param>
    /// <param name="screwBolts">Input aux </param>
    /// <returns></returns>
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
    /// Set Anchor Bolt Orientation
    /// </summary>
    /// <param name="orientation">Input Anchor Bolt Orientation location</param>
    /// <param name="anchorBolts">Input aux </param>
    /// <returns></returns>
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
  }
}