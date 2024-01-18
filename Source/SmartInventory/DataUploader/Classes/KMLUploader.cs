using BusinessLogics;
using Models;
using System;
using System.Collections.Generic;
using System.Xml;
using Utility;
namespace DataUploader
{
    public class KMLUploader : IUploadKML
    {

        public virtual ErrorMessage UploadKML(string strXMLPath, UploadSummary summary)
        {
           
            
            ErrorMessage error = new ErrorMessage();
            error.status = StatusCodes.OK.ToString();
            XmlDocument doc = new XmlDocument();

            using (XmlTextReader tr = new XmlTextReader(strXMLPath))
            {
                tr.Namespaces = false;// (uncomment to ignore namespace)
                doc.Load(tr);  // 'xsi' is an undeclared prefix error here
            }

            XmlElement root = null;
            XmlNodeList elePlacemark = null;
            try
            {
                //doc.Load(strXMLPath);
                root = doc.DocumentElement;
                elePlacemark = root.GetElementsByTagName("Placemark");
                string SQLquery = " Insert into KML_Attributes (user_id,Category,cable_name,Coordinates,uploaded_id) values";
                string SectionName = "";
                string coordinates = "";
                for (int i = 0; i <= elePlacemark.Count - 1; i++)
                {

                    SectionName = "";
                    coordinates = "";

                    foreach (XmlNode node in elePlacemark[i].ChildNodes)
                    {
                        if (node.Name.ToLower() == "name")
                        {
                            SectionName = node.InnerText;
                            break;
                        }
                    }

                    foreach (XmlNode node in elePlacemark[i].ChildNodes)
                    {
                        if (node.Name.ToLower() == "linestring")
                        {
                            foreach (XmlNode childnode in node.ChildNodes)
                            {
                                if (childnode.Name.ToLower() == "coordinates")
                                {
                                    coordinates = elePlacemark[i].LastChild.LastChild.InnerText.Trim();
                                    coordinates = coordinates.Replace("  ", "");
                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (coordinates.Contains("11\n"))
                    {
                        coordinates = coordinates.Replace("11\n", "");
                        coordinates = coordinates.Replace("\n", "");
                        coordinates = coordinates.Replace("\t", "");
                        coordinates = coordinates.Replace("\r\n", "##");
                        coordinates = coordinates.Replace("\\", "");
                        coordinates = coordinates.Replace("\r", "");
                    }
                    if (coordinates.Contains(",0"))
                    {
                        string Delimiter = ",0";
                        string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
                        List<string> lstOutLatLng = new List<string>();
                        string outGeometry = string.Empty;
                        if (Result.Length > 1 && Result[1].ToString() != "")
                        {
                            foreach (string latLng in Result)
                            {
                                if (!string.IsNullOrEmpty(latLng))
                                {
                                    lstOutLatLng.Add(latLng.Replace(",", " "));
                                }
                            }
                            //coordinates = coordinates.Replace(",0", "$$").Trim();

                            //SELECT ST_GeomFromText('LINESTRING(-71.160281 42.258729,-71.160837 42.259113,-71.161144 42.25932)');
                            //LINESTRING(-71.160281 42.258729,-71.160837 42.259113,-71.161144 42.25932)
                            outGeometry = string.Join(",", lstOutLatLng);
                            outGeometry = "LINESTRING(" + outGeometry + ")";
                            //if (coordinates.StartsWith("##"))
                            //{
                            //    coordinates = coordinates.Remove(0, 2);
                            //}

                            //coordinates = CommonUtility.Wrap(coordinates, 3000, "");
                            //SQLquery += "(" + summary.user_id + ",'" + summary.entity_type + "','" + SectionName.ToString().Replace("'", "") + "',E'" + coordinates.Trim() + "'," + summary.id + "),";
                            SQLquery += "(" + summary.user_id + ",'" + summary.entity_type + "','" + SectionName.ToString().Replace("'", "") + "','" + outGeometry + "'," + summary.id + "),";
                        }
                        else
                        {
                            //error.error_msg = "Placemark No " + (i + 1).ToString() + ") " + SectionName + " : has invalid geometry." + DateTimeHelper.Now.ToShortTimeString();
                            error.error_msg = "Placemark No " + (i + 1).ToString() + " " + SectionName + " : has invalid geometry!";
                            error.status = StatusCodes.INVALID_FILE.ToString();
                            error.is_valid = false;
                            error.uploaded_by = summary.user_id;
                            return error;


                        }
                    }
                    else
                    {
                        //error.error_msg = "Placemark No " + (i + 1).ToString()+" "  + SectionName + " : has invalid Delimiter." + DateTimeHelper.Now.ToShortTimeString();
                        error.error_msg = "Placemark No " + (i + 1).ToString() + " " + SectionName + " :" + (string.IsNullOrEmpty(coordinates) ? "has empty geometry!" : "has invalid Delimiter!") + " ";
                        error.status = StatusCodes.INVALID_FILE.ToString();
                        error.is_valid = false;
                        error.uploaded_by = summary.user_id;
                        return error;

                    }

                }
                error.status = StatusCodes.OK.ToString();
                SQLquery = SQLquery.Substring(0, SQLquery.Length - 1);
                new BLCable().InsertKML(SQLquery);
                error.is_valid = true;
                error.error_code = StatusCodes.OK.ToString();
                return error;
            }
            catch (Exception exp)
            {
                ErrorLogHelper.WriteErrorLog("KmlUploader", "UploadKML", exp);
                error.status = StatusCodes.FAILED.ToString();
                return error;

            }

        }
        //public virtual ErrorMessage UploadKML(string strXMLPath, UploadSummary summary)
        //{
        //    ErrorMessage error = new ErrorMessage();
        //    error.status = StatusCodes.OK.ToString();
        //    XmlDocument doc = new XmlDocument();

        //    using (XmlTextReader tr = new XmlTextReader(strXMLPath))
        //    {
        //        tr.Namespaces = false;// (uncomment to ignore namespace)
        //        doc.Load(tr);  // 'xsi' is an undeclared prefix error here
        //    }
        //    //SELECT ST_GeomFromText('LINESTRING(-71.160281 42.258729,-71.160837 42.259113,-71.161144 42.25932)');
        //    //LINESTRING(-71.160281 42.258729,-71.160837 42.259113,-71.161144 42.25932)

        //    XmlElement root = null;
        //    XmlNodeList elePlacemark = null;
        //    try
        //    {
        //        //doc.Load(strXMLPath);
        //        root = doc.DocumentElement;
        //        elePlacemark = root.GetElementsByTagName("Placemark");
        //        string SQLquery = " Insert into KML_Attributes (user_id,Category,cable_name,Coordinates,uploaded_id) values";
        //        string SectionName = "";
        //        string coordinates = "";
        //        for (int i = 0; i <= elePlacemark.Count - 1; i++)
        //        {

        //            SectionName = "";
        //            coordinates = "";

        //            foreach (XmlNode node in elePlacemark[i].ChildNodes)
        //            {
        //                if (node.Name.ToLower() == "name")
        //                {
        //                    SectionName = node.InnerText;
        //                    break;
        //                }
        //            }

        //            foreach (XmlNode node in elePlacemark[i].ChildNodes)
        //            {
        //                if (node.Name.ToLower() == "linestring")
        //                {
        //                    foreach (XmlNode childnode in node.ChildNodes)
        //                    {
        //                        if (childnode.Name.ToLower() == "coordinates")
        //                        {
        //                            coordinates = elePlacemark[i].LastChild.LastChild.InnerText.Trim();
        //                            coordinates = coordinates.Replace("  ", "");
        //                            break;
        //                        }
        //                    }
        //                    break;
        //                }
        //            }

        //            if (coordinates.Contains("11\n"))
        //            {
        //                coordinates = coordinates.Replace("11\n", "");
        //                coordinates = coordinates.Replace("\n", "");
        //                coordinates = coordinates.Replace("\t", "");
        //                coordinates = coordinates.Replace("\r\n", "##");
        //                coordinates = coordinates.Replace("\\", "");
        //                coordinates = coordinates.Replace("\r", "");
        //            }
        //            if (coordinates.Contains(",0"))
        //            {
        //                string Delimiter = ",0";
        //                string[] Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
        //                if (Result.Length > 1 && Result[1].ToString() != "")
        //                {
        //                    coordinates = coordinates.Replace(",0", "$$").Trim();

        //                    if (coordinates.StartsWith("##"))
        //                    {
        //                        coordinates = coordinates.Remove(0, 2);
        //                    }
        //                    //coordinates = CommonUtility.Wrap(coordinates, 3000, "");
        //                    SQLquery += "(" + summary.user_id + ",'" + summary.entity_type + "','" + SectionName.ToString().Replace("'", "") + "',E'" + coordinates.Trim() + "'," + summary.id + "),";
        //                }
        //                else
        //                {
        //                    error.error_msg = "Placemark No " + (i + 1).ToString() + ") " + SectionName + " : has invalid geometry." + DateTimeHelper.Now.ToShortTimeString();
        //                    error.status = StatusCodes.INVALID_FILE.ToString();
        //                    error.is_valid = false;
        //                    error.uploaded_by = summary.user_id;
        //                    return error;


        //                }
        //            }
        //            else
        //            {
        //                error.error_msg = "Placemark No " + (i + 1).ToString() + ") " + SectionName + " : has invalid Delimiter." + DateTimeHelper.Now.ToShortTimeString();
        //                error.status = StatusCodes.INVALID_FILE.ToString();
        //                error.is_valid = false;
        //                error.uploaded_by = summary.user_id;
        //                return error;

        //            }

        //        }
        //        error.status = StatusCodes.OK.ToString();
        //        SQLquery = SQLquery.Substring(0, SQLquery.Length - 1);
        //        new BLCable().InsertKML(SQLquery);
        //        error.is_valid = true;
        //        error.error_code = StatusCodes.OK.ToString();
        //        return error;
        //    }
        //    catch (Exception exp)
        //    {
        //        ErrorLogHelper.WriteErrorLog("KmlUploader", "UploadKML", exp);
        //        error.status = StatusCodes.FAILED.ToString();
        //        return error;

        //    }

        //}

    }
}
