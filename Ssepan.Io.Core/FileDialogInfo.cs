using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using Ssepan.Utility.Core;

namespace Ssepan.Io.Core
{
    public class FileDialogInfo
    {
        public const String FilterSeparator = "|";
        public const String FilterFormat = "{0} (*.{1})|*.{1}";

        public FileDialogInfo()
        { }

        /// <summary>
        /// Use this constructor if you want to default the location to an Environment.SpecialFolder.
        /// </summary>
        /// <param name="newFilename"></param>
        /// <param name="filename"></param>
        /// <param name="title"></param>
        /// <param name="extension"></param>
        /// <param name="description"></param>
        /// <param name="typeName"></param>
        /// <param name="additionalFilters"></param>
        /// <param name="multiselect"></param>
        /// <param name="initialDirectory"></param>
        public FileDialogInfo
        (
            String newFilename,
            String filename,
            String title,
            String extension,
            String description,
            String typeName,
            String[] additionalFilters,
            Boolean multiselect,
            Environment.SpecialFolder initialDirectory
        )
        {
            NewFilename = newFilename;
            Filename = filename;
            Title = title;
            Extension = extension;
            Description = description;
            TypeName = typeName;
            //Join additionalFilters, create new array of those plus primary filter, then Join them again.
            Filters = String.Join(FilterSeparator, new String[] { String.Format(FilterFormat, description, extension), String.Join(FilterSeparator, additionalFilters) });
            Multiselect = multiselect;
            InitialDirectory = initialDirectory;
        }

        /// <summary>
        /// Use this constructor if you want to default the location to a custom location.
        /// Pass default(Environment.SpecialFolder) for InitialDirectory.
        /// </summary>
        /// <param name="newFilename"></param>
        /// <param name="filename"></param>
        /// <param name="title"></param>
        /// <param name="extension"></param>
        /// <param name="description"></param>
        /// <param name="typeName"></param>
        /// <param name="additionalFilters"></param>
        /// <param name="multiselect"></param>
        /// <param name="initialDirectory"></param>
        /// <param name="customInitialDirectory"></param>
        public FileDialogInfo
        (
            String newFilename,
            String filename,
            String title,
            String extension,
            String description,
            String typeName,
            String[] additionalFilters,
            Boolean multiselect,
            Environment.SpecialFolder initialDirectory,
            String customInitialDirectory
        ) : 
            this
            (
                newFilename,
                filename,
                title,
                extension,
                description,
                typeName,
                additionalFilters,
                multiselect,
                initialDirectory
            )
        {
            CustomInitialDirectory = customInitialDirectory;
        }

        private String _NewFilename = default(String);
        public String NewFilename
        {
            get { return _NewFilename; }
            set { _NewFilename = value; }
        }

        private String _Filename = default(String);
        public String Filename
        {
            get { return _Filename; }
            set { _Filename = value; }
        }

        private String[] _Filenames = default(String[]);
        public String[] Filenames
        {
            get { return _Filenames; }
            set { _Filenames = value; }
        }

        private Boolean _Multiselect = default(Boolean);
        public Boolean Multiselect
        {
            get { return _Multiselect; }
            set { _Multiselect = value; }
        }

        private String _Title = default(String);
        public String Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private String _Extension = default(String);
        public String Extension
        {
            get { return _Extension; }
            set { _Extension = value; }
        }

        private String _Description = default(String);
        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private String _TypeName = default(String);
        public String TypeName
        {
            get { return _TypeName; }
            set { _TypeName = value; }
        }

        private String _Filters = default(String);
        public String Filters
        {
            get { return _Filters; }
            set { _Filters = value; }
        }

        private Environment.SpecialFolder _InitialDirectory = default(Environment.SpecialFolder);
        public Environment.SpecialFolder InitialDirectory
        {
            get { return _InitialDirectory; }
            set { _InitialDirectory = value; }
        }

        private String _CustomInitialDirectory = default(String);
        public String CustomInitialDirectory
        {
            get { return _CustomInitialDirectory; }
            set { _CustomInitialDirectory = value; }
        }

        /// <summary>
        /// Get path to save data.
        /// </summary>
        /// <param name="fileDialogInfo"></param>
        /// <param name="forceDialog"></param>
        public static Boolean GetPathForSave(FileDialogInfo fileDialogInfo, Boolean forceDialog = true)
        {
            Boolean returnValue = default(Boolean);

            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    //if ((fileDialogInfo.Filename == fileDialogInfo.NewFilename) || (forceDialog))
                    if ((fileDialogInfo.Filename.EndsWith(fileDialogInfo.NewFilename)) || (forceDialog))
                    {
                        //define location of file for settings by prompting user for filename.
                        saveFileDialog.AddExtension = true;
                        saveFileDialog.AutoUpgradeEnabled = true;
                        saveFileDialog.CheckPathExists = true;
                        saveFileDialog.CreatePrompt = false;
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.ValidateNames = true;
                        saveFileDialog.ShowHelp = true;
                        saveFileDialog.DefaultExt = fileDialogInfo.Extension;
                        saveFileDialog.FileName = fileDialogInfo.Filename;
                        saveFileDialog.Filter = fileDialogInfo.Filters;
                        saveFileDialog.FilterIndex = 1;
                        saveFileDialog.InitialDirectory = (fileDialogInfo.InitialDirectory == default(Environment.SpecialFolder) ? fileDialogInfo.CustomInitialDirectory : Environment.GetFolderPath(Environment.SpecialFolder.Personal).WithTrailingSeparator());
                        //saveFileDialog.InitialDirectory = (fileDialogInfo.InitialDirectory == default(Environment.SpecialFolder) ? null : Environment.GetFolderPath(Environment.SpecialFolder.Personal).WithTrailingSeparator());

                        DialogResult dialogResult = saveFileDialog.ShowDialog();

                        switch (dialogResult)
                        {
                            case DialogResult.OK:
                                {
                                    if (Path.GetFileName(saveFileDialog.FileName).ToLower() == (fileDialogInfo.NewFilename + "." + fileDialogInfo.Extension).ToLower())
                                    {
                                        //user did not select or enter a name different than new; for now I have chosen not to allow that name to be used for a file.--SJS, 12/16/2005
                                        MessageBox.Show("The name \"" + (fileDialogInfo.NewFilename + "." + fileDialogInfo.Extension).ToLower() + "\" is not allowed; please choose another. Settings not saved.", fileDialogInfo.Title);
                                    }
                                    else
                                    {
                                        //set new filename
                                        fileDialogInfo.Filename = saveFileDialog.FileName;
                                        fileDialogInfo.Filenames = saveFileDialog.FileNames;

                                        returnValue = true;
                                    }

                                    break;
                                }
                            case DialogResult.Cancel:
                                {
                                    break;
                                }
                            default:
                                {
                                    throw new InvalidEnumArgumentException();
                                }
                        }
                    }
                    else
                    {
                        returnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
                    
                throw;
            }

            return returnValue;
        }

        /// <summary>
        /// Get path to load data.
        /// </summary>
        /// <param name="fileDialogInfo"></param>
        /// <param name="forceNew"></param>
        public static Boolean GetPathForLoad(FileDialogInfo fileDialogInfo, Boolean forceNew = false)
        {
            Boolean returnValue = default(Boolean);

            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (forceNew)
                    {
                        fileDialogInfo.Filename = fileDialogInfo.NewFilename;

                        returnValue = true;
                    }
                    else
                    {
                        String PreviousFileName = fileDialogInfo.Filename;

                        //define location of file for settings by prompting user for filename.
                        openFileDialog.AutoUpgradeEnabled = true;
                        openFileDialog.CheckFileExists = true;
                        openFileDialog.CheckPathExists = true;
                        openFileDialog.DefaultExt = fileDialogInfo.Extension;
                        openFileDialog.DereferenceLinks = true;
                        openFileDialog.Filter = fileDialogInfo.Filters;
                        openFileDialog.Multiselect = fileDialogInfo.Multiselect;
                        openFileDialog.RestoreDirectory = true;
                        openFileDialog.ShowHelp = true;
                        openFileDialog.ShowReadOnly = false;
                        openFileDialog.ValidateNames = true;
                        openFileDialog.FileName = PreviousFileName;
                        openFileDialog.FileNames[0] = fileDialogInfo.Filename;
                        openFileDialog.InitialDirectory = (fileDialogInfo.InitialDirectory == default(Environment.SpecialFolder) ? fileDialogInfo.CustomInitialDirectory : Environment.GetFolderPath(Environment.SpecialFolder.Personal).WithTrailingSeparator());
                        //openFileDialog.InitialDirectory = (fileDialogInfo.InitialDirectory == default(Environment.SpecialFolder) ? null : Environment.GetFolderPath(Environment.SpecialFolder.Personal).WithTrailingSeparator());

                        DialogResult dialogResult = openFileDialog.ShowDialog();

                        switch (dialogResult)
                        {
                            case DialogResult.OK:
                                {
                                    fileDialogInfo.Filename = openFileDialog.FileName;
                                    fileDialogInfo.Filenames = openFileDialog.FileNames;

                                    returnValue = true;

                                    break;
                                }
                            case DialogResult.Cancel:
                                {
                                    break;
                                }
                            default:
                                {
                                    throw new InvalidEnumArgumentException();
                                }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, MethodBase.GetCurrentMethod(), EventLogEntryType.Error);
                    
                throw;
            }

            return returnValue;
        }
    }
}
