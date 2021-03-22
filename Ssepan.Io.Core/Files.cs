using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Ssepan.Utility.Core;

namespace Ssepan.Io.Core
{
    public static class Files
    {
        /// <summary>
        /// Read file into byte array.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Boolean Read
        (
            String filePath, 
            ref Byte[] bytes
        )
        {
            Boolean returnValue = default(Boolean);
            FileStream fileStream = default(FileStream);
            BinaryReader binaryReader = default(BinaryReader);

            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                binaryReader = new BinaryReader(fileStream);
                bytes = binaryReader.ReadBytes((Int32)fileStream.Length);

                returnValue = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Close();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Write byte array to file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Boolean Write
        (
            String filePath, 
            Byte[] bytes
        )
        {
            Boolean returnValue = default(Boolean);
            FileStream fileStream = default(FileStream);
            BinaryWriter binaryWriter = default(BinaryWriter);

            try
            {
                fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                binaryWriter = new BinaryWriter(fileStream);
                binaryWriter.Write(bytes);

                returnValue = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
                    
            }
            finally
            {
                if (binaryWriter != null)
                {
                    binaryWriter.Close();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return returnValue;
        }

        /// <summary>
        /// write content to output file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <param name="errorMessage"></param>
        public static void WriteOutputFile
        (
            String filePath,
            String content,
            ref String errorMessage
        )
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.Write(content);
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
            }
        }
    }
}
