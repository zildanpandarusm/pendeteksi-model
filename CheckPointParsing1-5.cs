using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Parsing
{
    public static class CheckParsing15
    {
        public static bool Point1(JArray jsonArray)
        {
            try
            {
                HashSet<string> subsystemNames = new HashSet<string>();

                foreach (var item in jsonArray)
                {
                    if (item["type"]?.ToString() == "subsystem")
                    {
                        var subsystemProperty = item["sub_name"];

                        if (subsystemProperty != null)
                        {
                            string subsystemName = subsystemProperty.ToString();

                            if (subsystemNames.Contains(subsystemName))
                            {
                                MessageBox.Show($"Ada subsistem dengan nama yang sama: {subsystemName}. Pastikan semua subsistem memiliki nama yang unik.");
                                return false;
                            }

                            subsystemNames.Add(subsystemName);
                        }
                        else
                        {
                            MessageBox.Show("Error: Properti 'sub_name' tidak ditemukan atau kosong.");
                            return false;
                        }
                    }
                }

                MessageBox.Show("Semua nama subsistem unik.");
                return true; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false; 
            }
        }
        public static bool Point2(JArray jsonArray)
        {
            try
            {
                foreach (var subsystem in jsonArray)
                {
                    HashSet<string> classIdsInClass = new HashSet<string>();
                    HashSet<string> classIdsInAssociationClass = new HashSet<string>();

                    var modelArray = subsystem["model"] as JArray;

                    if (modelArray != null)
                    {
                        foreach (var item in modelArray)
                        {
                            var itemType = item["type"]?.ToString();

                            if (itemType == "class")
                            {
                                var classIdProperty = item["class_id"];
                                var attributesArray = item["attributes"] as JArray;

                                if (classIdProperty != null)
                                {
                                    string classId = classIdProperty.ToString();
                                    classIdsInClass.Add(classId);
                                    
                                    if (attributesArray == null || !attributesArray.Any())
                                    {
                                        MessageBox.Show($"Error: Class {classId} pada subsistem {subsystem["sub_name"]?.ToString()} tidak memiliki atribut.");
                                        return false;
                                    }
                                }
                            }
                            else if (itemType == "association")
                            {
                                var classArrayProperty = item["class"] as JArray;

                                if (classArrayProperty != null)
                                {
                                    foreach (var classItem in classArrayProperty)
                                    {
                                        var classIdProperty = classItem["class_id"];

                                        if (classIdProperty != null)
                                        {
                                            string classId = classIdProperty.ToString();
                                            classIdsInAssociationClass.Add(classId);
                                        }
                                    }
                                }

                                var associationClassModel = item["model"];
                                if (associationClassModel is JObject associationObject)
                                {
                                    var associationClassType = associationObject["type"]?.ToString();

                                    if (associationClassType == "association_class")
                                    {
                                        var classIdProperty = associationObject["class_id"];

                                        if (classIdProperty != null)
                                        {
                                            string classId = classIdProperty.ToString();
                                            classIdsInClass.Add(classId);
                                            classIdsInAssociationClass.Add(classId);

                                            var attributesArray = associationObject["attributes"] as JArray;
                                            if (attributesArray == null || !attributesArray.Any())
                                            {
                                                MessageBox.Show($"Error: Class {classId} pada subsistem {subsystem["sub_name"]?.ToString()} tidak memiliki atribut.");
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var classesWithoutRelation = classIdsInClass.Except(classIdsInAssociationClass);

                    if (classesWithoutRelation.Any())
                    {
                        MessageBox.Show($"Error: Terdapat class tanpa relasi pada subsistem {subsystem["sub_name"]?.ToString()}. Class ID yang tidak memiliki relasi: {string.Join(", ", classesWithoutRelation)}");
                        return false;
                    }
                }

                MessageBox.Show("Semua class_id dalam type association pada semua subsistem telah ditemukan dalam array class.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static bool Point3(JArray jsonArray)
        {
            try
            {
                Dictionary<string, HashSet<string>> classInfoMap = new Dictionary<string, HashSet<string>>();

                Func<JToken, bool> processItem = null;
                processItem = (item) =>
                {
                    var itemType = item["type"]?.ToString();

                    if (itemType == "class")
                    {
                        var className = item["class_name"]?.ToString();
                        var attributes = GetAttributesAsString(item["attributes"] as JArray);

                        var classInfo = $"{className}-{attributes}";

                        if (classInfoMap.ContainsKey(classInfo))
                        {
                            MessageBox.Show($"Error: Class {className} dalam subsistem {item["sub_name"]?.ToString()} memiliki informasi yang sama dengan class dalam subsistem lain.");
                            return false;
                        }
                        else
                        {
                            classInfoMap.Add(classInfo, new HashSet<string>());
                        }
                    }
                    else if (itemType == "association")
                    {
                        var classArrayProperty = item["class"] as JArray;

                        if (classArrayProperty != null)
                        {
                            foreach (var classItem in classArrayProperty)
                            {
                                if (!processItem(classItem))
                                {
                                    return false;
                                }
                            }
                        }

                        var associationClassModel = item["model"];
                        if (associationClassModel is JObject associationObject)
                        {
                            if (!processItem(associationObject))
                            {
                                return false;
                            }
                        }
                    }
                    else if (itemType == "association_class")
                    {
                        var className = item["class_name"]?.ToString();
                        var attributes = GetAttributesAsString(item["attributes"] as JArray);

                        var classInfo = $"{className}-{attributes}";

                        if (classInfoMap.ContainsKey(classInfo))
                        {
                            MessageBox.Show($"Error: Class {className} dalam subsistem {item["sub_name"]?.ToString()} memiliki informasi yang sama dengan class dalam subsistem lain.");
                            return false;
                        }
                        else
                        {
                            classInfoMap.Add(classInfo, new HashSet<string>());
                        }
                    }

                    return true;
                };

                foreach (var subsystem in jsonArray)
                {
                    foreach (var item in subsystem["model"])
                    {
                        if (!processItem(item))
                        {
                            return false;
                        }
                    }
                }

                MessageBox.Show("Semua class dalam setiap subsistem memiliki informasi yang unik.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }


        private static string GetAttributesAsString(JArray attributes)
        {
            if (attributes == null)
            {
                return string.Empty;
            }

            List<string> attributeStrings = new List<string>();

            foreach (var attribute in attributes)
            {
                var attributeType = attribute["attribute_type"]?.ToString();
                var attributeName = attribute["attribute_name"]?.ToString();
                var dataType = attribute["data_type"]?.ToString();

                attributeStrings.Add($"{attributeType}-{attributeName}-{dataType}");
            }

            return string.Join("|", attributeStrings);
        }

        public static bool Point4(JArray jsonArray)
        {
            try
            {
                foreach (var subsystem in jsonArray)
                {
                    HashSet<string> classNames = new HashSet<string>();
                    HashSet<string> classIds = new HashSet<string>();

                    foreach (var item in subsystem["model"])
                    {
                        var itemType = item["type"]?.ToString();

                        if ((itemType == "class" || itemType == "association_class") && item["class_name"] != null)
                        {
                            var className = item["class_name"]?.ToString();
                            var classId = item["class_id"]?.ToString();

                            if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(classId))
                            {
                                MessageBox.Show("Error: Nama class atau class_id kosong dalam subsistem.");
                                return false;
                            }

                            if (classNames.Contains(className))
                            {
                                MessageBox.Show($"Error: Nama class {className} duplikat dalam subsistem ini.");
                                return false;
                            }

                            if (classIds.Contains(classId))
                            {
                                MessageBox.Show($"Error: class_id {classId} duplikat dalam subsistem ini.");
                                return false;
                            }

                            classNames.Add(className);
                            classIds.Add(classId);
                        }

                        if (itemType == "association" && item["model"] is JObject associationModel)
                        {
                            var associationItemType = associationModel["type"]?.ToString();

                            if (associationItemType == "association_class" && associationModel["class_name"] != null)
                            {
                                var associationClassName = associationModel["class_name"]?.ToString();
                                var associationClassId = associationModel["class_id"]?.ToString();

                                if (string.IsNullOrWhiteSpace(associationClassName) || string.IsNullOrWhiteSpace(associationClassId))
                                {
                                    MessageBox.Show("Error: Nama class atau class_id kosong dalam subsistem.");
                                    return false;
                                }

                                if (classNames.Contains(associationClassName))
                                {
                                    MessageBox.Show($"Error: Nama class {associationClassName} duplikat dalam subsistem ini.");
                                    return false;
                                }

                                if (classIds.Contains(associationClassId))
                                {
                                    MessageBox.Show($"Error: class_id {associationClassId} duplikat dalam subsistem ini.");
                                    return false;
                                }

                                classNames.Add(associationClassName);
                                classIds.Add(associationClassId);
                            }
                        }
                    }
                }

                MessageBox.Show("Semua nama class dan class_id unik di dalam setiap subsistem.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static bool Point5(JArray jsonArray)
        {
            try
            {
                foreach (var subsystem in jsonArray)
                {
                    HashSet<string> uniqueKLs = new HashSet<string>();
                    foreach (var item in subsystem["model"])
                    {
                        var itemType = item["type"]?.ToString();

                        if (itemType == "class" && item["class_name"] != null)
                        {
                            var className = item["class_name"]?.ToString();
                            var KL = item["KL"]?.ToString();

                            if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(KL))
                            {
                                MessageBox.Show("Error: Nama class atau KL kosong dalam subsistem.");
                                return false;
                            }

                            if (uniqueKLs.Contains(KL))
                            {
                                MessageBox.Show($"Error: Nilai KL {KL} duplikat dalam subsistem ini.");
                                return false;
                            }

                            uniqueKLs.Add(KL);
                        }

                        if (itemType == "association" && item["model"] is JObject associationModel)
                        {
                            var associationItemType = associationModel["type"]?.ToString();

                            if (associationItemType == "association_class" && associationModel["class_name"] != null)
                            {
                                var associationKL = associationModel["KL"]?.ToString();

                                if (string.IsNullOrWhiteSpace(associationKL))
                                {
                                    MessageBox.Show("Error: Nilai KL kosong dalam subsistem.");
                                    return false;
                                }

                                if (uniqueKLs.Contains(associationKL))
                                {
                                    MessageBox.Show($"Error: Nilai KL {associationKL} duplikat dalam subsistem ini.");
                                    return false;
                                }

                                uniqueKLs.Add(associationKL);
                            }
                        }
                    }
                }

                MessageBox.Show("Semua nilai KL unik di dalam setiap subsistem.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

    }

}

