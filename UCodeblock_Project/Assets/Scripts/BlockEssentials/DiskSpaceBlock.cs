using UnityEngine;
using System.Collections;
using System.IO;

namespace UCodeblock
{
    public class DiskSpaceBlock : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<int>
    {
        public override string Content => "Free Disk Space";

        public int Evaluate(ICodeblockExecutionContext context)
        {
            return (int)GetTotalFreeSpace("C:");
        }

        public object EvaluateObject (ICodeblockExecutionContext context)
        {
            return Evaluate(context);
        }

        private long GetTotalFreeSpace(string driveName)
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