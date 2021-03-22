using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Ssepan.Utility.Core;

namespace Ssepan.Io.Core
{
    public class Folder
    {
        /// <summary>
        /// Copy file from source path to destination path.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Boolean CopyFile
        (
            String sourcePath,
            String destinationPath,
            ref String errorMessage
        )
        {
            Boolean returnValue = default(Boolean);

            try
            {
                //copy source to destination
                //Note: could use Files.Copy(), but this provides more options
                FileSystem.CopyFile
                (
                    sourcePath, //inputpath + docfolder(s)
                    destinationPath, //outputpath + "Images"
                    UIOption.AllDialogs,
                    UICancelOption.ThrowException
                );

                returnValue = true;
            }
            catch (OperationCanceledException ex)
            {
                errorMessage = ex.Message;

                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);

                //cancelled
                throw new Exception(String.Format("Copy cancelled: {0}", ex.Message));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);

            }

            return returnValue;
        }
        /// <summary>
        /// Delete and re-create folder
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="folder"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Boolean ReCreateFolder
        (
            String outputPath,
            String folder,
            ref String errorMessage
        )
        {
            Boolean returnValue = default(Boolean);
            String destinationPath = Path.Combine(outputPath, folder);

            try
            {
                //remove pre-existing documentsFolder
                if (Directory.Exists(destinationPath))
                {
                    //Directory.Delete(destinationPath, true);
                    //deal with locking/timing issues
                    DeleteFolderWithWait(destinationPath, 500);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);

                throw new Exception(String.Format(" Unable to delete '{0}' \n{1}", destinationPath, ex.Message));
            }

            try
            {
                //create docsfolder at destination
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format(" Unable to create '{0}' \n{1}", destinationPath, ex.Message));
            }

            returnValue = true;

            return returnValue;
        }


        /// <summary>
        /// Perform Directory.Delete() with a wait time to allow the system to catch up.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="waitMilliSeconds"></param>
        /// <param name="reCreate"></param>
        public static void DeleteFolderWithWait(String folderPath, Int32 waitMilliSeconds)
        {
            try
            {
                //check for folder and delete if present
                if (Directory.Exists(folderPath))
                {
                    try
                    {
                        Directory.Delete(folderPath, true);
                    }
                    catch (IOException)
                    {
                        //handle locks by explorer
                        Thread.Sleep(500);
                        Directory.Delete(folderPath, true);
                    }
                }

                //allow system to catch up before checking for folder *again*
                Thread.Sleep(waitMilliSeconds);//1 second delay fixes new-recreate bug
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);

                        
            }
        }
    }
}
