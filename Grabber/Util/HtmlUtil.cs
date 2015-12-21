using System.IO;
using System.Web;
namespace Grabber.Util
{
  public class HtmlUtil
  {
    public int FindTag(string strHTML, string strTag, ref string strtagFound, int iPos)
    {
      if (iPos < 0 || iPos >= strHTML.Length)
        return -1;
      string str1 = strHTML;
      string str2 = strTag;
      string str3 = str1.ToLowerInvariant();
      str2.ToLowerInvariant();
      strtagFound = "";
      int startIndex = str3.IndexOf(strTag, iPos);
      if (startIndex < 0)
        return -1;
      int num = str3.IndexOf(">", startIndex);
      if (num < 0)
        num = str3.Length;
      strtagFound = str3.Substring(startIndex, num + 1 - startIndex);
      return startIndex;
    }

    public int FindClosingTag(string strHTML, string strTag, ref string strtagFound, int iPos)
    {
      string str = strHTML.ToLowerInvariant();
      strTag.ToLowerInvariant();
      strtagFound = "";
      int startIndex = str.IndexOf("</" + strTag, iPos);
      if (startIndex < 0)
        return -1;
      for (int index = str.IndexOf("<" + strTag, iPos); index < startIndex && index != -1; index = str.IndexOf("<" + strTag, index + 1))
        startIndex = str.IndexOf("</" + strTag, startIndex + 1);
      int num = str.IndexOf(">", startIndex);
      if (num < 0)
        num = str.Length;
      strtagFound = str.Substring(startIndex, num + 1 - startIndex);
      return startIndex;
    }

    public void getValueOfTag(string strTagAndValue, out string strValue)
    {
      strValue = strTagAndValue;
      int num1 = strTagAndValue.IndexOf(">");
      int num2 = strTagAndValue.IndexOf("<", num1 + 1);
      if (num1 < 0 || num2 < 0)
        return;
      int startIndex = num1 + 1;
      strValue = strTagAndValue.Substring(startIndex, num2 - startIndex);
    }

    public void getAttributeOfTag(string strTagAndValue, string strTag, ref string strValue)
    {
      strValue = strTagAndValue;
      int num = strTagAndValue.IndexOf(strTag);
      if (num < 0)
        return;
      int startIndex = num + strTag.Length;
      while ((int)strTagAndValue[startIndex + 1] == 32 || (int)strTagAndValue[startIndex + 1] == 39 || (int)strTagAndValue[startIndex + 1] == 34)
        ++startIndex;
      int index = startIndex + 1;
      while ((int)strTagAndValue[index] != 39 && (int)strTagAndValue[index] != 32 && ((int)strTagAndValue[index] != 34 && (int)strTagAndValue[index] != 62))
        ++index;
      if (startIndex < 0 || index < 0)
        return;
      strValue = strTagAndValue.Substring(startIndex, index - startIndex);
    }

    public void RemoveTags(ref string strHTML)
    {
      int num = 0;
      string str = "";
      for (int index = 0; index < strHTML.Length; ++index)
      {
        if ((int)strHTML[index] == 60)
          ++num;
        else if ((int)strHTML[index] == 62)
          --num;
        else if (num == 0)
          str += (string)(object)strHTML[index];
      }
      strHTML = str;
    }

    public string ConvertHTMLToAnsi(string strHTML)
    {
      string strStripped = string.Empty;
      this.ConvertHTMLToAnsi(strHTML, out strStripped);
      return strStripped;
    }

    public void ConvertHTMLToAnsi(string strHTML, out string strStripped)
    {
      strStripped = "";
      if (strHTML.Length == 0)
      {
        strStripped = "";
      }
      else
      {
        StringWriter stringWriter = new StringWriter();
        HttpUtility.HtmlDecode(strHTML, (TextWriter)stringWriter);
        string str = stringWriter.ToString();
        strStripped = str.Replace("<br>", "\n");
      }
    }

    public void ParseAHREF(string ahref, out string title, out string url)
    {
      title = "";
      url = "";
      int num1 = ahref.IndexOf("\"");
      if (num1 < 0)
        return;
      int num2 = ahref.IndexOf("\"", num1 + 1);
      if (num2 < 0)
        return;
      url = ahref.Substring(num1 + 1, num2 - num1 - 1);
      int startIndex = ahref.IndexOf(">");
      if (startIndex < 0)
        return;
      int num3 = ahref.IndexOf("<", startIndex);
      if (num3 < 0)
        return;
      title = ahref.Substring(startIndex + 1, num3 - startIndex - 1);
    }
  }
}