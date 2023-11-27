using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parsing
{
    public static class CheckParsing610
    {
        public static bool Point6(JArray jsonArray)
        {
            try
            {
                foreach (var subsystem in jsonArray)
                {
                   
                    foreach (var item in subsystem["model"])
                    {
                        var itemType = item["type"]?.ToString();

                        if (itemType == "class" && item["class_name"] != null)
                        {
                            var className = item["class_name"]?.ToString();
                            var attributes = item["attributes"] as JArray;

                            if (attributes != null)
                            {
                                HashSet<string> uniqueAttributeNames = new HashSet<string>();

                                foreach (var attribute in attributes)
                                {
                                    var attributeName = attribute["attribute_name"]?.ToString();

                                    if (string.IsNullOrWhiteSpace(attributeName))
                                    {
                                        MessageBox.Show($"Error: Nama atribut kosong dalam class {className}.");
                                        return false;
                                    }

                                    if (uniqueAttributeNames.Contains(attributeName))
                                    {
                                        MessageBox.Show($"Error: Nama atribut {attributeName} duplikat dalam class {className}.");
                                        return false;
                                    }

                                    uniqueAttributeNames.Add(attributeName);
                                }
                            }                         
                        }

                        if (itemType == "association" && item["model"] is JObject associationModel)
                        {
                            var associationItemType = associationModel["type"]?.ToString();

                            if (associationItemType == "association_class" && associationModel["class_name"] != null)
                            {
                                var associationClassName = associationModel["class_name"]?.ToString();
                                var associationAttributes = associationModel["attributes"] as JArray;

                                if (associationAttributes != null)
                                {
                                    HashSet<string> uniqueAssociationAttributeNames = new HashSet<string>();

                                    foreach (var attribute in associationAttributes)
                                    {
                                        var attributeName = attribute["attribute_name"]?.ToString();

                                        if (string.IsNullOrWhiteSpace(attributeName))
                                        {
                                            MessageBox.Show($"Error: Nama atribut kosong dalam association class {associationClassName}.");
                                            return false;
                                        }

                                        if (uniqueAssociationAttributeNames.Contains(attributeName))
                                        {
                                            MessageBox.Show($"Error: Nama atribut {attributeName} duplikat dalam association class {associationClassName}.");
                                            return false;
                                        }

                                        uniqueAssociationAttributeNames.Add(attributeName);
                                    }
                                }
                            }
                        }

                    }
                }

                MessageBox.Show("Semua nama atribut unik di dalam setiap class.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static bool Point7(JArray jsonArray)
        {
            try
            {
                foreach (var subsystem in jsonArray)
                {
                    foreach (var item in subsystem["model"])
                    {
                        var itemType = item["type"]?.ToString();

                        if (itemType == "class" && item["class_name"] != null)
                        {
                            var className = item["class_name"]?.ToString();
                            var attributes = item["attributes"] as JArray;

                            if (attributes != null)
                            {
                                bool hasPrimaryKey = false;

                                foreach (var attribute in attributes)
                                {
                                    var attributeType = attribute["attribute_type"]?.ToString();

                                    if (attributeType == "naming_attribute")
                                    {
                                        hasPrimaryKey = true;
                                        break;
                                    }
                                }

                                if (!hasPrimaryKey)
                                {
                                    MessageBox.Show($"Error: Class {className} tidak memiliki primary key.");
                                    return false;
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Error: Class {className} tidak memiliki atribut.");
                                return false;
                            }
                        }

                        if (itemType == "association" && item["model"] is JObject associationModel)
                        {
                            var associationItemType = associationModel["type"]?.ToString();

                            if (associationItemType == "association_class" && associationModel["class_name"] != null)
                            {
                                var associationClassName = associationModel["class_name"]?.ToString();
                                var associationAttributes = associationModel["attributes"] as JArray;

                                if (associationAttributes != null)
                                {
                                    bool hasPrimaryKey = false;

                                    foreach (var attribute in associationAttributes)
                                    {
                                        var attributeType = attribute["attribute_type"]?.ToString();

                                        if (attributeType == "naming_attribute")
                                        {
                                            hasPrimaryKey = true;
                                            break;
                                        }
                                    }

                                    if (!hasPrimaryKey)
                                    {
                                        MessageBox.Show($"Error: Association Class {associationClassName} tidak memiliki primary key.");
                                        return false;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"Error: Association Class {associationClassName} tidak memiliki atribut.");
                                    return false;
                                }
                            }
                        }

                    }
                }

                MessageBox.Show("Setiap class, termasuk association_class, memiliki primary key.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static bool Point8(JArray jsonArray)
        {
            try
            {              
                foreach (var subsystem in jsonArray)
                {
                    HashSet<string> associationNames = new HashSet<string>();

                    foreach (var item in subsystem["model"])
                    {
                        var itemType = item["type"]?.ToString();

                        if (itemType == "association" && item["name"] != null)
                        {
                            var associationName = item["name"]?.ToString();

                            if (string.IsNullOrWhiteSpace(associationName))
                            {
                                MessageBox.Show("Error: Nama association kosong dalam subsistem.");
                                return false;
                            }

                            if (associationNames.Contains(associationName))
                            {
                                MessageBox.Show($"Error: Nama association {associationName} duplikat dalam subsistem ini.");
                                return false;
                            }

                            associationNames.Add(associationName);
                        }
                    }
                }

                MessageBox.Show("Semua nama association unik di setiap subsistem.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        

        public static bool Point9(JArray jsonArray)
        {
            try
            {
                foreach (var subsystem in jsonArray)
                {
                    foreach (var item in subsystem["model"])
                    {
                        var itemType = item["type"]?.ToString();

                        if (itemType == "association")
                        {
                            var class1Multiplicity = item["class"][0]["class_multiplicity"]?.ToString();
                            var class2Multiplicity = item["class"][1]["class_multiplicity"]?.ToString();

                            if ((class1Multiplicity == "0..*" && class2Multiplicity == "0..*") || (class1Multiplicity == "0..*" && class2Multiplicity == "1..*") || (class1Multiplicity == "1..*" && class2Multiplicity == "0..*") || (class1Multiplicity == "1..*" && class2Multiplicity == "1..*"))
                            {
                                var associationModel = item["model"];
                                if (associationModel == null)
                                {
                                    MessageBox.Show($"Error: Relasi {item["name"]?.ToString()} (many-to-many) belum diformalisasi dengan association_class.");
                                    return false;
                                }

                                if (associationModel != null && associationModel["type"]?.ToString() != "association_class")
                                {
                                    MessageBox.Show($"Error: Relasi {item["name"]?.ToString()} (many-to-many) belum diformalisasi dengan association_class.");
                                    return false;
                                }
                            }
                            else if ((class1Multiplicity == "0..*" && class2Multiplicity == "1..1") ||
                                     (class1Multiplicity == "1..1" && class2Multiplicity == "0..*") ||
                                     (class1Multiplicity == "1..*" && class2Multiplicity == "1..1") ||
                                     (class1Multiplicity == "1..1" && class2Multiplicity == "1..*") ||
                                     (class1Multiplicity == "1..1" && class2Multiplicity == "1..1"))
                            {
                                var class1Id = item["class"][0]["class_id"]?.ToString();
                                var class2Id = item["class"][1]["class_id"]?.ToString();

                                if (!HasReferentialAttribute(jsonArray, class1Id) && !HasReferentialAttribute(jsonArray, class2Id))
                                {
                                    MessageBox.Show($"Error: Salah satu dari Class {class1Id} atau {class2Id} pada relasi {item["name"]?.ToString()} (one-to-one) harus diformalisasi dengan referential_attribute.");
                                    return false;
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Semua relasi sudah diformalisasi.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }
        public static bool HasReferentialAttribute(JArray jsonArray, string classId)
        {
            foreach (var subsystem in jsonArray)

            {
                foreach (var item in subsystem["model"])
                {
                    var itemType = item["type"]?.ToString();

                    if (itemType == "class")
                    {
                        var currentClassId = item["class_id"]?.ToString();

                        if (currentClassId == classId)
                        {
                            var attributes = item["attributes"] as JArray;

                            if (attributes != null)
                            {
                                foreach (var attribute in attributes)
                                {
                                    var attributeType = attribute["attribute_type"]?.ToString();

                                    if (attributeType == "referential_attribute")
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

    }
}
