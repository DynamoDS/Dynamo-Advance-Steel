using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ContentBuilder
{
  class MDGenerator
  {
    public static bool Generate(Doc doc, IEnumerable<Member> whichMethods, string strFullDocName)
    {
      bool bOk = false;
      using (FileStream fileStream = new FileStream(strFullDocName, FileMode.Create, FileAccess.Write))
      {
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
          // just write down the content
          //
          bOk = WriteFile(doc, whichMethods, writer);
          writer.Flush();
        }
      }
      return bOk;
    }

    public static void GenerateFromAssembly(string strFullAssemblyDocDir, IEnumerable<Member> methods, string strFullDocName)
    {
      // try to read the doc content
      //
      List<string> xmlFiles = getAllXmlFiles(strFullAssemblyDocDir);
      Doc docForAll = new Doc();
      foreach (string strXmlFile in xmlFiles)
      {
        try
        {
          XmlSerializer serializer = new XmlSerializer(typeof(Doc));
          using (FileStream fileStream = new FileStream(strXmlFile, FileMode.Open))
          {
            Doc currDoc = (Doc)serializer.Deserialize(fileStream);
            if (null == currDoc)
            {
              System.Console.WriteLine(" Cannot read the XML file !" + strXmlFile + " ... continue...");
              continue;
            }

            docForAll.AddContent(currDoc);
          }
        }
        catch { }
      }


      // .. try to generate the MD file at the specific location
      // 
      bool bOk = MDGenerator.Generate(docForAll, methods, strFullDocName);
      if (!bOk)
      {
        System.Console.WriteLine(" Cannot not find all methods description !");
        return;
      }
    }
    // just returns all xml files from target directory
    //
    static List<string> getAllXmlFiles(string strDir)
    {
      List<string> ret = new List<string>();
      DirectoryInfo d = new DirectoryInfo(strDir);
      foreach (var file in d.GetFiles("*.xml"))
      {
        ret.Add(file.FullName);
      }
      return ret;
    }

    private static Member findMethod(Doc doc, string desiredMethod)
    {
      Member ret = null;

      foreach (Member currMember in doc.content.members)
      {
        string nameWithoutParams = currMember.name.Split('(').FirstOrDefault();
        if (nameWithoutParams != null && nameWithoutParams.EndsWith(desiredMethod))
        {
          ret = currMember;
          ret.name = desiredMethod;
          break;
        }

        if (currMember.summary.userName.Contains(desiredMethod))
        {
          ret = currMember;
          ret.name = desiredMethod;
          break;
        }
      }

      return ret;
    }


    static private string NormalizeString(string strInput)
    {
      string ret = strInput;
      // remove \r?\n
      //
      ret = ret.Replace("\r\n", "");
      ret = ret.Replace("\n", "");

      // reduce spaces to one
      //
      RegexOptions options = RegexOptions.None;
      Regex regex = new Regex("[ ]{2,}", options);
      ret = regex.Replace(ret, " ");


      return ret;
    }

    static private string EscapeSpecialCharacters(string strInput)
    {
      string ret = strInput;

      RegexOptions options = RegexOptions.None;
      Regex regex = new Regex(@"([^\\])([*`#|\\])", options);
      ret = regex.Replace(ret, @"$1\$2"); ;

      return ret;
    }

    private static bool WriteMethod(Member member, StreamWriter writer)
    {
      //string strTemplate = " | **NodeName**<BR> NodeDesc | **InParamName**<BR> InParamDesc<BR> | **OutParamName**<BR> OutParamDesc<BR> | ";
      // node name and desc
      //
      string textNodeTemplate = "**NodeName**<BR> NodeDesc";

      if (0 != member.summary.userName.Length)
      {
        textNodeTemplate = textNodeTemplate.Replace("NodeName", NormalizeString(member.name));
      }
      else
      {
        textNodeTemplate = textNodeTemplate.Replace("NodeName", NormalizeString(member.name));
      }
      textNodeTemplate = textNodeTemplate.Replace("NodeDesc", NormalizeString(member.summary.description));

      // inParams
      //
      string textInParamsTemplate = "**InParamName**<BR> InParamDesc<BR>";
      string textInParams = "";

      foreach (Param param in member.parameters)
      {
        string strTemp = textInParamsTemplate;
        strTemp = strTemp.Replace("InParamName", NormalizeString(param.name));
        strTemp = strTemp.Replace("InParamDesc", EscapeSpecialCharacters(NormalizeString(param.description)));

        textInParams += strTemp;
      }
      // add also the remarks remarks
      //
      string textRemarksTemplate = "remarks";
      if (String.Empty != member.remarks)
      {
        string strTemp = textRemarksTemplate.Replace("remarks", EscapeSpecialCharacters(NormalizeString(member.remarks)));
        textInParams += strTemp;
      }



      // outPrams
      //
      string textOutParamsTemplate = "**OutParamName**<BR> OutParamDesc<BR>";
      string textOutParams = "";

      foreach (Return ret in member.returns)
      {
        string strTemp = textOutParamsTemplate;
        strTemp = strTemp.Replace("OutParamName", NormalizeString(ret.name));
        strTemp = strTemp.Replace("OutParamDesc", EscapeSpecialCharacters(NormalizeString(ret.description)));

        textOutParams += strTemp;
      }


      string text = " | " + textNodeTemplate + " | " + textInParams + " | " + textOutParams + " | ";

      // remove **** for the case when any string was empty
      //
      text = text.Replace("****", "");


      writer.WriteLine(text);
      Debug.WriteLine(text);
      return true;
    }

    private static bool WriteFile(Doc doc, IEnumerable<Member> whichMethods, StreamWriter writer)
    {
      bool bOk = true;

      writer.WriteLine("# Dynamo Nodes for Advance Steel");
      writer.WriteLine("\r\n"); // Do Not remove this or otherwise there will be no tbale
      writer.WriteLine(" | Dynamo Node | Inputs | Outputs | ", writer);
      writer.WriteLine(" | --- | --- | --- | ", writer);

      foreach (Member currMethod in whichMethods)
      {
        Member foundDocMethod = findMethod(doc, currMethod.name);
        if (null == foundDocMethod) // if the method is not in the xml file, use the information from the attributes
          foundDocMethod = currMethod;


        WriteMethod(foundDocMethod, writer);
      }

      return bOk;
    }

  }
}
