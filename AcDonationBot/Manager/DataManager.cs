using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using RimWorld;
using Verse;

namespace AlcoholV.Manager
{
    public enum ExcuteType
    {
        INSTANT,
        STACK,
        COOL
    }

    public enum EventType
    {
        DISEASE,
        FRIENDLY,
        HOSTILE,
        NATURAL,
        WEATHER,
        CARAVAN
    }

    public class CustomIncident
    {
        public string command = "";
        public int condition;
        public IncidentDef def;
        public EventType eventType;
        public ExcuteType excuteType = ExcuteType.INSTANT;
    }

    // Singleton
    public static class DataManager
    {
        private const string FolderPath = @"c:\ChatApp\";
        private const string FileName = @"Event.xml";
        private const string FilePath = @"c:\ChatApp\Event.xml";
        public static readonly List<CustomIncident> Datas = new List<CustomIncident>();

        public static List<CustomIncident> FindAll(int condition)
        {
            // 조건 만족하는 이벤트 모두 선택
            return Datas.FindAll(x => x.condition >= condition);
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

        public static void Init()
        {
            LoadData();
            //Log.MessageModel("Loaded Data File : " + Datas.Count);
            //foreach (var customIncident in Datas)
            //{
            //    Log.MessageModel(customIncident.command);
            //}

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

        private static void LoadFromXml(XDocument xml)
        {
            Datas.Clear(); // 불러오기 전에 클리어
            foreach (var node in xml.Root.Elements("Incident"))
                Datas.Add(new CustomIncident
                {
                    def = IncidentDef.Named(node.Element("DefName").Value),
                    command = node.Element("Command").Value,
                    condition = Convert.ToInt32(node.Element("Condition").Value),
                    excuteType = ParseEnum<ExcuteType>(node.Element("ExcuteType").Value),
                    eventType = ParseEnum<EventType>(node.Element("EventType").Value)
                });
        }


        private static void LoadData()
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

        private static void LoadData(object sender, FileSystemEventArgs e)
        {
            //LogManager.Enqueue("XML 파일 변경");
            LoadData();
        }
    }
}