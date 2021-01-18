using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public class PropertiesShearStuds
  {

    public PropertiesShearStuds()
    {
    }

    public PropertiesShearStuds(string standard,
                            string grade,
                            double diameter, double length,
                            int xCount, int yCount,
                            double xSpacing, double ySpacing,
                            double radius, int shearStudConnectionType,
                            bool displayAsSolid)
    {
      Standard = standard;
      Grade = grade;
      Diameter = diameter;
      Length = length;
      XCount = xCount;
      YCount = yCount;
      XSpacing = xSpacing;
      YSpacing = ySpacing;
      Radius = radius;
      ShearStudConnectionType = shearStudConnectionType;
      DisplayAsSolid = displayAsSolid;
    }

    public PropertiesShearStuds(PropertiesShearStuds derivedShearStudProperties)
    {
      Standard = derivedShearStudProperties.Standard;
      Grade = derivedShearStudProperties.Grade;
      Diameter = derivedShearStudProperties.Diameter;
      Length = derivedShearStudProperties.Length;
      XCount = derivedShearStudProperties.XCount;
      YCount = derivedShearStudProperties.YCount;
      XSpacing = derivedShearStudProperties.XSpacing;
      YSpacing = derivedShearStudProperties.YSpacing;
      Radius = derivedShearStudProperties.Radius;
      ShearStudConnectionType = derivedShearStudProperties.ShearStudConnectionType;
      DisplayAsSolid = derivedShearStudProperties.DisplayAsSolid;
    }

    private string _standard = "";
    private string _grade = "";
    private double _diameter = 0;
    private double _length = 0;

    private int _yCount = -1; //nx
    private int _xCount = -1; //ny
    private double _xSpacing = 0; //dx
    private double _ySpacing = 0; //dy

    private double _radius = 0;
    private int _shearStudConnType = -1;
    private bool _displayAsSolid = false;

    private bool foundData { get; set; }

    public string Standard
    {
      get { return _standard; }
      set
      {
        _standard = value;
        if (!foundData)
        {
          foundData = !string.IsNullOrEmpty(value);
        }
      }
    }

    public string Grade
    {
      get { return _grade; }
      set
      {
        _grade = value;
        if (!foundData)
        {
          foundData = !string.IsNullOrEmpty(value);
        }
      }
    }

    public double Diameter
    {
      get { return _diameter; }
      set
      {
        _diameter = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public double Length
    {
      get { return _length; }
      set
      {
        _length = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public double XSpacing
    {
      get { return _xSpacing; }
      set
      {
        _xSpacing = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public double YSpacing
    {
      get { return _ySpacing; }
      set
      {
        _ySpacing = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public int XCount
    {
      get { return _xCount; }
      set
      {
        _xCount = value;
        if (!foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public int YCount
    {
      get { return _yCount; }
      set
      {
        _yCount = value;
        if (!foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public double Radius
    {
      get { return _radius; }
      set
      {
        _radius = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public int ShearStudConnectionType
    {
      get { return _shearStudConnType; }
      set
      {
        _shearStudConnType = value;
        if (!foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public bool DisplayAsSolid
    {
      get { return _displayAsSolid; }
      set
      {
        _displayAsSolid = value;
      }
    }

    public bool HasProperties()
    {
      return foundData;
    }

  }
}