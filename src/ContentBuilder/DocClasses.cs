using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ContentBuilder
{
  [Serializable, XmlRoot("doc")]
  public class Doc
  {
    public Doc()
    {
      assembly = new AssemblyFile();
      content = new Members();
    }

    [XmlElement("assembly")]
    public AssemblyFile assembly { get; set; }

    [XmlElement("members")]
    public Members content { get; set; }

    public void AddContent(Doc other)
    {
      content.members.AddRange(other.content.members);
    }
  }

  public class AssemblyFile
  {
    [XmlElement("name")]
    public string name { get; set; }
  }

  public class Members
  {
    public Members()
    {
      members = new List<Member>();
    }

    [XmlElement("member")]
    public List<Member> members { get; set; }
  }


  public class Member
  {
    public Member()
    {
      name = String.Empty;
      remarks = String.Empty;
    }

    internal string signature { get; set; }

    [XmlAttribute]
    public string name { get; set; }

    [XmlElement("summary")]
    public Summary summary { get; set; }

    [XmlElement("param")]
    public List<Param> parameters { get; set; }

    [XmlElement("returns")]
    public List<Return> returns { get; set; }

    [XmlElement("remarks")]
    public string remarks { get; set; }
  }


  public class Summary
  {
    public Summary()
    {
      description = string.Empty;
    }

    [XmlText]
    public string description { get; set; }
  }

  public class Param
  {
    public Param()
    {
      name = String.Empty;
      description = string.Empty;
    }

    [XmlAttribute]
    public string name { get; set; }

    [XmlText]
    public string description { get; set; }
  }

  public class Return
  {
    public Return()
    {
      name = String.Empty;
      description = string.Empty;
    }
    [XmlAttribute]
    public string name { get; set; }

    [XmlText]
    public string description { get; set; }
  }
}