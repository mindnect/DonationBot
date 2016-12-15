using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace JSAssist
{
	internal class Update
	{
		private string fileServerAddr = "http://js-almighty.com/jsassist/app_new/";

		private string fileServerSubDirectory = "data\\";

		public string localPath;

		private string updateInfoFile = "updateinfo.dat";

		private UpdateInfo latestUpdateInfo;

		public bool mainFileModified;

		public bool updateSuccess;

		public int version;

		public Update()
		{
			this.mainFileModified = false;
			this.version = Program.currentVersion;
			this.updateSuccess = false;
		}

		public void CheckLatestVersion()
		{
			if (!this.GetUpdateInfo(this.updateInfoFile))
			{
				this.OnUpdateFailed();
				return;
			}
			this.version = this.latestUpdateInfo.version;
			for (int i = 0; i < (int)this.latestUpdateInfo.listFile.Length; i++)
			{
				string str = this.latestUpdateInfo.listFile[i].filePath;
				string str1 = this.latestUpdateInfo.listFile[i].hash;
				if (this.IsFile(str))
				{
					int num = str.LastIndexOf('\\');
					string str2 = this.localPath;
					if (num != -1)
					{
						str2 = string.Concat(this.localPath, str.Substring(0, num), "\\");
					}
					string str3 = str.Substring(num + 1);
					if (str2 != "" && !Directory.Exists(str2))
					{
						Directory.CreateDirectory(str2);
					}
					bool flag = false;
					string str4 = string.Concat(str2, str3);
					if (!File.Exists(str4))
					{
						flag = true;
					}
					else if (str1 != this.GetHash(str4))
					{
						flag = true;
					}
					if (flag)
					{
						Console.WriteLine(string.Concat("download file : ", str));
						if (!this.Decompress(string.Concat(this.fileServerAddr, this.fileServerSubDirectory, str, ".jsa"), str4))
						{
							return;
						}
					}
				}
			}
			this.OnUpdateSuccess();
		}

		public bool Decompress(string fileAddress, string fileOutput)
		{
			bool flag;
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				DeflateStream deflateStream = new DeflateStream(new MemoryStream((new WebClient()).DownloadData(fileAddress)), CompressionMode.Decompress);
				deflateStream.CopyTo(memoryStream);
				deflateStream.Close();
			}
			catch
			{
				this.OnUpdateFailed();
				flag = false;
				return flag;
			}
			try
			{
				if (fileOutput.Contains("JSAssist.exe"))
				{
					fileOutput = string.Concat(fileOutput, ".bak");
					this.mainFileModified = true;
				}
				if (File.Exists(fileOutput))
				{
					File.Delete(fileOutput);
				}
				FileStream fileStream = File.Open(fileOutput, FileMode.Create);
				byte[] array = memoryStream.ToArray();
				fileStream.Write(array, 0, (int)array.Length);
				fileStream.Close();
				return true;
			}
			catch
			{
				this.OnUpdateFailed();
				flag = false;
			}
			return flag;
		}

		public string GetHash(string filePath)
		{
			MD5 mD5 = MD5.Create();
			Stream stream = File.OpenRead(filePath);
			byte[] numArray = mD5.ComputeHash(stream);
			stream.Close();
			string str = "";
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				str = string.Concat(str, numArray[i].ToString("X2"));
			}
			return str;
		}

		private bool GetUpdateInfo(string updateInfoFileName)
		{
			bool flag;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					byte[] numArray = webClient.DownloadData(string.Concat(this.fileServerAddr, updateInfoFileName));
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream memoryStream = new MemoryStream(numArray);
					binaryFormatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
					this.latestUpdateInfo = (UpdateInfo)binaryFormatter.Deserialize(memoryStream);
					memoryStream.Close();
				}
				return true;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		private bool IsFile(string filePath)
		{
			return true;
		}

		public void OnUpdateFailed()
		{
			this.updateSuccess = false;
		}

		public void OnUpdateSuccess()
		{
			this.updateSuccess = true;
		}
	}
}