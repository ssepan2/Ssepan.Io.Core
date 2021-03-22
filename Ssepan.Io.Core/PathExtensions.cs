using System;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace Ssepan.Io.Core
{
    public static class PathExtensions
    {
        /// <summary>
        /// When given a path, append trailing directory separator, 
        ///  so that Path.GetFileName and Path.GetDirectoryName do not mistake last directory for a file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String WithTrailingSeparator(this String path)
        {
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path;
            }
            else
            {
                return Path.Combine(path, " ").Trim();
            }
        }
    }
}
