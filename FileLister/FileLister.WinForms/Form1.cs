using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileLister.WinForms
{
    public partial class Form1 : Form
    {
        #region public contructor
        public Form1()
        {
            InitializeComponent();
            m_OutputFilePath = String.Format("c:\\temp\\List_{0}.txt", DateTime.Now.Ticks.ToString());
        }
        #endregion

        private string m_SelectedFolder { get; set; }
        private string m_OutputFilePath { get; set; }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            DialogResult dResult = folderBrowserDialog1.ShowDialog();

            if (dResult != DialogResult.OK)
            {
                MessageBox.Show("Please select the parent folder !");
            }

            m_SelectedFolder = folderBrowserDialog1.SelectedPath;

            txtSelectedFolderPath.Text = m_SelectedFolder;

            // Start BackgroundWorker
            backgroundWorker1.RunWorkerAsync(m_SelectedFolder);

        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string pathToProcess = e.Argument as string;
            ListFiles(pathToProcess);
        }

        private void ListFiles(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(path);

            #region Check and Iterate File names

            FileInfo[] files = dirInfo.GetFiles();
            if (files != null && files.Length > 0)
            {
                int index = 1;
                List<string> mp3FileNames = (from f in files
                                             where f.Extension.ToLower().Contains("mp3")
                                             select String.Format("<tr><td>{0}</td><td>{1}</td></tr>", index++, f.Name.Replace(",", " "))).ToList<string>();

                if (mp3FileNames != null && mp3FileNames.Count > 0)
                {
                    mp3FileNames.Insert(0, String.Format("<tr><td>&nbsp;</td><td>&nbsp;</td></tr><tr><td>FolderName:</td><td>{1}</td></tr>", 
                        System.Environment.NewLine,  dirInfo.FullName.Replace(",", " ")));

                    File.AppendAllLines(m_OutputFilePath, mp3FileNames);
                }
            }
            #endregion

            #region Check and Recurse on Folders

            DirectoryInfo[] subFolders = dirInfo.GetDirectories();

            if (subFolders != null && subFolders.Length > 0)
            {
                foreach (DirectoryInfo subFolder in subFolders)
                {
                    ListFiles(subFolder.FullName);
                }
            }

            #endregion
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Process completed...");
        }
    }
}
