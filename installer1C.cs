/*
 * Created by SharpDevelop.
 * User: Alex
 * Date: 05.01.2014
 * Time: 16:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;

namespace installer1C
{
	/// <summary>
	/// Класс для хранения информации по конкретной версии платформы.
	/// </summary>
	class platformVersion
	{						
		public int version;		
		public string versionString;
		public string defaultStarter;
		public string thinStarter;
		
		public platformVersion()
		{		
		}
	}
	
	/// <summary>
	/// Класс-коллекция для различных версий
	/// </summary>
	class platformVersions: CollectionBase
	{
		public void Add(platformVersion version)
		{
			List.Add(version);
		}
		
		public void Remove(platformVersion version)
		{
			List.Remove(version);
		}
		
		public platformVersions()
		{
		
		}
		
		public platformVersion GetVersion(string versionString)
		{
			platformVersion result = null;
					
			int index = 0;
			while (index < List.Count && result == null)			
			{
				platformVersion version = (platformVersion)List[index];
				if (version.versionString == versionString)
				{
					result = version;				
				}
				index++;
			}
			
			return result;
		}
	}
	
	/// <summary>
	/// Класс, хранящий информации о конкретном типе платформы (8.1/8.2/и т.д), включая все версии платформы
	/// </summary>
	class platformInfo
	{
		public int platformType;
		public string starter;
		public platformVersions versions;
		
		public platformInfo()
		{
			versions = new platformVersions();
		}
		
		public platformVersion AddPlatformVersion(string versionString, string path)
		{
			if (!path.EndsWith(@"\"))
           	{
				path += @"\";                  
           	}
			
			platformVersion version = null;
						
			if (Directory.Exists(path))
			{
				version = new platformVersion();
				version.versionString = versionString;
				
							
				try
				{
					version.version = Convert.ToInt32(versionString.Replace(".", ""));
				}
				catch
				{
					version.version = -1;
				}
				
				if (File.Exists(path + "1cv8s.exe"))
				{
					version.defaultStarter = path + "1cv8s.exe";
				}
				else if (File.Exists(path + "1cv8.exe")) 
				{
					version.defaultStarter = path + "1cv8.exe";
				}
				
				if (File.Exists(path + "1cv8c.exe"))
				{
					version.thinStarter = path + "1cv8c.exe";
				}
				
				versions.Add(version);
			}
			
			return version;
		}
		
	}
	
	/// <summary>
	/// Класс, хранящий всю информацию об установленных платформах и их версиях
	/// </summary>
	public class installer1CInfo: CollectionBase
	{		
		private void Add(platformInfo platform)
		{
			List.Add(platform);
		}
		
		private void Remove(platformInfo platform)
		{
			List.Remove(platform);
		}
		
		private platformInfo AddPlatform(int platformType, string starter)
		{
			if (File.Exists(starter))
			{
				platformInfo platform = new platformInfo();
				platform.platformType = platformType;					
				platform.starter = starter;	
			
				Add(platform);
							
				return platform;
			}
			else
			{
				return null;
			}
		}
		
		private platformInfo GetPlatform(int platformType)
		{
			platformInfo result = null;
					
			int index = 0;
			while (index < List.Count && result == null)			
			{
				platformInfo platform = (platformInfo)List[index];
				if (platform.platformType == platformType)
				{
					result = platform;				
				}
				index++;
			}
			
			return result;
		}
		
		private void LoadPlatformFromRegister(List<string> v8Folders, int platformType, string starterName, string commonTemplate, string binTemplate)
		{
			// 8.x common
			Regex RegExp = new Regex(commonTemplate);
			string path = null;
			
			foreach (string KeyValue in v8Folders)
			{		
				path = KeyValue;
				
				if (RegExp.IsMatch(KeyValue))
				{
					platformInfo platform = GetPlatform(platformType);
					
					if (!path.EndsWith(@"\"))
		           	{
						path += @"\";                  
		           	}
					
					if (platform == null)
					{
						AddPlatform(platformType, path + starterName);
					}
					else
					{
						platform.starter = path + starterName;
					}			
				}
			}
			
			if (!String.IsNullOrEmpty(binTemplate))
			{
				// 8.x bin
				RegExp = new Regex(binTemplate);
				
				foreach (string KeyValue in v8Folders)
				{	
					path = KeyValue;
					
					MatchCollection Matches = RegExp.Matches(KeyValue);
					if (Matches.Count > 0)
					{
						
						if (!path.EndsWith(@"\"))
			           	{
							path += @"\";                  
			           	}
						
						platformInfo platform = GetPlatform(platformType);
						
						if (platform != null)
						{
							string stringVersion = Matches[0].Groups["version"].Value;
							if (platform.versions.GetVersion(stringVersion) == null)
							{
								platform.AddPlatformVersion(stringVersion, path);
							}
						}
					}
				}
			}			
			RegExp = null;
		}
		
		private void LoadPlatformFromPath8(string folder, string subfolder, string platformTypeStr, int platformType)
		{						
			if (Directory.Exists(folder + subfolder))
			{
				if (Directory.Exists(folder + subfolder + @"\common"))
				{
					platformInfo platform = GetPlatform(platformType);	
					if (platform == null)
					{
						platform = AddPlatform(platformType, folder + subfolder + @"\common\1cestart.exe");						
					}
					
					if (platform != null)
					{
						Regex RegExp = new Regex(@".*\\(?<version>(" + platformTypeStr + @"[.\d]*))$");
						
						foreach (string childFolder in Directory.GetDirectories(folder + subfolder))						
						{							
							MatchCollection Matches = RegExp.Matches(childFolder);
							if (Matches.Count > 0)
							{
								string stringVersion = Matches[0].Groups["version"].Value;
								if (platform.versions.GetVersion(stringVersion) == null)
								{
									platform.AddPlatformVersion(stringVersion, childFolder + @"\bin\");
								}
							}
						}
						
						RegExp = null;
					}
				}
				else
				{
					platformInfo platform = GetPlatform(platformType);	
					if (platform == null)
					{
						AddPlatform(platformType, folder + subfolder + @"\bin\1cv8.exe");
					}					
				}
			}			
		}
		
		private void LoadPlatformFromPath7(string folder)
		{			
			
			string[] folders = new string[3];
			folders[0] = "1Cv77";
			folders[1] = "1Cv77S";
			folders[2] = "1Cv77.ADM";
			
			foreach (string subfolder in folders)
			{
			
				if (Directory.Exists(folder + subfolder))
				{
					if (Directory.Exists(folder + subfolder + @"\bin"))
					{
						
						string[] startFiles = new string[3];
						startFiles[0] = "1cv7s.exe";
						startFiles[1] = "1cv7.exe";
						startFiles[2] = "1cv7l.exe";
						
						foreach (string startFile in startFiles)
						{
						
							if (File.Exists(folder + subfolder + @"\bin\" + startFile))
							{
							
								platformInfo platform = GetPlatform(77);
								if (platform == null)
								{
									platform = AddPlatform(77, folder + subfolder + @"\bin\" + startFile);						
								}																
							}
						}
					}
				}
			}
		}
				
		public string GetStarter(int platformType, string platformVer = "", bool thinClient = false)
		{
			string result = "";
			
			platformInfo platform = GetPlatform(platformType);
			
			if (platform != null)
			{
				result = platform.starter;
				
				if (!String.IsNullOrEmpty(platformVer))
				{
					platformVersion version = platform.versions.GetVersion(platformVer);
					
					if (version != null)
					{

						if (thinClient)
						{
							result = version.thinStarter;	
						}
						else
						{
							result = version.defaultStarter;
						}
						
					}
				}
			}
			
			return result;
		}
		
		private platformVersion GetLastVersion(int platformType)
		{
			platformVersion lastVersion = null;
			
			platformInfo platform = GetPlatform(platformType);
			
			if (platform != null)
			{				
				int maxVersion = 0;					
				
				foreach (platformVersion version in platform.versions)
				{
					if (version.version > maxVersion)
					{
						maxVersion = version.version;
						lastVersion = version; 
					}
				}					
			}
			
			return lastVersion;
		}
		
		public string GetStarterForLastVersion(int platformType, bool thinClient = false)
		{
			string result = "";
			
			platformInfo platform = GetPlatform(platformType);
			
			if (platform != null)
			{
				result = platform.starter;
				
				platformVersion lastVersion = GetLastVersion(platformType);
				
				if (lastVersion != null)
				{
					if (thinClient)
					{
						result = lastVersion.thinStarter;	
					}
					else
					{
						result = lastVersion.defaultStarter;
					}					
				}
			}
			
			return result;
		}
		
		public List<string> GetPlatformVersions(int platformType)
		{
			List<string> result = new List<string>();
			
			platformInfo platform = GetPlatform(platformType);
			
			if (platform != null)
			{
				foreach (platformVersion version in platform.versions)
				{
					result.Add(version.versionString);
				}
			}
			
			return result;
		}
		
		private string GetProgramFiles()
        {
            string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            
            if (!ProgramFiles.EndsWith(@"\"))
			{
				ProgramFiles += @"\";
			}
            
            return ProgramFiles;            
        }
		
		private List<string> Getv8Folders(string[] folders)
		{
			List<string> result = new List<string>();
			Regex RegExp = new Regex(@".?\\1cv8.");
				
			foreach (string folder in folders)
			{
				if (RegExp.IsMatch(folder))
				{
					result.Add(folder);
				}
			}
			
			return result;
		}

        private RegistryKey GetRegistryBase()
        {
            /*RegistryKey registryBase;                        
            switch (IntPtr.Size)
            {                
                case 4:
                    // Use the 64-bit registry 
                    registryBase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    break;

                case 8:
                    // Use the 32-bit (WOW) registry
                    registryBase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    break;

                default:
                    // Use the default bitness
                    registryBase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                    break;
                }

            return registryBase;*/
            return Registry.LocalMachine;
        }
        
        public void Refresh()
        { 
        	
        	List.Clear();
        	
        	RegistryKey RegistryBase = GetRegistryBase();
            RegistryKey InstallerFolders = RegistryBase.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Installer\Folders", false);

            if (InstallerFolders != null)
            {
                string[] folders = InstallerFolders.GetValueNames();

                List<string> v8Folders = Getv8Folders(folders);

                // 8.1
                LoadPlatformFromRegister(v8Folders, 81, "1cv8.exe", @".*(1cv81)\\bin\\?$", null);
                // 8.2
                LoadPlatformFromRegister(v8Folders, 82, "1cestart.exe", @".*(1cv82)\\common\\?$", @".*1cv82\\(?<version>(.*?))\\bin\\?$");
                // 8.3
                LoadPlatformFromRegister(v8Folders, 83, "1cestart.exe", @".*(1cv8)\\common\\?$", @".*1cv8\\(?<version>(8\.3\..*?))\\bin\\?$");

                InstallerFolders.Close();
            }
            						
			string ProgramFiles = GetProgramFiles();			
			
			LoadPlatformFromPath7(ProgramFiles);
			LoadPlatformFromPath8(ProgramFiles, "1cv81", "8.1", 81);
			LoadPlatformFromPath8(ProgramFiles, "1cv82", "8.2", 82);
			LoadPlatformFromPath8(ProgramFiles, "1cv8", "8.3", 83);
			
        }
        		
		public installer1CInfo()
		{          
			Refresh();			
		}
				
	}
}
