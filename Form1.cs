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
                    MessageBox.Show("No JSON files found in this folder.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                this.ProcessJson(this.fileNames);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private JArray ProcessJson(string[] fileNames)
        {
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
                    MessageBox.Show($"Error reading the file {Path.GetFileName(fileName)}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

            JArray jsonArray = new JArray(jsonArrayList.Select(JToken.Parse));

            textSourceCode.Text = jsonArray.ToString();
            return jsonArray;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                MessageBox.Show("Please select a folder containing JSON files first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            JArray jsonArray = this.ProcessJson(fileNames);

            msgBox.Clear();

            CheckParsing15.Point1(this, jsonArray);
            CheckParsing15.Point2(this, jsonArray);
            CheckParsing15.Point3(this, jsonArray);
            CheckParsing15.Point4(this, jsonArray);
            CheckParsing15.Point5(this, jsonArray);
            CheckParsing610.Point6(this, jsonArray);
            CheckParsing610.Point7(this, jsonArray);
            CheckParsing610.Point8(this, jsonArray);
            CheckParsing610.Point9(this, jsonArray);
            CheckParsing610.Point10(this, jsonArray);
            CheckParsing1115.Point11(this, jsonArray);
            CheckParsing1115.Point13(this, jsonArray);
            CheckParsing1115.Point14(this, jsonArray);
            CheckParsing1115.Point15(this, jsonArray);
            CheckParsing1115.Point99(this, jsonArray);

            if (string.IsNullOrWhiteSpace(msgBox.Text))
            {
                msgBox.Text = "Model has successfully passed parsing";
            }
        }
        public TextBox GetMessageBox()
        {
            return msgBox;
        }
    }
}
