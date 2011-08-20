﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MediaPortal.GUI.Library;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
    public class LoadParameterInfo
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

        public enum ReturnMode { Locked, Root };

        protected bool _ShowVKonFailedSearch = true;

        public string Config { get; protected set; }
        public string MovieID { get; protected set; }
        public string Play { get; protected set; }

        public string View { get; protected set; }
        public string ViewValue { get; protected set; }

        public string Layout { get; protected set; }

        public string Search { get; protected set; }
        
        public bool ShowVKonFailedSearch { get { return _ShowVKonFailedSearch; } }
        public ReturnMode Return { get; protected set; }

        public LoadParameterInfo(string loadParam)
        {
            LogMyFilms.Debug("LoadParameterInfo() : parsing load parameter: '" + loadParam + "'");  
            // MF planned options:
            // config:<configname>|view:<viewname>|viewvalue:<viewvalue>|search:<searchexpression>|layout:<facadeviewtype>|movieid:<movieid>
            // priority order: search overrules property view. propertyvalue is optional. view is optional, config is optional, if not set, last used or default will be used.
            // actors are possible by using property:actors|propertyvalue:actorname
            // if invalid values are given, param start is ignored
            // optional: add public method for basichome editors to ask possible values
            // new: add jump to DETAILS view for certain movie for trakt integration !!! -> to be done my config:<configname>|movieid:<movieid>

            Return = ReturnMode.Root;

            // if we cant load params or there is no param passed, quit
            if (string.IsNullOrEmpty(loadParam)) 
              return;

            Config = Regex.Match(loadParam, "config:([^|]*)").Groups[1].Value; // name of configuration
            MovieID = Regex.Match(loadParam, "movieid:([^|]*)").Groups[1].Value; // movieID - used by Trakt
            Play = Regex.Match(loadParam, "play:([^|]*)").Groups[1].Value; // "true" -> start movie with MovieID
            
            View = Regex.Match(loadParam, "view:([^|]*)").Groups[1].Value; // Category name as start grouped view
            ViewValue = Regex.Match(loadParam, "viewvalue:([^|]*)").Groups[1].Value; // Category value, e.g. "Country" to show that category's movies directly
            Layout = Regex.Match(loadParam, "layout:([^|]*)").Groups[1].Value; // Choose Layout -> 0, 1, 2, 3, 4 for ListView, SmallThumb, BigThumb, FilmStrip, CoverFlow
            Search = Regex.Match(loadParam, "search:([^|]*)").Groups[1].Value; // Start MyFilms with GlobalSearch with that parameter
            if (!bool.TryParse(Regex.Match(loadParam, "VKonfail:([^|]*)").Groups[1].Value, out _ShowVKonFailedSearch)) _ShowVKonFailedSearch = true;
            try { Return = (ReturnMode)Enum.Parse(typeof(ReturnMode), Regex.Match(loadParam, "return:([^|]*)").Groups[1].Value); }
            catch { Return = ReturnMode.Root; }

            //try
            //{
            //  foreach (String currParam in loadParam.Split('|'))
            //  {
            //    String[] keyValue = currParam.Split(':');
            //    String key = keyValue[0];
            //    String value = keyValue[1];

            //    try
            //    {
            //      switch (key)
            //      {
            //        case "categoryname":
            //          Category = value;
            //          if (Category == null) MyFilms.ShowNotifyDialog("MyFilms", "BadCategory");
            //          break;
            //        case "movieid":
            //          break;
            //      }
            //    }
            //    catch (FormatException)
            //    {
            //      LogMyFilms.Warn("Received invalid load parameter: " + currParam);
            //      MyFilms.ShowNotifyDialog("MyFilms", "Bad Loadparameter");
            //    }
            //  }
            //}
            //catch (Exception ex)
            //{
            //  LogMyFilms.Debug("Unexpected error parsing paramater: " + loadParam, ex);
            //}
        }

        public static string FromGuiProperties()
        {
            List<string> paramsFromGuiProps = new List<string>();
            if (!string.IsNullOrEmpty(GUIPropertyManager.GetProperty("#myfilms.startparams.config")))
            {
              paramsFromGuiProps.Add("config:" + GUIPropertyManager.GetProperty("#myfilms.startparams.config"));
              GUIPropertyManager.SetProperty("#myfilms.startparams.config", string.Empty);
            }
            if (!string.IsNullOrEmpty(GUIPropertyManager.GetProperty("#myfilms.startparams.movieid")))
            {
              paramsFromGuiProps.Add("movie:" + GUIPropertyManager.GetProperty("#myfilms.startparams.movieid"));
              GUIPropertyManager.SetProperty("#myfilms.startparams.movieid", string.Empty);
            }
            if (paramsFromGuiProps.Count > 0) 
              return string.Join("|", paramsFromGuiProps.ToArray());
            else 
              return null;
        }

    }
}
