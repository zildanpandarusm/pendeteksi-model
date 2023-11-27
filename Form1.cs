using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Parsing
{
    public partial class Form1 : Form
    {
        private string[] fileNames;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderDialog.SelectedPath;
                txtPath.Text = folderPath;
                ProcessFilesInFolder(folderPath);
            }
        }

        private void ProcessFilesInFolder(string folderPath)
        {
            try
            {
                this.fileNames = Directory.GetFiles(folderPath, "*.json");

                if (this.fileNames.Length == 0)
                {
                    MessageBox.Show("Tidak ada file JSON yang ditemukan di folder ini.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                MessageBox.Show("Pilih folder yang berisi file JSON terlebih dahulu.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<string> jsonArrayList = new List<string>();

            foreach (var fileName in fileNames)
            {
                try
                {
                    string jsonContent = File.ReadAllText(fileName);
                    jsonArrayList.Add(jsonContent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membaca file {Path.GetFileName(fileName)}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            JArray jsonArray = new JArray(jsonArrayList.Select(JToken.Parse));

            CheckParsing15.Point1(jsonArray);
            CheckParsing15.Point2(jsonArray);
            CheckParsing15.Point3(jsonArray);
            CheckParsing15.Point4(jsonArray);
            CheckParsing15.Point5(jsonArray);
            CheckParsing610.Point6(jsonArray);
            CheckParsing610.Point7(jsonArray);
            CheckParsing610.Point8(jsonArray);
            CheckParsing610.Point9(jsonArray);
        }
    }
}
