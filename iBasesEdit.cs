using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;

namespace TestMenuPopup
{
	public class BaseGUID
	{
		private string path;
		private string guid;
		
		public BaseGUID()
		{
			
		}
		
		public BaseGUID(string path, string guid)
		{
			Path = path;
			GUID = guid;
		}
		
		public string Path
		{
			set
			{
				path = value;  
			}
			get
			{
				return path;
			}
		}
		
		public string GUID
		{
			set
			{
				guid = value;	
			}
			get
			{
				return guid;
			}
		}
	}
	
    static class iBasesEdit
    {
    	private static List<BaseGUID> GUIDs;

		private static void LoadGUIDS()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<BaseGUID>));

                if (!File.Exists("GUIDS.cfg"))
                {
                    using (TextWriter writer = new StreamWriter("GUIDS.cfg"))
                    {
                    	serializer.Serialize(writer, new List<BaseGUID>());
                    }
                }
                using (FileStream fs = new FileStream("GUIDS.cfg", FileMode.Open))
                {
                    GUIDs = (List<BaseGUID>)serializer.Deserialize(fs);                    
                }
            }
            catch
            {
            	GUIDs = null;
            }

        }  

		private static string GetBaseGUID(string connectionString)
		{
			LoadGUIDS();
			
			string result = null;
			
			if (GUIDs != null)
			{
				foreach(BaseGUID info in GUIDs)
				{
					if (info.Path == connectionString)
					{
						result = info.GUID;
						break;
					}
				}
			}
			
			return result;
		}
		
		private static void SaveBaseGUID(string connectionString, string GUID)
		{
			if (GUIDs == null)
			{
				GUIDs = new List<BaseGUID>();
			}
			
			GUIDs.Add(new BaseGUID(connectionString, GUID));
			
			XmlSerializer serializer = new XmlSerializer(typeof(List<BaseGUID>));
            
            using (TextWriter writer = new StreamWriter("GUIDS.cfg"))
            {
            	serializer.Serialize(writer, GUIDs);
            }             
		}
    	
        struct StructBaseParam
        {
            public string Name;
            public string Value;
            public int Line;
        }

        struct StructBaseInfo
        {
            public string Name;
        	public int StartLine;
            public int EndLine;
            public Dictionary<string, StructBaseParam> Parameters;
        }

        private static Dictionary<string, StructBaseInfo> v8i_info;
        private static string v8i_file;

        private static string GetFileV8I()
        {
            string AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (!String.IsNullOrEmpty(AppFolder))
            {

                if (!AppFolder.EndsWith(@"\"))
                    AppFolder += @"\";

                string v8i_name = AppFolder + @"1C\1CEStart\ibases.v8i";

                if (File.Exists(v8i_name))
                    return v8i_name;
                else
                {
                    if (Directory.Exists(AppFolder + @"1C"))
                    {
                        if (!Directory.Exists(AppFolder + @"1C\1CEStart\"))
                        {
                            try
                            {
                                Directory.CreateDirectory(AppFolder + @"1C\1CEStart\");
                            }
                            catch
                            {
                                return "";
                            }
                        }

                        try
                        {
                            FileStream fsFile = File.Create(v8i_name);
                            fsFile.Close();
                            fsFile.Dispose();
                            return v8i_name;
                        }
                        catch
                        {
                            return "";
                        }

                    }
                    else
                        return "";
                }
            }

            return "";
        }

        private static StructBaseParam ReadParameter(string strLine, int LineNumber)
        {
            StructBaseParam BaseParam = new StructBaseParam();
            BaseParam.Line = LineNumber;

            int EqualPos = strLine.IndexOf('=');

            if (EqualPos != -1)
            {
                BaseParam.Name = strLine.Substring(0, EqualPos);
                BaseParam.Value = strLine.Substring(EqualPos + 1);
            }

            return BaseParam;
        }

        private static string GetUniConnectionString(string ConnectionString)
        {
            ConnectionString = ConnectionString.ToLower();
            ConnectionString = ConnectionString.Replace(@";", "");
            ConnectionString = ConnectionString.Replace(@"\", "");

            return ConnectionString;
        }

        public static bool Read_v8i()
        {
            bool Result = false;

            v8i_info = new Dictionary<string, iBasesEdit.StructBaseInfo>();

            v8i_file = GetFileV8I();

            if (!String.IsNullOrEmpty(v8i_file))
            {

                try
                {
                    FileStream fsFile = new FileStream(v8i_file, FileMode.Open);
                    StreamReader rdFile = new StreamReader(fsFile);

                    string strLine;
                    string ConnectionString = null;
                    StructBaseInfo BaseInfo = new StructBaseInfo();

                    int LineNumber = 1;

                    strLine = rdFile.ReadLine();
                    while (strLine != null)
                    {
                        strLine = strLine.Trim();

                        if (!strLine.StartsWith("//"))
                        {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                            {
                                if (!String.IsNullOrEmpty(ConnectionString))
                                {

                                    BaseInfo.EndLine = LineNumber - 1;

                                    if (!v8i_info.ContainsKey(ConnectionString))
                                        v8i_info.Add(ConnectionString, BaseInfo);

                                    ConnectionString = "";

                                }

                                BaseInfo = new StructBaseInfo();
                                BaseInfo.StartLine = LineNumber;
                                BaseInfo.Name = strLine.Substring(1, strLine.Length - 2);
                                BaseInfo.Parameters = new Dictionary<string, StructBaseParam>();

                            }
                            else
                            {
                                if (strLine.ToLower().StartsWith("connect="))
                                {
                                    ConnectionString = GetUniConnectionString(strLine);
                                }

                                StructBaseParam BaseParam = ReadParameter(strLine, LineNumber);
                                string ParamName = BaseParam.Name.ToLower();

                                if (!BaseInfo.Parameters.ContainsKey(ParamName))
                                    BaseInfo.Parameters.Add(ParamName, BaseParam);
                            }
                        }

                        strLine = rdFile.ReadLine();
                        LineNumber++;
                    }


                    if (LineNumber > 1)
                    {
                        BaseInfo.EndLine = LineNumber - 1;
                        if (!v8i_info.ContainsKey(ConnectionString))
                            v8i_info.Add(ConnectionString, BaseInfo);
                    }

                    rdFile.Close();
                    rdFile.Dispose();

                    fsFile.Close();
                    fsFile.Dispose();

                    Result = true;

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: {0}", e.ToString());
                }

            }

            return Result;

        }


        private static string CreateConnectionString(int BaseType, string BasePath)
        {
            string ConnectionString = "";

            switch (BaseType)
            {
                case 0:
                    ConnectionString = "Connect=File=\"" + BasePath + "\";";
                    break;
                case 1:
                    string[] ConnectArray = BasePath.Split('/');
                    ConnectionString = "Connect=Srvr=\"" + ConnectArray[0] + "\";Ref=\"";
                    if (ConnectArray.Length > 1)
                        ConnectionString += ConnectArray[1];
                    ConnectionString += "\";";
                    break;
            }

            return ConnectionString;
        }

        public static bool BaseExists(int BaseType, string BasePath)
        {
            string ConnectionString = CreateConnectionString(BaseType, BasePath);
            return v8i_info.ContainsKey(GetUniConnectionString(ConnectionString));
        }

        public static string GetParameterValue(int BaseType, string BasePath, string ParamName)
        {
            string Result = null;
            StructBaseInfo BaseInfo;
            StructBaseParam BaseParam;

            string ConnectionString = CreateConnectionString(BaseType, BasePath);

            if (v8i_info.TryGetValue(GetUniConnectionString(ConnectionString), out BaseInfo))
            {

                if (BaseInfo.Parameters.TryGetValue(ParamName.ToLower(), out BaseParam))
                {
                    Result = BaseParam.Value;
                }

            }

            return Result;
        }

        private static string[] GetFileStrings()
        {
            try
            {
                return File.ReadAllLines(v8i_file);
            }
            catch
            {
                return null;
            }
        }

        public static bool SetParameterValue(int BaseType, string BasePath, string ParamName, string ParamValue)
        {
            bool Result = false;
            StructBaseInfo BaseInfo;
            StructBaseParam BaseParam;

            string ConnectionString = CreateConnectionString(BaseType, BasePath);

            if (v8i_info.TryGetValue(GetUniConnectionString(ConnectionString), out BaseInfo))
            {

                if (BaseInfo.Parameters.TryGetValue(ParamName.ToLower(), out BaseParam))
                {
                    try
                    {
                        string[] FileStrings = File.ReadAllLines(v8i_file);
                        FileStrings[BaseParam.Line - 1] = ParamName + "=" + ParamValue;
                        File.WriteAllLines(v8i_file, FileStrings);

                        Result = iBasesEdit.Read_v8i();

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        Result = false;
                    }
                }

            }

            return Result;
        }

        public static bool CreateNewBase(int BaseType, string BasePath, string Version, string App, bool WinAutentification = false)
        {
        	
            bool Result = false;

            string ConnectionString = CreateConnectionString(BaseType, BasePath);

            try
            {
                string[] FileStrings = File.ReadAllLines(v8i_file);
                List<string> FileBuilder = new List<string>();

                foreach (string strLine in FileStrings)
                {
                    FileBuilder.Add(strLine);
                }
				
                string NewGUID = GetBaseGUID(ConnectionString);
                if (String.IsNullOrEmpty(NewGUID))
                {
                	NewGUID = Convert.ToString(Guid.NewGuid());
                	SaveBaseGUID(ConnectionString, NewGUID);
                }
                                
                FileBuilder.Add("[HotTray1C_TempDB_" + NewGUID + "]");
                FileBuilder.Add(ConnectionString);
                FileBuilder.Add("ID=" + NewGUID);
                FileBuilder.Add("OrderInList=100");
                FileBuilder.Add("Folder=/");
                FileBuilder.Add("OrderInTree=100");
                FileBuilder.Add("External=0");
                FileBuilder.Add("ClientConnectionSpeed=Normal");
                FileBuilder.Add("App=" + App);
                FileBuilder.Add("WA=" + ((WinAutentification) ? 1 : 0));
                FileBuilder.Add("Version=" + Version);                

                FileStrings = new string[FileBuilder.Count];
                FileBuilder.CopyTo(FileStrings);

                File.WriteAllLines(v8i_file, FileStrings);

                Result = iBasesEdit.Read_v8i();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Result = false;
            }

            return Result;
        }

        public static bool DeleteBaseFromFile(int BaseType, string BasePath)
        {

            bool Result = false;
            StructBaseInfo BaseInfo;

            string ConnectionString = CreateConnectionString(BaseType, BasePath);

            if (v8i_info.TryGetValue(GetUniConnectionString(ConnectionString), out BaseInfo))
            {

                try
                {
                    string[] FileStrings = File.ReadAllLines(v8i_file);

                    List<string> FileBuilder = new List<string>();

                    int LineIndex = 0;

                    foreach (string strLine in FileStrings)
                    {
                        if (LineIndex + 1 < BaseInfo.StartLine || LineIndex + 1 > BaseInfo.EndLine)
                            FileBuilder.Add(strLine);
                        LineIndex++;
                    }

                    FileStrings = new string[FileBuilder.Count];
                    FileBuilder.CopyTo(FileStrings);

                    File.WriteAllLines(v8i_file, FileStrings);

                    Result = iBasesEdit.Read_v8i();

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Result = false;
                }
            }

            return Result;
        }
        
        public static bool DeleteTempBasesFromFile()
        {
        	bool Result = false; 
        	
			try
            {                
				string[] FileStrings = File.ReadAllLines(v8i_file);

                List<string> FileBuilder = new List<string>();
                List<int> IndexForDel = new List<int>();
                

                foreach (StructBaseInfo BaseInfo in v8i_info.Values)
                {
                	if (BaseInfo.Name.StartsWith("HotTray1C_TempDB_"))
                	{                        
                        for(int LineIndex = BaseInfo.StartLine - 1; LineIndex <= (BaseInfo.EndLine - 1); LineIndex++)
                		{
                			IndexForDel.Add(LineIndex);
                		}                		
                	}
                }
                
                int CurentIndex = 0;
                
                foreach (string strLine in FileStrings)
                {
                	if (!IndexForDel.Contains(CurentIndex))
                		FileBuilder.Add(strLine);
                	CurentIndex++;
                }

                FileStrings = new string[FileBuilder.Count];
                FileBuilder.CopyTo(FileStrings);

                File.WriteAllLines(v8i_file, FileStrings);

                Result = iBasesEdit.Read_v8i();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Result = false;
            }            

            return Result;
        }

    }
}
