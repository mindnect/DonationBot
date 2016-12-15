using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Verse
{
	public static class XmlLoader
	{
		private static LoadableXmlAsset loadingAsset;

		[DebuggerHidden]
		public static IEnumerable<LoadableXmlAsset> XmlAssetsInModFolder(ModContentPack mod, string folderPath)
		{
			XmlLoader.<XmlAssetsInModFolder>c__Iterator1EA <XmlAssetsInModFolder>c__Iterator1EA = new XmlLoader.<XmlAssetsInModFolder>c__Iterator1EA();
			<XmlAssetsInModFolder>c__Iterator1EA.mod = mod;
			<XmlAssetsInModFolder>c__Iterator1EA.folderPath = folderPath;
			<XmlAssetsInModFolder>c__Iterator1EA.<$>mod = mod;
			<XmlAssetsInModFolder>c__Iterator1EA.<$>folderPath = folderPath;
			XmlLoader.<XmlAssetsInModFolder>c__Iterator1EA expr_23 = <XmlAssetsInModFolder>c__Iterator1EA;
			expr_23.$PC = -2;
			return expr_23;
		}

		[DebuggerHidden]
		public static IEnumerable<T> LoadXmlDataInResourcesFolder<T>(string folderPath) where T : new()
		{
			XmlLoader.<LoadXmlDataInResourcesFolder>c__Iterator1EB<T> <LoadXmlDataInResourcesFolder>c__Iterator1EB = new XmlLoader.<LoadXmlDataInResourcesFolder>c__Iterator1EB<T>();
			<LoadXmlDataInResourcesFolder>c__Iterator1EB.folderPath = folderPath;
			<LoadXmlDataInResourcesFolder>c__Iterator1EB.<$>folderPath = folderPath;
			XmlLoader.<LoadXmlDataInResourcesFolder>c__Iterator1EB<T> expr_15 = <LoadXmlDataInResourcesFolder>c__Iterator1EB;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static T ItemFromXmlFile<T>(string filePath, bool resolveCrossRefs = true) where T : new()
		{
			if (resolveCrossRefs && CrossRefLoader.LoadingInProgress)
			{
				Log.Error("Cannot call ItemFromXmlFile with resolveCrossRefs=true while loading is already in progress.");
			}
			FileInfo fileInfo = new FileInfo(filePath);
			if (!fileInfo.Exists)
			{
				return (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			}
			T result;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(File.ReadAllText(fileInfo.FullName));
				T t = XmlToObject.ObjectFromXml<T>(xmlDocument.DocumentElement, false);
				if (resolveCrossRefs)
				{
					CrossRefLoader.ResolveAllWantedCrossReferences(FailMode.LogErrors);
				}
				result = t;
			}
			catch (Exception ex)
			{
				Log.Error("Exception loading file at " + filePath + ". Loading defaults instead. Exception was: " + ex.ToString());
				result = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
			}
			return result;
		}

		[DebuggerHidden]
		public static IEnumerable<Def> AllDefsFromAsset(LoadableXmlAsset asset)
		{
			XmlLoader.<AllDefsFromAsset>c__Iterator1EC <AllDefsFromAsset>c__Iterator1EC = new XmlLoader.<AllDefsFromAsset>c__Iterator1EC();
			<AllDefsFromAsset>c__Iterator1EC.asset = asset;
			<AllDefsFromAsset>c__Iterator1EC.<$>asset = asset;
			XmlLoader.<AllDefsFromAsset>c__Iterator1EC expr_15 = <AllDefsFromAsset>c__Iterator1EC;
			expr_15.$PC = -2;
			return expr_15;
		}

		[DebuggerHidden]
		public static IEnumerable<T> AllGameItemsFromAsset<T>(LoadableXmlAsset asset) where T : new()
		{
			XmlLoader.<AllGameItemsFromAsset>c__Iterator1ED<T> <AllGameItemsFromAsset>c__Iterator1ED = new XmlLoader.<AllGameItemsFromAsset>c__Iterator1ED<T>();
			<AllGameItemsFromAsset>c__Iterator1ED.asset = asset;
			<AllGameItemsFromAsset>c__Iterator1ED.<$>asset = asset;
			XmlLoader.<AllGameItemsFromAsset>c__Iterator1ED<T> expr_15 = <AllGameItemsFromAsset>c__Iterator1ED;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
