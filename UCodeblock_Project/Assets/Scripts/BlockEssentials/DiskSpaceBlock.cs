using UnityEngine;
using System.Collections;
using System.IO;

namespace UCodeblock
{
    public class DiskSpaceBlock : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<int>
    {
        public override string Content => "Free MB on C:\\";

        public int Evaluate(ICodeblockExecutionContext context)
        {
            long totalBytes = GetTotalFreeSpaceInBytes("C:\\");
            int megaBytes = Mathf.FloorToInt((totalBytes / 1024f) / 1024f);
            return megaBytes;
        }

        public object EvaluateObject (ICodeblockExecutionContext context)
        {
            return Evaluate(context);
        }

        private long GetTotalFreeSpaceInBytes(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }
    }
}