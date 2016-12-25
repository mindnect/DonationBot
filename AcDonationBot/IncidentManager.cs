﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Verse;

namespace AlcoholV
{
    public class CustomIncident
    {
        public string command = "";
        public string condition = "";
        public string defsName = "";
        public ExcuteType excuteType = ExcuteType.INSTANT;
    }

    // Singleton
    public class IncidentManager
    {
        private const string FolderPath = @"c:\ChatApp\";
        private const string FileName = @"Event.xml";
        private const string FilePath = @"c:\ChatApp\Event.xml";
        public static readonly List<CustomIncident> Datas = new List<CustomIncident>();

        private IncidentManager()
        {
        }

        public static bool IsValidElementName(string tagName)
        {
            try
            {
                XmlConvert.VerifyName(tagName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Init()
        {
            LoadData();

            var fsw = new FileSystemWatcher(FolderPath)
            {
                Filter = FileName,
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            fsw.Changed += LoadData;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        protected void LoadFromXml(XDocument xml)
        {
            Datas.Clear(); // 불러오기 전에 클리어
            foreach (var node in xml.Root.Elements("Incident"))
                Datas.Add(new CustomIncident
                {
                    defsName = node.Element("DefName").Value,
                    command = node.Element("Command").Value,
                    condition = node.Element("Condition").Value,
                    excuteType = ParseEnum<ExcuteType>(node.Element("ExcuteType").Value)
                });
        }


        protected void LoadData()
        {
            if (!File.Exists(FilePath)) return;
            try
            {
                var doc = XDocument.Load(FilePath);
                LoadFromXml(doc);
            }
            catch (Exception ex)
            {
                Log.Warning("Exception loading xml from " + FilePath + ". Loading defaults instead. Exception was: " + ex);
            }
        }

        private void LoadData(object sender, FileSystemEventArgs e)
        {
            PacketManager.LogQueue.Enqueue("XML 파일 변경");
            LoadData();
        }

        #region Singleton

        private static IncidentManager _instance;
        public static IncidentManager Instance => _instance ?? (_instance = new IncidentManager());

        #endregion
    }
}

//protected void WriteXml(XDocument xml)
//{
//    var root = new XElement("settings");
//    xml.Add(root);
//    foreach (var incident in incidents)
//    {
//        Log.Message(incident.Key);
//        root.Add(new XElement("Incident",
//            new XElement("DefName", incident.Key),
//            new XElement("Label", incident.Value.label),
//            new XElement("ExcuteType", incident.Value.excuteType),
//            new XElement("Condition", incident.Value.condition)
//        ));
//    }
//}

//protected void SaveData()
//{
//    try
//    {
//        var doc = new XDocument();
//        WriteXml(doc);
//        doc.Save(FilePath);
//    }
//    catch (Exception ex)
//    {
//        Log.Warning("Failed to save xml to " + FilePath + ". Exception was: " + ex);
//    }
//}