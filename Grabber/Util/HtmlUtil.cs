using System;
using System.Text;
using System.IO;
using System.Web;
namespace Grabber.Util
{
  public class HtmlUtil
  {
    public HtmlUtil() { }

    public int FindTag(string strHtml, string strTag, ref string strtagFound, int iPos)
    {
      if (iPos < 0 || iPos >= strHtml.Length) return -1;
      string strHtmlLow = strHtml;
      string strTagLow = strTag;
      strHtmlLow = strHtmlLow.ToLowerInvariant();
      strTagLow = strTagLow.ToLowerInvariant();
      strtagFound = "";
      int iStart = strHtmlLow.IndexOf(strTag, iPos);
      if (iStart < 0) return -1;
      int iEnd = strHtmlLow.IndexOf(">", iStart);
      if (iEnd < 0) iEnd = strHtmlLow.Length;
      strtagFound = strHtmlLow.Substring(iStart, (iEnd + 1) - iStart);
      return iStart;
    }

    public int FindClosingTag(string strHtml, string strTag, ref string strtagFound, int iPos)
    {
      string strHTMLLow = strHtml.ToLowerInvariant();
      string strTagLow = strTag.ToLowerInvariant();
      strtagFound = "";
      int iStart = strHTMLLow.IndexOf("</" + strTag, iPos);
      if (iStart < 0) return -1;
      int iOpenStart = strHTMLLow.IndexOf("<" + strTag, iPos);
      while (iOpenStart < iStart && iOpenStart != -1)
      {
        iStart = strHTMLLow.IndexOf("</" + strTag, iStart + 1);
        iOpenStart = strHTMLLow.IndexOf("<" + strTag, iOpenStart + 1);
      }

      int iEnd = strHTMLLow.IndexOf(">", iStart);
      if (iEnd < 0) iEnd = strHTMLLow.Length;
      strtagFound = strHTMLLow.Substring(iStart, (iEnd + 1) - iStart);
      return iStart;
    }


    public void getValueOfTag(string strTagAndValue, out string strValue)
    {
      // strTagAndValue contains:
      // like <a href=blablabla.....>value</a>
      strValue = strTagAndValue;
      int iStart = strTagAndValue.IndexOf(">");
      int iEnd = strTagAndValue.IndexOf("<", iStart + 1);
      if (iStart >= 0 && iEnd >= 0)
      {
        iStart++;
        strValue = strTagAndValue.Substring(iStart, iEnd - iStart);
      }
    }

    public void getAttributeOfTag(string strTagAndValue, string strTag, ref string strValue)
    {
      // strTagAndValue contains:
      // like <a href=""value".....
      strValue = strTagAndValue;
      int iStart = strTagAndValue.IndexOf(strTag);
      if (iStart < 0) return;
      iStart += strTag.Length;
      while (strTagAndValue[iStart + 1] == 0x20 || strTagAndValue[iStart + 1] == 0x27 ||
             strTagAndValue[iStart + 1] == 34) iStart++;
      int iEnd = iStart + 1;
      while (strTagAndValue[iEnd] != 0x27 && strTagAndValue[iEnd] != 0x20 && strTagAndValue[iEnd] != 34 &&
             strTagAndValue[iEnd] != '>') iEnd++;
      if (iStart >= 0 && iEnd >= 0)
      {
        strValue = strTagAndValue.Substring(iStart, iEnd - iStart);
      }
    }

    public void RemoveTags(ref string strHtml)
    {
      int iNested = 0;
      string strReturn = "";
      for (int i = 0; i < strHtml.Length; ++i)
      {
        if (strHtml[i] == '<') iNested++;
        else if (strHtml[i] == '>') iNested--;
        else
        {
          if (0 == iNested)
          {
            strReturn += strHtml[i];
          }
        }
      }
      strHtml = strReturn;
    }

    public string ConvertHTMLToAnsi(string strHtml)
    {
      string strippedHtml = string.Empty;
      ConvertHTMLToAnsi(strHtml, out strippedHtml);
      return strippedHtml;
    }

    public void ConvertHTMLToAnsi(string strHtml, out string strStripped)
    {
      strStripped = "";
      //	    int i=0; 
      if (strHtml.Length == 0)
      {
        strStripped = "";
        return;
      }
      //int iAnsiPos=0;
      StringWriter writer = new StringWriter();

      HttpUtility.HtmlDecode(strHtml, writer);

      String DecodedString = writer.ToString();
      strStripped = DecodedString.Replace("<br>", "\n");
      if (true)
        return;
      /*
            string szAnsi = "";

            while (i < (int)strHtml.Length )
            {
              char kar=strHtml[i];
              if (kar=='&')
              {
                if (strHtml[i+1]=='#')
                {
                  int ipos=0;
                  i+=2;
                  string szDigit="";
                  while ( ipos < 12 && i<strHtml.Length && Char.IsDigit(strHtml[i])) 
                  {
                    szDigit+=strHtml[i];
                    ipos++;
                    i++;
                  }
                  szAnsi+= (char)(Int32.Parse(szDigit));
                  i++;
                }
                else
                {
                  i++;
                  int ipos=0;
                  string szKey="";
                  while (i<strHtml.Length && strHtml[i] != ';' && ipos < 12)
                  {
                    szKey+=Char.ToLower(strHtml[i]);
                    ipos++;
                    i++;
                  }
                  i++;
                  if (String.Compare(szKey,"amp")==0) szAnsi+='&';
                  if (String.Compare(szKey,"nbsp")==0) szAnsi+=' ';
                }
              }
              else
              {
                szAnsi+=kar;
                i++;
              }
            }
            strStripped=szAnsi;*/
    }

    public void ParseAHREF(string ahref, out string title, out string url)
    {
      title = "";
      url = "";
      int pos1 = ahref.IndexOf("\"");
      if (pos1 < 0) return;
      int pos2 = ahref.IndexOf("\"", pos1 + 1);
      if (pos2 < 0) return;
      url = ahref.Substring(pos1 + 1, pos2 - pos1 - 1);

      pos1 = ahref.IndexOf(">");
      if (pos1 < 0) return;
      pos2 = ahref.IndexOf("<", pos1);
      if (pos2 < 0) return;
      title = ahref.Substring(pos1 + 1, pos2 - pos1 - 1);
    }
  }

}