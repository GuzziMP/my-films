using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Google.API.Translate;
using HtmlAgilityPack;
//using System.Web.Extensions;

namespace Grabber.Util
{
  public class TranslateProvider
  {
    private readonly TranslateClient client;
    private readonly string referrer = string.Empty;

    public TranslateProvider(string referrer)
    {
      this.referrer = referrer;
      this.client = new TranslateClient(this.referrer);
    }

    public string Translate(string input, string translateTo, out string detectedLanguage, out bool reliable,
                            out double confidence, string lineshiftMark)
    {
      const string symbolToRepresentLineshift = "QQQQQ";
      input = input.Replace(lineshiftMark, symbolToRepresentLineshift);

      detectedLanguage = client.Detect(input, out reliable, out confidence);
      string translated = client.Translate(input, detectedLanguage, translateTo);

      translated = translated.Replace(symbolToRepresentLineshift, lineshiftMark);

      //translated = translated.Replace("[*] ", lineshiftMark);
      //translated = translated.Replace("[ *] ", lineshiftMark);
      //translated = translated.Replace("[* ] ", lineshiftMark);
      //translated = translated.Replace("[ * ] ", lineshiftMark);
      translated = translated.Replace(" " + lineshiftMark, lineshiftMark);
      return translated;
    }

  }

  public class BingTranslator
  {
    private void TranslateText()
    {
      string applicationid = "<your Bing appId>";
      string fromlanguage = "en";//from language you can change this as your requirement
      string translatedText = "";//collect result here
      string texttotranslate = ""; // txttext.Text;//what to be translated?
      string tolanguage = ""; // ddltolang.SelectedValue.ToString();//in which language?
      //preparing url with all four parameter
      string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" + applicationid + "&text=" + texttotranslate + "&from=" + fromlanguage + "&to=" + tolanguage;
      //making web request to url
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
      //getting response from api
      WebResponse response = request.GetResponse();

      Stream strm = response.GetResponseStream();

      StreamReader reader = new StreamReader(strm);
      //reading result
      translatedText = reader.ReadToEnd();

      // Response.Write("Converted Texts Are: " + translatedText);
      response.Close();
    }
  }

  public class GoogleTranslator
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    /// <summary>
    /// Translates a string into another language using Google's translate API JSON calls.
    /// <seealso>Class TranslationServices</seealso>
    /// </summary>
    /// <param name="text">Text to translate. Should be a single word or sentence.</param>
    /// <param name="fromCulture">
    /// Two letter culture (en of en-us, fr of fr-ca, de of de-ch)
    /// </param>
    /// <param name="toCulture">
    /// Two letter culture (as for FromCulture)
    /// </param>
    public string TranslateGoogle(string text, string fromCulture, string toCulture)
    {
      fromCulture = fromCulture.ToLower();
      toCulture = toCulture.ToLower();

      // normalize the culture in case something like en-us was passed 
      // retrieve only en since Google doesn't support sub-locales
      string[] tokens = fromCulture.Split('-');
      if (tokens.Length > 1)
        fromCulture = tokens[0];

      // normalize ToCulture
      tokens = toCulture.Split('-');
      if (tokens.Length > 1)
        toCulture = tokens[0];

      string url = string.Format(@"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}", HttpUtility.UrlEncode(text), fromCulture, toCulture);

      // Retrieve Translation with HTTP GET call
      string html;
      try
      {
        WebClient web = new WebClient();

        // MUST add a known browser user agent or else response encoding doen't return UTF-8 (WTF Google?)
        web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
        web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

        // Make sure we have response encoding to UTF-8
        web.Encoding = Encoding.UTF8;
        html = web.DownloadString(url);
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("ConnectionFailed: " + ex.GetBaseException().Message);
        return null;
      }

      // Extract out trans":"...[Extracted]...","from the JSON string
      string result = Regex.Match(html, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value;

      if (string.IsNullOrEmpty(result))
      {
        LogMyFilms.Error("No result found!");
        return null;
      }

      //return WebUtils.DecodeJsString(result);

      // Result is a JavaScript string so we need to deserialize it properly
      JavaScriptSerializer ser = new JavaScriptSerializer();
      return ser.Deserialize(result, typeof(string)) as string;
    }

    public class BabelFishTranslator
    {
      public string TranslateBabelFish(string Text, string FromCulture, string ToCulture)
      {
        FromCulture = GetNeutralCulture(FromCulture).TwoLetterISOLanguageName;
        ToCulture = GetNeutralCulture(ToCulture).TwoLetterISOLanguageName;

        // Override since yahoo doesn't understand zh-Hans/zh-Hant
        if (FromCulture == "zh")
        {
          if (GetNeutralCulture(FromCulture).ThreeLetterISOLanguageName == "CHT")
          {
            FromCulture = "zt";
          }
        }

        if (ToCulture == "zh")
        {
          if (GetNeutralCulture(ToCulture).ThreeLetterISOLanguageName == "CHT")
          {
            ToCulture = "zt";
          }
        }
        string LangPair = FromCulture + "_" + ToCulture;

        string url = string.Format(@"http://babelfish.yahoo.com/translate_txt?ei=UTF-8&doit=done&fr=bf-home&intl=1&tt=urltext&trtext={0}&lp={1}&btnTrTxt=Translate",
                                   HttpUtility.UrlEncode(Text), LangPair);

        // Retrieve Translation with HTTP GET call
        string Html = null;
        try
        {
          WebClient web = new WebClient();

          // MUST add the following browser user agent or else yahoo doesn't respond correctly (WTF Yahoo?)
          web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

          // Make sure we have response encoding to UTF-8
          web.Encoding = Encoding.UTF8;
          Html = web.DownloadString(url);
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("ConnectionFailed: " + ex.GetBaseException().Message);
          return null;
        }

        // <div id="result"><div style="padding:0.6em;">Hallo</div></div>
        string Result = ""; // ToDo: StringUtils.ExtractString(Html, "<div id=\"result\">", "</div>");
        if (Result == "")
        {
          // ErrorMessage = "Invalid search result. Couldn't find marker.";
          return null;
        }
        Result = Result.Substring(Result.LastIndexOf(">") + 1);

        return HttpUtility.HtmlDecode(Result);
      }

      private System.Globalization.CultureInfo GetNeutralCulture(string culture)
      {
        return GetNeutralCulture(System.Globalization.CultureInfo.CreateSpecificCulture(culture));
      }

      private System.Globalization.CultureInfo GetNeutralCulture(System.Globalization.CultureInfo ci)
      {
        System.Globalization.CultureInfo ci2 = ci;
        while (!ci2.IsNeutralCulture && ci2.Parent.Name != "")
          ci2 = ci2.Parent;
        return ci2;
      }

    }

  }

  public class GoogleTranslatorWeb
  {

    static void Main(string[] args)
    {
      string content = "Thank You";

      // Set the From and To language
      string fromLanguage = "English";
      string toLanguage = "Spanish";

      // Create a Language mapping
      Dictionary<string, string> languageMap = new Dictionary<string, string>();
      // ToDo: InitLanguageMap(languageMap);

      Console.WriteLine("Given Word: " + content);

      // Create an instance of WebClient in order to make the language translation
      Uri address = new Uri("http://translate.google.com/translate_t");
      WebClient wc = new WebClient();
      wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
      wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);

      // Async Upload to the specified source
      // i.e http://translate.google.com/translate_t for handling the translation.
      wc.UploadStringAsync(address,
         GetPostData(languageMap[fromLanguage], languageMap[toLanguage], content));

      Console.ReadLine();
    }

    static string GetPostData(string fromLanguage, string toLanguage, string content)
    {
      // Set the language translation. All we need is the language pair, from and to.
      string strPostData = string.Format("hl=en&ie=UTF8&oe=UTF8submit=Translate&langpair={0}|{1}",
                                         fromLanguage,
                                         toLanguage);

      // Encode the content and set the text query string param
      return strPostData += "&text=" + HttpUtility.UrlEncode(content);
    }

    static void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
    {
      if (e.Result != null)
      {
        var doc = new HtmlDocument();
        doc.LoadHtml(e.Result);
        var node = doc.DocumentNode.SelectSingleNode("//span[@id='result_box']");
        var output = node != null ? node.InnerText : e.Error.Message;
        Console.WriteLine("Translated Text: " + output);
      }
    }
  }
}