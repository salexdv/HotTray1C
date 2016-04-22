using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace TestMenuPopup
{
    abstract class DBUsers
    {        
        [ComImport]
        [Guid("0000000d-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumSTATSTG
        {
            // The user needs to allocate an STATSTG array whose size is celt. 
            [PreserveSig]
            uint Next(
                uint celt,
                [MarshalAs(UnmanagedType.LPArray), Out] 
            System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt,
                out uint pceltFetched
            );
            void Skip(uint celt);
            void Reset();
            [return: MarshalAs(UnmanagedType.Interface)]
            IEnumSTATSTG Clone();
        }
        [ComImport]
        [Guid("0000000b-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IStorage
        {
            void CreateStream(
                /* [string][in] */ string pwcsName,
                /* [in] */ uint grfMode,
                /* [in] */ uint reserved1,
                /* [in] */ uint reserved2,
                /* [out] */ out IStream ppstm);
            void OpenStream(
                /* [string][in] */ string pwcsName,
                /* [unique][in] */ IntPtr reserved1,
                /* [in] */ uint grfMode,
                /* [in] */ uint reserved2,
                /* [out] */ out IStream ppstm);
            void CreateStorage(
                /* [string][in] */ string pwcsName,
                /* [in] */ uint grfMode,
                /* [in] */ uint reserved1,
                /* [in] */ uint reserved2,
                /* [out] */ out IStorage ppstg);
            void OpenStorage(
                /* [string][unique][in] */ string pwcsName,
                /* [unique][in] */ IStorage pstgPriority,
                /* [in] */ uint grfMode,
                /* [unique][in] */ IntPtr snbExclude,
                /* [in] */ uint reserved,
                /* [out] */ out IStorage ppstg);
            void CopyTo(
                /* [in] */ uint ciidExclude,
                /* [size_is][unique][in] */ Guid rgiidExclude, // should this be an array? 
                /* [unique][in] */ IntPtr snbExclude,
                /* [unique][in] */ IStorage pstgDest);
            void MoveElementTo(
                /* [string][in] */ string pwcsName,
                /* [unique][in] */ IStorage pstgDest,
                /* [string][in] */ string pwcsNewName,
                /* [in] */ uint grfFlags);
            void Commit(
                /* [in] */ uint grfCommitFlags);
            void Revert();
            void EnumElements(
                /* [in] */ uint reserved1,
                /* [size_is][unique][in] */ IntPtr reserved2,
                /* [in] */ uint reserved3,
                /* [out] */ out IEnumSTATSTG ppenum);
            void DestroyElement(
                /* [string][in] */ string pwcsName);
            void RenameElement(
                /* [string][in] */ string pwcsOldName,
                /* [string][in] */ string pwcsNewName);
            void SetElementTimes(
                /* [string][unique][in] */ string pwcsName,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pctime,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME patime,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
            void SetClass(
                /* [in] */ Guid clsid);
            void SetStateBits(
                /* [in] */ uint grfStateBits,
                /* [in] */ uint grfMask);
            void Stat(
                /* [out] */ out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg,
                /* [in] */ uint grfStatFlag);
        }
        [Flags]
        public enum STGM : int
        {
            DIRECT = 0x00000000,
            TRANSACTED = 0x00010000,
            SIMPLE = 0x08000000,
            READ = 0x00000000,
            WRITE = 0x00000001,
            READWRITE = 0x00000002,
            SHARE_DENY_NONE = 0x00000040,
            SHARE_DENY_READ = 0x00000030,
            SHARE_DENY_WRITE = 0x00000020,
            SHARE_EXCLUSIVE = 0x00000010,
            PRIORITY = 0x00040000,
            DELETEONRELEASE = 0x04000000,
            NOSCRATCH = 0x00100000,
            CREATE = 0x00001000,
            CONVERT = 0x00020000,
            FAILIFTHERE = 0x00000000,
            NOSNAPSHOT = 0x00200000,
            DIRECT_SWMR = 0x00400000,
        }
        public enum STATFLAG : uint
        {
            STATFLAG_DEFAULT = 0,
            STATFLAG_NONAME = 1,
            STATFLAG_NOOPEN = 2
        }
        public enum STGTY : int
        {
            STGTY_STORAGE = 1,
            STGTY_STREAM = 2,
            STGTY_LOCKBYTES = 3,
            STGTY_PROPERTY = 4
        }

        [DllImport("ole32.dll")]
        private static extern int StgIsStorageFile(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName);
        [DllImport("ole32.dll")]
        static extern int StgOpenStorage(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            IStorage pstgPriority,
            STGM grfMode,
            IntPtr snbExclude,
            uint reserved,
            out IStorage ppstgOpen);

        [StructLayout(LayoutKind.Sequential)]
        public struct OVERLAPPED
        {
            public uint internalLow;
            public uint internalHigh;
            public uint offsetLow;
            public uint offsetHigh;
            public IntPtr hEvent;
        }

        [DllImport("kernel32.dll")]
        static extern bool LockFile(IntPtr hFile, uint dwFileOffsetLow, uint
           dwFileOffsetHigh, uint nNumberOfBytesToLockLow, uint
           nNumberOfBytesToLockHigh);

        [DllImport("kernel32.dll")]
        static extern bool UnlockFile(IntPtr hFile, uint dwFileOffsetLow,
           uint dwFileOffsetHigh, uint nNumberOfBytesToUnlockLow,
           uint nNumberOfBytesToUnlockHigh);

        [Flags]
        public enum EFileAccess : uint
        {
            /// <summary>
            /// 
            /// </summary>
            GenericRead = 0x80000000,
            /// <summary>
            /// 
            /// </summary>
            GenericWrite = 0x40000000,
            /// <summary>
            /// 
            /// </summary>
            GenericExecute = 0x20000000,
            /// <summary>
            /// 
            /// </summary>
            GenericAll = 0x10000000
        }

        [Flags]
        public enum EFileShare : uint
        {
            /// <summary>
            /// 
            /// </summary>
            None = 0x00000000,
            /// <summary>
            /// Enables subsequent open operations on an object to request read access. 
            /// Otherwise, other processes cannot open the object if they request read access. 
            /// If this flag is not specified, but the object has been opened for read access, the function fails.
            /// </summary>
            Read = 0x00000001,
            /// <summary>
            /// Enables subsequent open operations on an object to request write access. 
            /// Otherwise, other processes cannot open the object if they request write access. 
            /// If this flag is not specified, but the object has been opened for write access, the function fails.
            /// </summary>
            Write = 0x00000002,
            /// <summary>
            /// Enables subsequent open operations on an object to request delete access. 
            /// Otherwise, other processes cannot open the object if they request delete access.
            /// If this flag is not specified, but the object has been opened for delete access, the function fails.
            /// </summary>
            Delete = 0x00000004
        }

        public enum ECreationDisposition : uint
        {
            /// <summary>
            /// Creates a new file. The function fails if a specified file exists.
            /// </summary>
            New = 1,
            /// <summary>
            /// Creates a new file, always. 
            /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes, 
            /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
            /// </summary>
            CreateAlways = 2,
            /// <summary>
            /// Opens a file. The function fails if the file does not exist. 
            /// </summary>
            OpenExisting = 3,
            /// <summary>
            /// Opens a file, always. 
            /// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
            /// </summary>
            OpenAlways = 4,
            /// <summary>
            /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
            /// The calling process must open the file with the GENERIC_WRITE access right. 
            /// </summary>
            TruncateExisting = 5
        }

        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
           string lpFileName,
           EFileAccess dwDesiredAccess,
           EFileShare dwShareMode,
           IntPtr lpSecurityAttributes,
           ECreationDisposition dwCreationDisposition,
           EFileAttributes dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        static extern uint GetFileSize(IntPtr hFile, IntPtr lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer,
           uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        public struct StringMonitor
        {
            public string UserName;
            public string Mono;
            public string Date;
            public string RunMode;
            public string ComputerName;
        }

        public static List<string> GetUsers(string filename)
        {            
            List<string> users = new List<string>();
            if (StgIsStorageFile(filename) == 0)
            {

                IStorage storage = null;
                if (StgOpenStorage(
                    filename,
                    null,
                    STGM.DIRECT | STGM.READ | STGM.SHARE_EXCLUSIVE,
                    IntPtr.Zero,
                    0,
                    out storage) == 0)
                {
                    System.Runtime.InteropServices.ComTypes.STATSTG statstg;
                    storage.Stat(out statstg, (uint)STATFLAG.STATFLAG_DEFAULT);
                    IEnumSTATSTG pIEnumStatStg = null;
                    storage.EnumElements(0, IntPtr.Zero, 0, out pIEnumStatStg);
                    System.Runtime.InteropServices.ComTypes.STATSTG[] regelt = { statstg };
                    uint fetched = 0;
                    uint res = pIEnumStatStg.Next(1, regelt, out fetched);
                    if (res == 0)
                    {
                        while (res != 1)
                        {
                            string strNode = statstg.pwcsName;
                            bool bNodeFound = false;                            
                            if (strNode.IndexOf("Container.Contents") != -1)
                            {
                                bNodeFound = true;
                            }
                            if (bNodeFound)
                            {
                                switch (statstg.type)
                                {
                                    case (int)STGTY.STGTY_STORAGE:
                                        {
                                            IStorage pIChildStorage;
                                            storage.OpenStorage(statstg.pwcsName,
                                               null,
                                               (uint)(STGM.READ | STGM.SHARE_EXCLUSIVE),
                                               IntPtr.Zero,
                                               0,
                                               out pIChildStorage);
                                        }
                                        break;
                                    case (int)STGTY.STGTY_STREAM:
                                        {
                                            IStream pIStream;
                                            storage.OpenStream(statstg.pwcsName,
                                               IntPtr.Zero,
                                               (uint)(STGM.READ | STGM.SHARE_EXCLUSIVE),
                                               0,
                                               out pIStream);
                                            if (statstg.cbSize > 0)
                                            {
                                                byte[] data = new byte[statstg.cbSize];
                                                pIStream.Read(data, (int)statstg.cbSize, IntPtr.Zero);
                                                Encoding en = Encoding.Default;
                                                string tmpStr = en.GetString(data);

                                                int pos = tmpStr.IndexOf("Page");
                                                while (pos > 0)
                                                {
                                                    tmpStr = tmpStr.Remove(0, pos);
                                                    pos = tmpStr.IndexOf(",\"");
                                                    if (pos > 0)
                                                    {
                                                        tmpStr = tmpStr.Remove(0, pos + 2);
                                                        users.Add(tmpStr.Substring(0, tmpStr.IndexOf('"')));
                                                    }
                                                    pos = tmpStr.IndexOf("Page");
                                                }

                                            }
                                            pIStream = null;
                                        }
                                        break;
                                }
                            }
                            if ((res = pIEnumStatStg.Next(1, regelt, out fetched)) != 1)
                            {
                                statstg = regelt[0];
                            }
                        }
                        storage = null;
                    }
                }

            }
            return users;
        }

        public static List<StringMonitor> GetActiveUsers(string filename)
        {                        
            string FileStr = "";
            uint k = 0;

            List<StringMonitor> MonitorData = new List<StringMonitor>();

            IntPtr lngHandle = CreateFile(filename, EFileAccess.GenericRead | EFileAccess.GenericWrite, EFileShare.Read | EFileShare.Write, IntPtr.Zero, ECreationDisposition.OpenExisting, EFileAttributes.Normal, IntPtr.Zero);
            if (lngHandle != IntPtr.Zero && (lngHandle != (IntPtr)(-1)))
            {
                uint fsize = GetFileSize(lngHandle, IntPtr.Zero);
                while (fsize > 0)
                {
                    if (fsize > 4069)
                        k = 4096;
                    else
                        k = fsize;

                    byte[] buffer = new byte[k];
                    uint BytesRead = 0;
                    ReadFile(lngHandle, buffer, k, out BytesRead, IntPtr.Zero);
                    Encoding enc = Encoding.Default;
                    FileStr += enc.GetString(buffer);
                    fsize = fsize - BytesRead;
                }

                if (FileStr.Length > 0)
                    FileStr = FileStr.Substring(3, FileStr.Length - 3);

                bool fActiveUser = false;
                int n = -1;
                int pos = -1;
                int z = -1;

                string UserName = "";
                string KindStr = "";
                string Mono = "";
                string StartDate = "";
                string PCName = "";

                while (1 == 1)
                {
                    pos = FileStr.IndexOf(Convert.ToChar(13));

                    bool AllOK = false;
                    string TempStr = "";

                    if (pos > 0)
                    {
                        TempStr = FileStr.Substring(0, pos - 1);
                        FileStr = FileStr.Substring(pos + 2, FileStr.Length - pos - 2);
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(FileStr))
                            break;
                        TempStr = FileStr;
                        FileStr = "";
                    }


                    if (TempStr.Substring(0, 8) == "{\"Name\",")
                    {
                        n++;
                        if (!LockFile(lngHandle, (uint)(2000001 + n), 0, 1, 0))
                        {
                            fActiveUser = true;
                            TempStr = TempStr.Substring(9, TempStr.Length - 9);
                            z = TempStr.IndexOf('"');
                            UserName = TempStr.Substring(0, z);
                        }
                        else
                        {
                            UnlockFile(lngHandle, (uint)(2000001 + n), 0, 1, 0);
                            fActiveUser = false;
                        }
                    }
                    else if (TempStr.Substring(0, 13) == "{\"Run mode\",\"")
                    {
                        if (!fActiveUser)
                            continue;
                        KindStr = TempStr.Substring(13, 1);
                        continue;
                    }
                    else if (TempStr.Substring(0, 11) == "{\"IsMono\",\"")
                    {
                        if (!fActiveUser)
                            continue;
                        Mono = TempStr.Substring(11, 1);
                        continue;
                    }
                    else if (TempStr.Substring(0, 14) == "{\"Date&Time\",\"")
                    {
                        if (!fActiveUser)
                            continue;

                        TempStr = TempStr.Substring(14, TempStr.Length - 15);
                        z = TempStr.IndexOf('"');
                        StartDate = TempStr.Substring(0, z);
                        continue;
                    }
                    else if (TempStr.Substring(0, 17) == "{\"ComputerName\",\"")
                    {
                        if (!fActiveUser)
                            continue;

                        TempStr = TempStr.Substring(17, TempStr.Length - 18);
                        z = TempStr.IndexOf('"');
                        PCName = TempStr.Substring(0, z);
                        AllOK = true;
                    }

                    if (AllOK)
                    {
                        StringMonitor Data = new StringMonitor();
                        Data.UserName = UserName;
                        Data.RunMode = KindStr;
                        Data.Mono = Mono;
                        Data.Date = StartDate;
                        Data.ComputerName = PCName;
                        MonitorData.Add(Data);

                        AllOK = false;
                        UserName = "";
                        KindStr = "";
                        Mono = "";
                        StartDate = "";
                        PCName = "";
                    }

                    if (pos <= 0)
                        break;
                }

                CloseHandle(lngHandle);
            }


            return MonitorData;
        }

    }
}
