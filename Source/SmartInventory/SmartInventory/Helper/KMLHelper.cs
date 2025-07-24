using Ionic.Zip;
using Models;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Web;
using Utility;
using System.Linq;
using System.Reflection;
using BusinessLogics;

namespace SmartInventory.Helper
{
    public class KMLHelper
    {
        public static void DatasetToKML(DataSet ds, List<layerReportDetail> lstLayers, string tempFolderPath, string TempkmlFileName, string layerName = "", DataTable dtFilter = null, string fileType = "")
        {
            string desFolderPath = string.Empty;
            string tempFolderName = "";
            if (fileType == "KML") { tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss"); }
            else { tempFolderName = "XML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss"); }
            string desTempFolderPath = HttpContext.Current.Server.MapPath(tempFolderPath) + tempFolderName;
            if (Directory.Exists(desTempFolderPath).Equals(false))
                Directory.CreateDirectory(desTempFolderPath);

            string finalkml = GetKmlForEntities(ds, lstLayers, tempFolderPath, dtFilter, desTempFolderPath);
            string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
            System.IO.File.WriteAllText(kmlDesFullPath, finalkml.ToString());
            string zipfilePath = desTempFolderPath + ".zip";

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(desTempFolderPath);
                zip.Save(zipfilePath);
            }
            if (System.IO.File.Exists(zipfilePath))
            {
                string fileName = Path.GetFileName(zipfilePath);
                Directory.Delete(desTempFolderPath, true);
            }
            FileInfo file = new FileInfo(zipfilePath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export_" + fileType + "_" + layerName + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/zip";
            HttpContext.Current.Response.WriteFile(file.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
            File.Delete(zipfilePath);
        }

        public static void DatasetToKMLNew(string ftpfilePath, string ftpUserName, string ftpPassword, DataSet ds, List<layerReportDetail> lstLayers, string tempFolderPath,string tempFolderName, string TempkmlFileName, string layerName = "", DataTable dtFilter = null, string fileType = "")
        {
            string desFolderPath = string.Empty;
            string desTempFolderPath = System.Web.Hosting.HostingEnvironment.MapPath(tempFolderPath) + tempFolderName;
            if (Directory.Exists(desTempFolderPath).Equals(false))
                Directory.CreateDirectory(desTempFolderPath);

            string finalkml = GetKmlForEntitiesNew(ds, lstLayers, tempFolderPath, dtFilter, desTempFolderPath);
            finalkml = finalkml.Replace("<>", "");
            finalkml = finalkml.Replace("</>", "");

            finalkml = RemoveInvalidXmlChars(finalkml);
            string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
            System.IO.File.WriteAllText(kmlDesFullPath, finalkml.ToString());
            string zipfilePath = desTempFolderPath + ".zip";
            
            string fileNameValue=tempFolderName + ".zip";

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(desTempFolderPath);
                zip.Save(zipfilePath);
            }
            if (System.IO.File.Exists(zipfilePath))
            {
                string fileName = Path.GetFileName(zipfilePath);
                Directory.Delete(desTempFolderPath, true);
            }
            FileInfo file = new FileInfo(zipfilePath);

            CommonUtility.FTPFileUpload(zipfilePath, fileNameValue, ftpfilePath, ftpUserName, ftpPassword);

            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export_" + fileType + "_" + layerName + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            //HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
            //HttpContext.Current.Response.ContentType = "application/zip";
            //HttpContext.Current.Response.WriteFile(file.FullName);
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();
            File.Delete(zipfilePath);
        }

        public static string GetKmlForEntities(DataSet ds, List<layerReportDetail> lstLayers, string tempFolderPath, DataTable dtFilter = null, string desTempFolderPath = null)
        {
            string finalKMLString = String.Empty;
            StringBuilder sbLine = new StringBuilder();
            StringBuilder sbPoint = new StringBuilder();
            StringBuilder sbPolygon = new StringBuilder();
            sbLine.Append("<name>Line</name>");
            sbPoint.Append("<name>Point</name>");
            sbPolygon.Append("<name>Polygon</name>");
            string cablecolor = string.Empty;
            string polycolor = string.Empty;
            int lineWidth = 2;
            #region ANTRA
            foreach (DataTable dt in ds.Tables)
            {
                var objLayerDetail = lstLayers.Where(m => m.layer_title.ToUpper() == dt.TableName.ToString().ToUpper() || m.layer_name.ToUpper() == dt.TableName.ToString().ToUpper()).FirstOrDefault();
                foreach (DataRow row in dt.Rows)
                {
                    //var obj = (IDictionary<string, object>)new ExpandoObject();
                    StringBuilder description = new StringBuilder();
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE", "GEOM_TYPE", "ENTITY_TITLE", "ENTITY_NAME", "GEOM" };
                    string[] arrIgnoreColumnskml = { "TOTALRECORDS", "S_NO", "BARCODE" };
                    foreach (DataColumn col in dt.Columns)
                    {
                        //if (!Array.Exists(arrIgnoreColumnskml, m => m == col.ColumnName.ToUpper()))
                        //{
                        //    //obj.Add(col.ColumnName.ToUpper() == "NETWORK ID" ? "network_id" : (col.ColumnName.ToUpper() == "CABLE TYPE" ? "cable_type" : col.ColumnName), row["NETWORK ID"].ToString());
                        //}
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.ColumnName.ToUpper()))
                        {
                            description.Append("<SimpleData name='" + col.ColumnName + "'>" + (String.IsNullOrEmpty(Convert.ToString(row[col.ColumnName])) ? row[col.ColumnName].ToString() : row[col.ColumnName].ToString().Replace("&", "&amp;")) + "</SimpleData>");
                        }
                    }

                    if (objLayerDetail.geom_type.ToUpper() == "LINE")
                    {
                        if (dt.TableName.ToUpper() == "DUCT" || dt.TableName.ToUpper() == "TRENCH")
                        {
                            lineWidth = 4;
                        }

                        cablecolor = dt.TableName.ToUpper() == "DUCT" ? "#f1021f8" : dt.TableName.ToUpper() == "TRENCH" ? "#ff0000ff" : "#ff0000ff";

                        if (dt.TableName.ToUpper() == "CABLE")
                        {
                            if ((dt.Columns.Contains("cable_type")))
                            {
                                cablecolor = Convert.ToString(row["cable_type"]).ToUpper() == "UNDERGROUND" ? "#ff0000ff" : Convert.ToString(row["cable_type"]).ToUpper() == "OVERHEAD" ? "#f1021f8" : (Convert.ToString(row["cable_type"]).ToUpper() == "WALL MOUNTED" || Convert.ToString(row["cable_type"]).ToUpper() == "ISP") ? "#fa894e7" : "#ff0000ff";
                            }
                            else
                            {
                                cablecolor = "#ff0000ff";
                            }
                        }
                        sbLine.Append("<Placemark><name>" + dt.TableName + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                        sbLine.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                        if ((!string.IsNullOrEmpty(Convert.ToString(row["geom"]))))
                        {
                           // string t = Convert.ToString(row["geom"]).Substring(11, Convert.ToString(row["geom"]).Length - 13);

                            string t = Convert.ToString(row["geom"]).Replace("LINESTRING", "").Replace("(", "").Replace(")", "").Trim();          
                            string[] x = t.Split(',');
                            foreach (string y in x)
                            {
                                sbLine.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                            }
                        }
                        sbLine.Append("</coordinates></LineString></Placemark>");
                    }
                    if (objLayerDetail.geom_type.ToUpper() == "POINT")
                    {

                        if (dt.TableName != null)
                        {

                            foreach (var lyr in dt.TableName.ToString())
                            {
                                var strIconPath = string.Concat(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), objLayerDetail.layer_name, ".png");
                                if (File.Exists(strIconPath))
                                {
                                    var legendFolderPath = desTempFolderPath + "\\Legend\\";
                                    if (Directory.Exists(legendFolderPath).Equals(false))
                                        Directory.CreateDirectory(legendFolderPath);
                                    var layerImageName = Path.GetFileName(strIconPath.ToString());
                                    MiscHelper mh = new MiscHelper();
                                    if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                                    {
                                        mh.CopyFile(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
                                    }
                                }

                            }
                        }
                        sbPoint.Append("<Placemark><name>" + dt.TableName + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + string.Concat("Legend\\" + "\\" + objLayerDetail.layer_name, ".png") + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                        sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                        if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
                        {
                            //string t = Convert.ToString(row["geom"]).Substring(6, Convert.ToString(row["geom"]).Length - 8);

                            string t = Convert.ToString(row["geom"]).Replace("POINT", "").Replace("(", "").Replace(")", "").Trim();

                            sbPoint.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                        }
                        sbPoint.Append("</coordinates></Point></Placemark>");

                    }
                    if (objLayerDetail.geom_type.ToUpper() == "POLYGON")
                    {
                        polycolor = dt.TableName.ToUpper() == "AREA" ? "7fe8dab4" : dt.TableName.ToUpper() == "SUBAREA" ? "7f71e9d7" : "#ff0000ff";
                        //sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToUpper() + "\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>" + polycolor + "</color></PolyStyle></Style><Placemark><name>" + dt.TableName + "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToUpper() + "</styleUrl>");
                        if ((dt.TableName.ToString().ToUpper() == EntityType.Sector.ToString().ToUpper()) && dt.Columns.Contains("Kml Color Code"))
                        {
                            polycolor = Convert.ToString(row["Kml Color Code"]);
                        }

                        sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToString().ToUpper() +
                            "\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>" + polycolor +
                            "</color></PolyStyle></Style><Placemark><name>" + dt.TableName.ToString() +
                            "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description +
                            "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToString().ToUpper() +
                            "</styleUrl>");

                        sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
                        if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
                        {
                           // string t = Convert.ToString(row["geom"]).Substring(9, Convert.ToString(row["geom"]).Length - 11);

                            string t = Convert.ToString(row["geom"]).Replace("POLYGON", "").Replace("(", "").Replace(")", "").Trim(); 
                            string[] x = t.Split(',');
                            foreach (string y in x)
                            {
                                sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                            }
                        }
                        sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                    }
                }
            }
            #endregion

            //sbLine.Append("</Folder>");
            //sbPoint.Append("</Folder>");
            //sbPolygon.Append("</Folder>");
            StringBuilder filterValues = new StringBuilder();
            filterValues.Append("<table><tr><th>Filter Type</th><th>Value</th></tr>");
            filterValues.AppendLine();
            if (dtFilter != null)
            {
                for (int i = 0; i < dtFilter.Rows.Count; i++)
                {
                    filterValues.Append("<tr><td>" + dtFilter.Rows[i][0] + "</td><td>" + dtFilter.Rows[i][1] + "</td></tr>");
                    filterValues.AppendLine();
                }
            }
            filterValues.Append("</table>");

            finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                               "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                "<filter>" + filterValues + "</filter>" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                               "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                               "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style><Folder>" +
                               sbPolygon.ToString() + sbLine.ToString() + sbPoint.ToString() + "</Folder></Document></kml>";

            return finalKMLString;
        }
        public static string GetKmlForEntitiesNew(DataSet ds, List<layerReportDetail> lstLayers, string tempFolderPath, DataTable dtFilter = null, string desTempFolderPath = null)
        {
            string finalKMLString = String.Empty;
            StringBuilder sbLine = new StringBuilder();
            StringBuilder sbPoint = new StringBuilder();
            StringBuilder sbPolygon = new StringBuilder();
            sbLine.Append("<name>Line</name>");
            sbPoint.Append("<name>Point</name>");
            sbPolygon.Append("<name>Polygon</name>");
            string cablecolor = string.Empty;
            string polycolor = string.Empty;
            int lineWidth = 2;
            #region ANTRA
            foreach (DataTable dt in ds.Tables)
            {
                var objLayerDetail = lstLayers.Where(m => m.layer_title.ToUpper() == dt.TableName.ToString().ToUpper() || m.layer_name.ToUpper() == dt.TableName.ToString().ToUpper()).FirstOrDefault();
                foreach (DataRow row in dt.Rows)
                {
                    //var obj = (IDictionary<string, object>)new ExpandoObject();
                    StringBuilder description = new StringBuilder();
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE", "GEOM_TYPE", "ENTITY_TITLE", "ENTITY_NAME", "GEOM" };
                    string[] arrIgnoreColumnskml = { "TOTALRECORDS", "S_NO", "BARCODE" };
                    foreach (DataColumn col in dt.Columns)
                    {
                        //if (!Array.Exists(arrIgnoreColumnskml, m => m == col.ColumnName.ToUpper()))
                        //{
                        //    //obj.Add(col.ColumnName.ToUpper() == "NETWORK ID" ? "network_id" : (col.ColumnName.ToUpper() == "CABLE TYPE" ? "cable_type" : col.ColumnName), row["NETWORK ID"].ToString());
                        //}
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.ColumnName.ToUpper()))
                        {
                            description.Append("<SimpleData name='" + col.ColumnName + "'>" + (String.IsNullOrEmpty(Convert.ToString(row[col.ColumnName])) ? row[col.ColumnName].ToString() : row[col.ColumnName].ToString().Replace("&", "&amp;")) + "</SimpleData>");
                        }
                    }

                    if (objLayerDetail.geom_type.ToUpper() == "LINE")
                    {
                        if (dt.TableName.ToUpper() == "DUCT" || dt.TableName.ToUpper() == "TRENCH")
                        {
                            lineWidth = 4;
                        }

                        cablecolor = dt.TableName.ToUpper() == "DUCT" ? "#f1021f8" : dt.TableName.ToUpper() == "TRENCH" ? "#ff0000ff" : "#ff0000ff";

                        if (dt.TableName.ToUpper() == "CABLE")
                        {
                            if ((dt.Columns.Contains("cable_type")))
                            {
                                cablecolor = Convert.ToString(row["cable_type"]).ToUpper() == "UNDERGROUND" ? "#ff0000ff" : Convert.ToString(row["cable_type"]).ToUpper() == "OVERHEAD" ? "#f1021f8" : (Convert.ToString(row["cable_type"]).ToUpper() == "WALL MOUNTED" || Convert.ToString(row["cable_type"]).ToUpper() == "ISP") ? "#fa894e7" : "#ff0000ff";
                            }
                            else
                            {
                                cablecolor = "#ff0000ff";
                            }
                        }
                        sbLine.Append("<Placemark><name>" + dt.TableName + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                        sbLine.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                        if ((!string.IsNullOrEmpty(Convert.ToString(row["geom"]))))
                        {
                            // string t = Convert.ToString(row["geom"]).Substring(11, Convert.ToString(row["geom"]).Length - 13);

                            //string t = Convert.ToString(row["geom"]).Replace("LINESTRING", "").Replace("(", "").Replace(")", "").Trim();
                            //string[] x = (Convert.ToString(row["geom"]).Replace("LINESTRING", "").Replace("(", "").Replace(")", "").Trim()).Split(',');
                            //foreach (string y in x)
                            //{
                            //    sbLine.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                            //}


                            string x = Convert.ToString(row["geom"]).Replace("LINESTRING", "").Replace("(", "").Replace(")", "").Trim();
                            x = x.Replace(",", "#").Replace(" ", ",").Replace("#",",0 ");
                            sbLine.Append(x);
                        }
                        sbLine.Append("</coordinates></LineString></Placemark>");
                    }
                    if (objLayerDetail.geom_type.ToUpper() == "POINT")
                    {

                        if (dt.TableName != null)
                        {

                            foreach (var lyr in dt.TableName.ToString())
                            {
                                var strIconPath = string.Concat(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), objLayerDetail.layer_name, ".png");
                                if (File.Exists(strIconPath))
                                {
                                    var legendFolderPath = desTempFolderPath + "\\Legend\\";
                                    if (Directory.Exists(legendFolderPath).Equals(false))
                                        Directory.CreateDirectory(legendFolderPath);
                                    var layerImageName = Path.GetFileName(strIconPath.ToString());
                                    MiscHelper mh = new MiscHelper();
                                    if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                                    {
                                        mh.CopyFile(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
                                    }
                                }

                            }
                        }
                        sbPoint.Append("<Placemark><name>" + dt.TableName + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + string.Concat("Legend\\" + "\\" + objLayerDetail.layer_name, ".png") + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                        sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                        if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
                        {
                            //string t = Convert.ToString(row["geom"]).Substring(6, Convert.ToString(row["geom"]).Length - 8);

                            string t = Convert.ToString(row["geom"]).Replace("POINT", "").Replace("(", "").Replace(")", "").Trim();

                            sbPoint.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                        }
                        sbPoint.Append("</coordinates></Point></Placemark>");

                    }
                    if (objLayerDetail.geom_type.ToUpper() == "POLYGON")
                    {
                        polycolor = dt.TableName.ToUpper() == "AREA" ? "7fe8dab4" : dt.TableName.ToUpper() == "SUBAREA" ? "7f71e9d7" : "#ff0000ff";
                        //sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToUpper() + "\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>" + polycolor + "</color></PolyStyle></Style><Placemark><name>" + dt.TableName + "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToUpper() + "</styleUrl>");
                        if ((dt.TableName.ToString().ToUpper() == EntityType.Sector.ToString().ToUpper()) && dt.Columns.Contains("Kml Color Code"))
                        {
                            polycolor = Convert.ToString(row["Kml Color Code"]);
                        }

                        sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToString().ToUpper() +
                            "\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>" + polycolor +
                            "</color></PolyStyle></Style><Placemark><name>" + dt.TableName.ToString() +
                            "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description +
                            "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToString().ToUpper() +
                            "</styleUrl>");

                        sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
                        if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
                        {
                            // string t = Convert.ToString(row["geom"]).Substring(9, Convert.ToString(row["geom"]).Length - 11);

                            //string t = Convert.ToString(row["geom"]).Replace("POLYGON", "").Replace("(", "").Replace(")", "").Trim();
                            string[] x = (Convert.ToString(row["geom"]).Replace("POLYGON", "").Replace("(", "").Replace(")", "").Trim()).Split(',');
                            foreach (string y in x)
                            {
                                sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                            }
                        }
                        sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                    }
                }
            }
            #endregion

            //sbLine.Append("</Folder>");
            //sbPoint.Append("</Folder>");
            //sbPolygon.Append("</Folder>");
            StringBuilder filterValues = new StringBuilder();
            filterValues.Append("<table><tr><th>Filter Type</th><th>Value</th></tr>");
            filterValues.AppendLine();
            if (dtFilter != null)
            {
                for (int i = 0; i < dtFilter.Rows.Count; i++)
                {
                    filterValues.Append("<tr><td>" + dtFilter.Rows[i][0] + "</td><td>" + dtFilter.Rows[i][1] + "</td></tr>");
                    filterValues.AppendLine();
                }
            }
            filterValues.Append("</table>");

            finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                               "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                "<filter>" + filterValues + "</filter>" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                               "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                               "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style><Folder>" +
                               sbPolygon.ToString() + sbLine.ToString() + sbPoint.ToString() + "</Folder></Document></kml>";

            return finalKMLString;
        }
		public static string GetKmlForEntityNew(DataTable dt, List<layerReportDetail> lstLayers, DataTable dtFilter = null, string fileName=null, string desTempFolderPath = null)
		{
			string finalKMLString = String.Empty;
			StringBuilder sbLine = new StringBuilder();
			StringBuilder sbPoint = new StringBuilder();
			StringBuilder sbPolygon = new StringBuilder();
			sbLine.Append("<name>Line</name>");
			sbPoint.Append("<name>Point</name>");
			sbPolygon.Append("<name>Polygon</name>");
			string cablecolor = string.Empty;
			string polycolor = string.Empty;
			int lineWidth = 2;
			var objLayerDetail = lstLayers.Where(m => m.layer_title.ToUpper() == dt.TableName.ToString().ToUpper() || m.layer_name.ToUpper() == dt.TableName.ToString().ToUpper()).FirstOrDefault();
				foreach (DataRow row in dt.Rows)
				{
					//var obj = (IDictionary<string, object>)new ExpandoObject();
					StringBuilder description = new StringBuilder();
					string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE", "GEOM_TYPE", "ENTITY_TITLE", "ENTITY_NAME", "GEOM" };
					string[] arrIgnoreColumnskml = { "TOTALRECORDS", "S_NO", "BARCODE" };
					foreach (DataColumn col in dt.Columns)
					{
						//if (!Array.Exists(arrIgnoreColumnskml, m => m == col.ColumnName.ToUpper()))
						//{
						//    //obj.Add(col.ColumnName.ToUpper() == "NETWORK ID" ? "network_id" : (col.ColumnName.ToUpper() == "CABLE TYPE" ? "cable_type" : col.ColumnName), row["NETWORK ID"].ToString());
						//}
						if (!Array.Exists(arrIgnoreColumns, m => m == col.ColumnName.ToUpper()))
						{
							description.Append("<SimpleData name='" + col.ColumnName + "'>" + (String.IsNullOrEmpty(Convert.ToString(row[col.ColumnName])) ? row[col.ColumnName].ToString() : row[col.ColumnName].ToString().Replace("&", "&amp;")) + "</SimpleData>");
						}
					}

					if (objLayerDetail.geom_type.ToUpper() == "LINE")
					{
						if (dt.TableName.ToUpper() == "DUCT" || dt.TableName.ToUpper() == "TRENCH")
						{
							lineWidth = 4;
						}

						cablecolor = dt.TableName.ToUpper() == "DUCT" ? "#f1021f8" : dt.TableName.ToUpper() == "TRENCH" ? "#ff0000ff" : "#ff0000ff";

						if (dt.TableName.ToUpper() == "CABLE")
						{
							if ((dt.Columns.Contains("cable_type")))
							{
								cablecolor = Convert.ToString(row["cable_type"]).ToUpper() == "UNDERGROUND" ? "#ff0000ff" : Convert.ToString(row["cable_type"]).ToUpper() == "OVERHEAD" ? "#f1021f8" : (Convert.ToString(row["cable_type"]).ToUpper() == "WALL MOUNTED" || Convert.ToString(row["cable_type"]).ToUpper() == "ISP") ? "#fa894e7" : "#ff0000ff";
							}
							else
							{
								cablecolor = "#ff0000ff";
							}
						}
						sbLine.Append("<Placemark><name>" + dt.TableName + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
						sbLine.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
						if ((!string.IsNullOrEmpty(Convert.ToString(row["geom"]))))
						{
							// string t = Convert.ToString(row["geom"]).Substring(11, Convert.ToString(row["geom"]).Length - 13);

							//string t = Convert.ToString(row["geom"]).Replace("LINESTRING", "").Replace("(", "").Replace(")", "").Trim();
							string[] x = (Convert.ToString(row["geom"]).Replace("LINESTRING", "").Replace("(", "").Replace(")", "").Trim()).Split(',');
							foreach (string y in x)
							{
								sbLine.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
							}
						}
						sbLine.Append("</coordinates></LineString></Placemark>");
					}
					if (objLayerDetail.geom_type.ToUpper() == "POINT")
					{

						if (dt.TableName != null)
						{

							foreach (var lyr in dt.TableName.ToString())
							{
								var strIconPath = string.Concat(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), objLayerDetail.layer_name, ".png");
								if (File.Exists(strIconPath))
								{
									var legendFolderPath = desTempFolderPath + "\\Legend\\";
									if (Directory.Exists(legendFolderPath).Equals(false))
										Directory.CreateDirectory(legendFolderPath);
									var layerImageName = Path.GetFileName(strIconPath.ToString());
									MiscHelper mh = new MiscHelper();
									if (!System.IO.File.Exists(legendFolderPath + layerImageName))
									{
										mh.CopyFile(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
									}
								}

							}
						}
						sbPoint.Append("<Placemark><name>" + dt.TableName + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + string.Concat("Legend\\" + "\\" + objLayerDetail.layer_name, ".png") + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
						sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
						if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
						{
							//string t = Convert.ToString(row["geom"]).Substring(6, Convert.ToString(row["geom"]).Length - 8);

							string t = Convert.ToString(row["geom"]).Replace("POINT", "").Replace("(", "").Replace(")", "").Trim();

							sbPoint.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
						}
						sbPoint.Append("</coordinates></Point></Placemark>");

					}
					if (objLayerDetail.geom_type.ToUpper() == "POLYGON")
					{
						polycolor = dt.TableName.ToUpper() == "AREA" ? "7fe8dab4" : dt.TableName.ToUpper() == "SUBAREA" ? "7f71e9d7" : "#ff0000ff";
						//sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToUpper() + "\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>" + polycolor + "</color></PolyStyle></Style><Placemark><name>" + dt.TableName + "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToUpper() + "</styleUrl>");
						if ((dt.TableName.ToString().ToUpper() == EntityType.Sector.ToString().ToUpper()) && dt.Columns.Contains("Kml Color Code"))
						{
							polycolor = Convert.ToString(row["Kml Color Code"]);
						}

						sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToString().ToUpper() +
							"\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>" + polycolor +
							"</color></PolyStyle></Style><Placemark><name>" + dt.TableName.ToString() +
							"</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description +
							"</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToString().ToUpper() +
							"</styleUrl>");

						sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
						if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
						{
							// string t = Convert.ToString(row["geom"]).Substring(9, Convert.ToString(row["geom"]).Length - 11);

							//string t = Convert.ToString(row["geom"]).Replace("POLYGON", "").Replace("(", "").Replace(")", "").Trim();
							string[] x = (Convert.ToString(row["geom"]).Replace("POLYGON", "").Replace("(", "").Replace(")", "").Trim()).Split(',');
							foreach (string y in x)
							{
								sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
							}
						}
						sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
					}
				}
			
			

			//sbLine.Append("</Folder>");
			//sbPoint.Append("</Folder>");
			//sbPolygon.Append("</Folder>");
			StringBuilder filterValues = new StringBuilder();
			filterValues.Append("<table><tr><th>Filter Type</th><th>Value</th></tr>");
			filterValues.AppendLine();
			if (dtFilter != null)
			{
				for (int i = 0; i < dtFilter.Rows.Count; i++)
				{
					filterValues.Append("<tr><td>" + dtFilter.Rows[i][0] + "</td><td>" + dtFilter.Rows[i][1] + "</td></tr>");
					filterValues.AppendLine();
				}
			}
			filterValues.Append("</table>");
		

			finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
							   "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
								"<filter>" + filterValues + "</filter>" +
							   "<Document>  <!-- Begin Style Definitions -->" +
							   "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
							   "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style><Folder>" +
							   sbPolygon.ToString() + sbLine.ToString() + sbPoint.ToString() + "</Folder></Document></kml>";
			return finalKMLString;
		}

		public static void GetKmlForFiberLikEntities(DataSet ds, string tempFolderPath)
        {
            string desFolderPath = string.Empty;
            string tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
            string desTempFolderPath = System.Web.Hosting.HostingEnvironment.MapPath(tempFolderPath) + tempFolderName;
            if (Directory.Exists(desTempFolderPath).Equals(false))
                Directory.CreateDirectory(desTempFolderPath);

            StringBuilder sbEntity = new StringBuilder();
            StringBuilder sbEntity1 = new StringBuilder();
            string iconpath = string.Empty;
            string cablecolor = string.Empty;
            int lineWidth = 2;
            string geomType = string.Empty;
            StringBuilder strlinkdetail = new StringBuilder();
            //prepare lindetails string..
            if (ds.Tables.Contains("LinkDetails"))
            {
                foreach (DataRow row in ds.Tables["LinkDetails"].Rows)
                {
                    foreach (DataColumn col in ds.Tables["LinkDetails"].Columns)
                    {
                        strlinkdetail.Append("<SimpleData name='" + (String.IsNullOrEmpty(Convert.ToString(col.ColumnName))) + "' >" + (String.IsNullOrEmpty(Convert.ToString(row[col.ColumnName]))) + "</SimpleData>");
                    }
                }
            }

            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName.ToUpper() == "CABLEINFO")
                {
                    StringBuilder description = new StringBuilder();
                    foreach (DataRow row in dt.Rows)
                    {

                        if (row["cable_type"].ToString().ToUpper() != "ISP")
                        {
                            geomType = row["cable_geom"].ToString().ToUpper().Contains("POINT") ? "POINT" : row["cable_geom"].ToString().ToUpper().Contains("LINE") ? "LINE" : row["cable_geom"].ToString().ToUpper().Contains("POLYGON") ? "POLYGON" : "";
                            var PortNo = row["fiber_number"].ToString();
                            string portDetail = " (" + PortNo + ")";
                            var entity_type = "Cable";

                            description.Append("<SimpleData name='Network Id'>" + row["cable_network_id"].ToString() + "</SimpleData>");
                            description.Append("<SimpleData name='Port'>" + PortNo + "</SimpleData>");
                            if (String.IsNullOrEmpty(strlinkdetail.ToString()))
                            {
                                description.Append(strlinkdetail);

                            }
                            //cablecolor = "#0000FF";
                            cablecolor = "#ffff0000";

                            sbEntity.Append("<Placemark><name>" + entity_type + " (" + row["cable_network_id"] + ")" + portDetail + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");//<mode>dashed</mode><dashsize>5</dashsize><alternatecolor>ff000000</alternatecolor>
                                                                                                                                                                                                                                                                                                                                                            //sbEntity.Append("<Placemark><name>'Fiber Link Details'</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");//<mode>dashed</mode><dashsize>5</dashsize><alternatecolor>ff000000</alternatecolor>
                            sbEntity.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(row["cable_geom"].ToString()))
                            {
                                string t = row["cable_geom"].ToString().Substring(11, row["cable_geom"].ToString().Length - 13);
                                string[] x = t.Split(',');
                                foreach (string y in x)
                                {
                                    sbEntity.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                                }
                            }
                            sbEntity.Append("</coordinates></LineString></Placemark>");

                        }
                    }
                }


                if (dt.TableName.ToUpper() == "CONNECTEDELEMENTS")
                {
                    // PREPARE POINT ENTITY LEGEND..
                    DataView view = new DataView(dt);
                    DataTable distinctValues = view.ToTable(true, "connected_entity_type");
                    if (distinctValues.Rows.Count > 0)
                    {

                        foreach (DataRow lyr in distinctValues.Rows)
                        {
                            var strIconPath = string.Concat(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), "v_", lyr["connected_entity_type"], ".png");
                            if (File.Exists(strIconPath))
                            {
                                var legendFolderPath = desTempFolderPath + "\\Legend\\";
                                if (Directory.Exists(legendFolderPath).Equals(false))
                                    Directory.CreateDirectory(legendFolderPath);
                                var layerImageName = Path.GetFileName(strIconPath.ToString());
                                MiscHelper mh = new MiscHelper();
                                if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                                {
                                    mh.CopyFile(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
                                }
                            }

                        }
                    }
                    if (distinctValues.Rows.Count > 0)
                    {

                        foreach (DataRow lyr in distinctValues.Rows)
                        {
                            var strIconPath = string.Concat(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), lyr["connected_entity_type"], ".png");
                            if (File.Exists(strIconPath))
                            {
                                var legendFolderPath = desTempFolderPath + "\\Legend\\";
                                if (Directory.Exists(legendFolderPath).Equals(false))
                                    Directory.CreateDirectory(legendFolderPath);
                                var layerImageName = Path.GetFileName(strIconPath.ToString());
                                MiscHelper mh = new MiscHelper();
                                if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                                {
                                    mh.CopyFile(System.Web.Hosting.HostingEnvironment.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
                                }
                            }

                        }
                    }
                    foreach (DataRow row in dt.Rows)
                    {
                        StringBuilder description1 = new StringBuilder();
                        var PortNo = row["connected_port_no"].ToString();
                        description1.Append("<SimpleData name='Network Id'>" + row["connected_network_id"].ToString() + "</SimpleData>");
                        description1.Append("<SimpleData name='Port'>" + PortNo + "</SimpleData>");
                        if (Convert.ToBoolean(row["is_virtual"]))
                        {
                            iconpath = string.Concat("Legend\\" + "\\" + "v_" + row["connected_entity_type"].ToString().ToUpper() + ".png");
                        }
                        else
                        {
                            iconpath = string.Concat("Legend\\" + "\\" + row["connected_entity_type"].ToString().ToUpper() + ".png");
                        }
                        //iconpath = Utility.CommonUtility.GetIconURL(ApplicationSettings.KMLIconURL.ToString(), ApplicationSettings.KMLIconURL.ToString() + entityList[k].connected_entity_type.ToUpper() + ".png");

                        if (!string.IsNullOrEmpty(Convert.ToString(row["connected_entity_type"])))
                        {
                            sbEntity1.Append("<Placemark><name>" + row["connected_entity_type"] + " (" + row["connected_network_id"] + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description1 + "</SchemaData></ExtendedData>");// to reduce size of icon put <scale>.5</scale> under icon tab 1 for actual size and anything less than 1 (0.1 - 0.9) - reduces the size of icon
                        }
                        //else
                        //{
                        //    sbEntity1.Append("<Placemark><name>" + entityList[k].connected_entity_type + " (" + entityList[k].connected_network_id + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description1 + "</SchemaData></ExtendedData>");
                        //}
                        sbEntity1.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                        if (!string.IsNullOrEmpty(Convert.ToString(row["connected_entity_geom"])))
                        {
                            string t = row["connected_entity_geom"].ToString().Substring(6, row["connected_entity_geom"].ToString().Length - 8);
                            sbEntity1.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                        }
                        sbEntity1.Append("</coordinates></Point></Placemark>");
                    }
                }
            }
            string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                                "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                                 "<Document>  <!-- Begin Style Definitions -->" +
                                                "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                                "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                               sbEntity.ToString() + sbEntity1.ToString() + "</Document></kml>";

            string TempkmlFileName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
            string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
            System.IO.File.WriteAllText(kmlDesFullPath, finalKMLString.ToString());
            string zipfilePath = desTempFolderPath + ".zip";

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(desTempFolderPath);
                zip.Save(zipfilePath);
            }
            if (System.IO.File.Exists(zipfilePath))
            {
                string fileName = Path.GetFileName(zipfilePath);
                Directory.Delete(desTempFolderPath, true);
            }
            FileInfo file = new FileInfo(zipfilePath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export_FiberLinkConnection_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/zip";
            HttpContext.Current.Response.WriteFile(file.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
            File.Delete(zipfilePath);

        }

        public static void GetKmlForSplicedEntities(DataSet ds, string tempFolderPath)
        {
            string desFolderPath = string.Empty;
            string tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
            string desTempFolderPath = HttpContext.Current.Server.MapPath(tempFolderPath) + tempFolderName;
            if (Directory.Exists(desTempFolderPath).Equals(false))
                Directory.CreateDirectory(desTempFolderPath);

            StringBuilder sbEntity = new StringBuilder();

            string iconpath = string.Empty;
            string cablecolor = string.Empty;
            int lineWidth = 2;
            string geomType = string.Empty;

            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName.ToUpper() == "CPFELEMENTS")
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        geomType = row["sp_geometry"].ToString().ToUpper().Contains("POINT") ? "POINT" : row["sp_geometry"].ToString().ToUpper().Contains("LINE") ? "LINE" : row["sp_geometry"].ToString().ToUpper().Contains("POLYGON") ? "POLYGON" : "";
                        var PortNo = Convert.ToBoolean(row["is_virtual_port_allowed"]) == false ? Convert.ToInt32(row["port_no"]) > 0 ? row["port_no"].ToString() + " OUT" : row["port_no"].ToString().Replace("-", "") + " IN" : row["port_no"].ToString();
                        string portDetail = Convert.ToBoolean(row["is_virtual_port_allowed"]) == false ? " (" + PortNo + ")" : "";
                        StringBuilder description = new StringBuilder();
                        description.Append("<SimpleData name='Network Id'>" + row["display_name"].ToString() + "</SimpleData>");
                        description.Append("<SimpleData name='Port'>" + PortNo + "</SimpleData>");
                        if (geomType.ToUpper() == "LINE")
                        {
                            if (row["en_type"].ToString().ToUpper() == "CABLE")
                            {
                                if (Convert.ToBoolean(row["backward_path"]) == true)
                                {
                                    cablecolor = "#ffff0000";
                                }
                                else
                                {
                                    cablecolor = "#ff0000ff";
                                }

                            }
                            sbEntity.Append("<Placemark><name>" + row["en_type"].ToString() + " (" + row["display_name"].ToString() + ")" + portDetail + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");//<mode>dashed</mode><dashsize>5</dashsize><alternatecolor>ff000000</alternatecolor>
                                                                                                                                                                                                                                                                                                                                                                                 //sbEntity.Append("<Placemark><name>" + entityList[k].en_type + " (" + entityList[k].network_id + ")" + portDetail + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");//<mode>dashed</mode><dashsize>5</dashsize><alternatecolor>ff000000</alternatecolor>
                            sbEntity.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(row["sp_geometry"].ToString()))
                            {
                                string t = row["sp_geometry"].ToString().Substring(11, row["sp_geometry"].ToString().Length - (11 + (row["sp_geometry"].ToString().Length - row["sp_geometry"].ToString().IndexOf(")"))));
                                // string t = row["sp_geometry"].ToString().Substring(11, row["sp_geometry"].ToString().Length - 13);
                                string[] x = t.Split(',');
                                foreach (string y in x)
                                {
                                    sbEntity.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                                }
                            }
                            sbEntity.Append("</coordinates></LineString></Placemark>");
                        }
                        if (geomType.ToUpper() == "POINT")
                        {

                            // PREPARE POINT ENTITY LEGEND..
                            DataView view = new DataView(dt);
                            DataTable distinctValues = view.ToTable(true, "en_type");
                            if (distinctValues.Rows.Count > 0)
                            {
                                foreach (DataRow lyr in distinctValues.Rows)
                                {
                                    if (lyr["en_type"].ToString().ToUpper() != "CABLE")
                                    {
                                        var strIconPath = string.Concat(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), lyr["en_type"], ".png");
                                        if (File.Exists(strIconPath))
                                        {
                                            var legendFolderPath = desTempFolderPath + "\\Legend\\";
                                            if (Directory.Exists(legendFolderPath).Equals(false))
                                                Directory.CreateDirectory(legendFolderPath);
                                            var layerImageName = Path.GetFileName(strIconPath.ToString());
                                            MiscHelper mh = new MiscHelper();
                                            if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                                            {
                                                mh.CopyFile(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
                                            }
                                        }
                                    }

                                }
                            }
                            iconpath = string.Concat("Legend\\" + "\\" + row["en_type"].ToString().ToUpper() + ".png");
                            sbEntity.Append("<Placemark><name>" + row["entity_title"].ToString() + " (" + row["display_name"].ToString() + ")" + portDetail + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");// to reduce size of icon put <scale>.5</scale> under icon tab 1 for actual size and anything less than 1 (0.1 - 0.9) - reduces the size of icon
                            sbEntity.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                            if (!string.IsNullOrEmpty(row["sp_geometry"].ToString()))
                            {
                                //string t = row["sp_geometry"].ToString().Substring(6, row["sp_geometry"].ToString().Length - 8);
                                string t = row["sp_geometry"].ToString().Substring(6, row["sp_geometry"].ToString().Length - (6 + (row["sp_geometry"].ToString().Length - row["sp_geometry"].ToString().IndexOf(")"))));
                                sbEntity.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                            }
                            sbEntity.Append("</coordinates></Point></Placemark>");

                        }
                    }
                    string finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                        "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                         "<Document>  <!-- Begin Style Definitions -->" +
                                        "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                        "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                       sbEntity.ToString() + "</Document></kml>";

                    string TempkmlFileName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
                    string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
                    System.IO.File.WriteAllText(kmlDesFullPath, finalKMLString.ToString());
                    string zipfilePath = desTempFolderPath + ".zip";

                    using (var zip = new ZipFile())
                    {
                        zip.AddDirectory(desTempFolderPath);
                        zip.Save(zipfilePath);
                    }
                    if (System.IO.File.Exists(zipfilePath))
                    {
                        string fileName = Path.GetFileName(zipfilePath);
                        Directory.Delete(desTempFolderPath, true);
                    }
                    FileInfo file = new FileInfo(zipfilePath);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export_ConnectionPathFinder_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
                    HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                    HttpContext.Current.Response.ContentType = "application/zip";
                    HttpContext.Current.Response.WriteFile(file.FullName);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                    File.Delete(zipfilePath);

                }
            }
        }

        public static void GetKMLForAllFiberLinkEntities(List<Dictionary<string, string>> lstAllFiberLinkDetails, string tempFolderPath, int user_id)
        {
            string desFolderPath = string.Empty;
            string tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
            string desTempFolderPath = HttpContext.Current.Server.MapPath(tempFolderPath) + tempFolderName;
            if (Directory.Exists(desTempFolderPath).Equals(false))
                Directory.CreateDirectory(desTempFolderPath);

            string iconpath = string.Empty;
            string cablecolor = string.Empty;
            int lineWidth = 2;
            string geomType = string.Empty;
            string finalKMLString = string.Empty;
            StringBuilder sbEntity = new StringBuilder();
            Dictionary<string, string> LinkDetails = new Dictionary<string, string>();
            JsonResponse<vmfiberLinkOnMap> objResp = new JsonResponse<vmfiberLinkOnMap>();
            #region ANTRA
            foreach (var item in lstAllFiberLinkDetails)
            {
                objResp.result = new BLFiberLink().getFiberLinkElementsByLinkSystemIds(item["system_id"], user_id);
                LinkDetails = new BLFiberLink().getLinkInfoForKML(Convert.ToInt32(item["system_id"]));
                sbEntity.Append("<Folder><name>" + item["link_id"] + "(" + (item["network_id"]) + ")</name>");
                if (objResp.result != null && objResp.result.lstCableInfo != null)
                {
                    var lstCableInfo = objResp.result.lstCableInfo.ToList();
                    for (int k = 0; k < lstCableInfo.Count; k++)
                    {
                        if (lstCableInfo[k].cable_type.ToUpper() != "ISP")
                        {
                            StringBuilder description = new StringBuilder();

                            geomType = lstCableInfo[k].cable_geom.ToUpper().Contains("POINT") ? "POINT" : lstCableInfo[k].cable_geom.ToUpper().Contains("LINE") ? "LINE" : lstCableInfo[k].cable_geom.ToUpper().Contains("POLYGON") ? "POLYGON" : "";
                            var PortNo = lstCableInfo[k].fiber_number;
                            string portDetail = " (" + PortNo + ")";
                            var entity_type = "Cable";

                            description.Append("<SimpleData name='Network Id'>" + lstCableInfo[k].cable_network_id + "</SimpleData>");
                            description.Append("<SimpleData name='Port'>" + PortNo + "</SimpleData>");
                            if (LinkDetails != null)
                            {
                                foreach (var info in LinkDetails)
                                {
                                    description.Append("<SimpleData name='" + info.Key + "' >" + info.Value + "</SimpleData>");
                                }
                            }
                            //cablecolor = "#0000FF";
                            cablecolor = "#ffff0000";

                            sbEntity.Append("<Placemark><name>" + entity_type + " (" + lstCableInfo[k].cable_network_id + ")" + portDetail + "</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");//<mode>dashed</mode><dashsize>5</dashsize><alternatecolor>ff000000</alternatecolor>
                            sbEntity.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(lstCableInfo[k].cable_geom))
                            {
                                string t = lstCableInfo[k].cable_geom.Substring(11, lstCableInfo[k].cable_geom.Length - 13);
                                string[] x = t.Split(',');
                                foreach (string y in x)
                                {
                                    sbEntity.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                                }
                            }
                            sbEntity.Append("</coordinates></LineString></Placemark>");
                        }
                    }

                }
                if (objResp.result != null && objResp.result.lstConnectedElements != null)
                {
                    var entityList = objResp.result.lstConnectedElements.ToList();
                    var a = entityList.Select(x => x.connected_entity_type).Distinct().ToList();
                    // PREPARE POINT ENTITY LEGEND..
                    for (int i = 0; i < a.Count; i++)
                    {
                        var strIconPath = string.Concat(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), a[i], ".png");
                        if (File.Exists(strIconPath))
                        {
                            var legendFolderPath = desTempFolderPath + "\\Legend\\";
                            if (Directory.Exists(legendFolderPath).Equals(false))
                                Directory.CreateDirectory(legendFolderPath);
                            var layerImageName = Path.GetFileName(strIconPath.ToString());
                            MiscHelper mh = new MiscHelper();
                            if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                            {
                                mh.CopyFile(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
                            }
                        }
                    }
                    for (int k = 0; k < entityList.Count; k++)
                    {
                        StringBuilder description1 = new StringBuilder();
                        var PortNo = entityList[k].connected_port_no;
                        string portDetails = " (" + PortNo + ")";
                        description1.Append("<SimpleData name='Network Id'>" + entityList[k].connected_network_id + "</SimpleData>");
                        description1.Append("<SimpleData name='Port'>" + PortNo + "</SimpleData>");
                        if (entityList[k].is_virtual)
                        {
                            iconpath = string.Concat("Legend\\" + "\\" + "v_" + entityList[k].connected_entity_type.ToUpper() + ".png");
                        }
                        else
                        {
                            iconpath = string.Concat("Legend\\" + "\\" + entityList[k].connected_entity_type.ToUpper() + ".png");
                        }

                        if (!string.IsNullOrEmpty(entityList[k].connected_entity_type))
                        {
                            sbEntity.Append("<Placemark><name>" + entityList[k].connected_entity_type + " (" + entityList[k].connected_network_id + ")" + " </name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description1 + "</SchemaData></ExtendedData>");// to reduce size of icon put <scale>.5</scale> under icon tab 1 for actual size and anything less than 1 (0.1 - 0.9) - reduces the size of icon
                        }
                        else
                        {
                            sbEntity.Append("<Placemark><name>" + entityList[k].connected_entity_type + " (" + entityList[k].connected_network_id + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description1 + "</SchemaData></ExtendedData>");
                        }
                        sbEntity.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                        if (!string.IsNullOrEmpty(entityList[k].connected_entity_geom))
                        {
                            string t = entityList[k].connected_entity_geom.Substring(6, entityList[k].connected_entity_geom.Length - 8);
                            sbEntity.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                        }
                        sbEntity.Append("</coordinates></Point></Placemark>");
                    }

                }

                sbEntity.Append("</Folder>");

            }
            finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                   "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                    "<Document>  <!-- Begin Style Definitions -->" +
                                   "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                   "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                  sbEntity.ToString() + "</Document></kml>";

            string TempkmlFileName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
            string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
            System.IO.File.WriteAllText(kmlDesFullPath, finalKMLString.ToString());
            string zipfilePath = desTempFolderPath + ".zip";

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(desTempFolderPath);
                zip.Save(zipfilePath);
            }
            if (System.IO.File.Exists(zipfilePath))
            {
                string fileName = Path.GetFileName(zipfilePath);
                Directory.Delete(desTempFolderPath, true);
            }
            FileInfo file = new FileInfo(zipfilePath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export_FiberLinkConnection_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/zip";
            HttpContext.Current.Response.WriteFile(file.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
            File.Delete(zipfilePath);
            #endregion
        }
        public static void GetKMLForLMCEntities(DataSet ds, string tempFolderPath, ExportLMCReportFilter objReportFilter)
        {

            string desFolderPath = string.Empty;
            string tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
            string desTempFolderPath = HttpContext.Current.Server.MapPath(tempFolderPath) + tempFolderName;
            if (Directory.Exists(desTempFolderPath).Equals(false))
                Directory.CreateDirectory(desTempFolderPath);

            string finalKMLString = String.Empty;
            string iconpath = string.Empty;
            string cablecolor = string.Empty;
            int lineWidth = 2;
            string geomType = string.Empty;
            StringBuilder sbEntity = new StringBuilder();
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName.ToUpper() == "LMCREPORT")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        sbEntity.Append("<Folder><name>" + objReportFilter.entityType + "</name>");

                        if (objReportFilter.entityType == "Cable")
                        {
                            StringBuilder description = new StringBuilder();
                            geomType = row["geom"].ToString().ToUpper().Contains("POINT") ? "POINT" : row["geom"].ToString().ToUpper().Contains("LINE") ? "LINE" : row["geom"].ToString().ToUpper().Contains("POLYGON") ? "POLYGON" : "";
                            var entity_type = objReportFilter.entityType;
                            cablecolor = "#ffff0000";

                            foreach (DataRow rows in ds.Tables["LMCREPORT"].Rows)
                            {
                                foreach (DataColumn col in ds.Tables["LMCREPORT"].Columns)
                                {
                                    if (col.ColumnName != "geom")
                                    {
                                        description.Append("<SimpleData name='" + col.ColumnName + "'>" + row[col.ColumnName] + "</SimpleData>");
                                    }
                                }

                            }
                            sbEntity.Append("<Placemark><name>" + entity_type + " (" + row["Cable Network Id"].ToString() + ")</name><Style id=\"cable\"><LineStyle><color>" + cablecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");//<mode>dashed</mode><dashsize>5</dashsize><alternatecolor>ff000000</alternatecolor>
                            sbEntity.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                            if (!string.IsNullOrEmpty(row["geom"].ToString()))
                            {
                                string t = row["geom"].ToString().Substring(11, row["geom"].ToString().Length - 13);
                                string[] x = t.Split(',');
                                foreach (string y in x)
                                {
                                    sbEntity.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                                }
                            }
                            sbEntity.Append("</coordinates></LineString></Placemark>");
                        }
                        else
                        {
                            // PREPARE POINT ENTITY LEGEND..
                            var pointLayers = objReportFilter.entityType.ToString();

                            var strIconPath = string.Concat(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), pointLayers, ".png");
                            if (File.Exists(strIconPath))
                            {
                                var legendFolderPath = desTempFolderPath + "\\Legend\\";
                                if (Directory.Exists(legendFolderPath).Equals(false))
                                    Directory.CreateDirectory(legendFolderPath);
                                var layerImageName = Path.GetFileName(strIconPath.ToString());
                                MiscHelper mh = new MiscHelper();
                                if (!System.IO.File.Exists(legendFolderPath + layerImageName))
                                {
                                    mh.CopyFile(HttpContext.Current.Server.MapPath(ApplicationSettings.KMLIconURL), legendFolderPath, layerImageName, layerImageName);
                                }
                            }

                            StringBuilder description = new StringBuilder();
                            foreach (DataRow rows in ds.Tables["LMCREPORT"].Rows)
                            {
                                foreach (DataColumn col in ds.Tables["LMCREPORT"].Columns)
                                {
                                    if (col.ColumnName != "geom")
                                    {
                                        description.Append("<SimpleData name='" + col.ColumnName + "'>" + row[col.ColumnName] + "</SimpleData>");
                                    }
                                }

                            }
                            if (objReportFilter.entityType == "Customer")
                            {
                                iconpath = string.Concat("Legend\\" + "\\" + objReportFilter.entityType.ToUpper() + ".png");
                                sbEntity.Append("<Placemark><name>" + objReportFilter.entityType + " (" + row["Can Id"].ToString() + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                            }
                            else if (objReportFilter.entityType == "Site")
                            {
                                iconpath = string.Concat("Legend\\" + "\\" + "STRUCTURE" + ".png");
                                sbEntity.Append("<Placemark><name>" + objReportFilter.entityType + " (" + row["Structure Network Id"].ToString() + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + iconpath + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                            }

                            sbEntity.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                            if (!string.IsNullOrEmpty(row["geom"].ToString()))
                            {
                                string t = row["geom"].ToString().Substring(6, row["geom"].ToString().Length - 8);
                                sbEntity.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                            }
                            sbEntity.Append("</coordinates></Point></Placemark>");
                        }

                        sbEntity.Append("</Folder>");
                    }
                    finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                                "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                                "<Style id=\"cable\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                                "<Style id=\"transGreenPoly\"><LineStyle><color>#ff0000ff</color><width>2</width></LineStyle><PolyStyle><color>AEAEAC</color></PolyStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" +
                                "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                                sbEntity.ToString() + "</Document></kml>";
                    string TempkmlFileName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".kml";
                    string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
                    System.IO.File.WriteAllText(kmlDesFullPath, finalKMLString.ToString());
                    string zipfilePath = desTempFolderPath + ".zip";

                    using (var zip = new ZipFile())
                    {
                        zip.AddDirectory(desTempFolderPath);
                        zip.Save(zipfilePath);
                    }
                    if (System.IO.File.Exists(zipfilePath))
                    {
                        string fileName = Path.GetFileName(zipfilePath);
                        Directory.Delete(desTempFolderPath, true);
                    }
                    FileInfo file = new FileInfo(zipfilePath);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export_ConnectionPathFinder_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
                    HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                    HttpContext.Current.Response.ContentType = "application/zip";
                    HttpContext.Current.Response.WriteFile(file.FullName);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                    File.Delete(zipfilePath);

                }
            }
        }

        private static string RemoveInvalidXmlChars(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder output = new StringBuilder();
            foreach (char ch in input)
            {
                // Valid XML 1.0 characters
                if (ch == '\t' || ch == '\n' || ch == '\r' || (ch >= ' ' && ch <= '\uD7FF') || (ch >= '\uE000' && ch <= '\uFFFD'))
                {
                    output.Append(ch);
                }
            }
            return output.ToString();
        }

        /*-LANDBASE-*/
        public static void LandBaseDatasetToKML(DataSet ds, List<LandBaseMaster> lstLayers, string tempFolderPath, string TempkmlFileName, string layerName = "", DataTable dtFilter = null)
        {
            string desFolderPath = string.Empty;
            string tempFolderName = "KML_" + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss");
            string desTempFolderPath = HttpContext.Current.Server.MapPath(tempFolderPath) + tempFolderName;
            if (Directory.Exists(desTempFolderPath).Equals(false))
                Directory.CreateDirectory(desTempFolderPath);

            string finalkml = GetKmlForLandBaseLayer(ds, lstLayers, tempFolderPath, dtFilter, desTempFolderPath);
            string kmlDesFullPath = desTempFolderPath + "\\" + TempkmlFileName;
            System.IO.File.WriteAllText(kmlDesFullPath, finalkml.ToString());
            string zipfilePath = desTempFolderPath + ".zip";

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(desTempFolderPath);
                zip.Save(zipfilePath);
            }
            if (System.IO.File.Exists(zipfilePath))
            {
                string fileName = Path.GetFileName(zipfilePath);
                Directory.Delete(desTempFolderPath, true);
            }
            FileInfo file = new FileInfo(zipfilePath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export_KML_" + layerName + DateTimeHelper.Now.ToString("ddMMyyyy") + " - " + DateTimeHelper.Now.ToString("HHmmss") + ".zip");
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/zip";
            HttpContext.Current.Response.WriteFile(file.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
            File.Delete(zipfilePath);
        }


        public static string GetKmlForLandBaseLayer(DataSet ds, List<LandBaseMaster> lstLayers, string tempFolderPath, DataTable dtFilter = null, string desTempFolderPath = null)
        {
            string finalKMLString = String.Empty;
            StringBuilder sbLine = new StringBuilder();
            StringBuilder sbPoint = new StringBuilder();
            StringBuilder sbPolygon = new StringBuilder();
            sbLine.Append("<Folder><name>Line</name>");
            sbPoint.Append("<Folder><name>Point</name>");
            sbPolygon.Append("<Folder><name>Polygon</name>");
            string Linecolor = string.Empty;
            string polycolor = string.Empty;
            string pointcolor = string.Empty;
            string polybordercolor = string.Empty;
            int polyborderwidth = 2;
            int lineWidth = 2;
            #region ANTRA
            foreach (DataTable dt in ds.Tables)
            {

                var objLayerDetail = lstLayers.Where(m => m.layer_name.ToUpper() == dt.TableName.ToString().ToUpper()).FirstOrDefault();

                // Last 2 digit of the 8 digit color is the opacity of the color. 00 represents a fully transparent && FF represents a fully opaque color.

                List<string> result = new List<string>(System.Text.RegularExpressions.Regex.Split(objLayerDetail.map_border_color.Replace("#", ""), @"(?<=\G.{2})", System.Text.RegularExpressions.RegexOptions.Singleline));
                string clrsKML = "" + result[2] + result[1] + result[0];
                Linecolor = polybordercolor = "#EE" + clrsKML;
                List<string> result2 = new List<string>(System.Text.RegularExpressions.Regex.Split(objLayerDetail.map_bg_color.Replace("#", ""), @"(?<=\G.{2})", System.Text.RegularExpressions.RegexOptions.Singleline));
                string clrsKML2 = "" + result2[2] + result2[1] + result2[0];
                polycolor = "#70" + clrsKML2;


                //------------------------
                foreach (DataRow row in dt.Rows)
                {
                    StringBuilder description = new StringBuilder();
                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE", "GEOM_TYPE", "ENTITY_TITLE", "ENTITY_NAME", "GEOM", "BUFFER_GEOM" };
                    string[] arrIgnoreColumnskml = { "TOTALRECORDS", "S_NO", "BARCODE", "BUFFER_GEOM" };
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (!Array.Exists(arrIgnoreColumns, m => m == col.ColumnName.ToUpper()))
                        {
                            description.Append("<SimpleData name='" + col.ColumnName + "'>" + (String.IsNullOrEmpty(Convert.ToString(row[col.ColumnName])) ? row[col.ColumnName].ToString() : row[col.ColumnName].ToString().Replace("&", "&amp;")) + "</SimpleData>");
                        }
                    }

                    if (objLayerDetail.geom_type.ToUpper() == "LINE")
                    {
                        sbLine.Append("<Placemark><name>" + dt.TableName + " (" + Convert.ToString(row["Network Id"]) + ")" + "</name><Style id=\"line\"><LineStyle><color>" + Linecolor + "</color><width>" + lineWidth + "</width></LineStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                        sbLine.Append("<styleUrl>#line</styleUrl><LineString><coordinates>");
                        if ((!string.IsNullOrEmpty(Convert.ToString(row["geom"]))))
                        {
                           // string t = Convert.ToString(row["geom"]).Substring(11, Convert.ToString(row["geom"]).Length - 13);

                            string t = Convert.ToString(row["geom"]).Replace("LINESTRING", "").Replace("(", "").Replace(")", "").Trim();

                            string[] x = t.Split(',');
                            foreach (string y in x)
                            {
                                sbLine.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                            }
                        }
                        sbLine.Append("</coordinates></LineString></Placemark>");
                    }

                    // Commented by Abhimanyu on 01/10/2021 ------ Not required Buffer Geom value in KML ///////// 
                    //if (objLayerDetail.geom_type.ToUpper() == "LINE" && objLayerDetail.is_center_line_enable && (!string.IsNullOrEmpty(Convert.ToString(row["buffer_geom"]))))
                    //{
                    //    sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToString().ToUpper() +
                    //        "\"><LineStyle><color>" + polybordercolor + "</color><width>" + polyborderwidth + "</width></LineStyle><PolyStyle><color>" + polycolor +
                    //        "</color></PolyStyle></Style><Placemark><name>" + dt.TableName + " (" + Convert.ToString(row["Network Id"]) + ")" +
                    //        "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description +
                    //        "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToString().ToUpper() +
                    //        "</styleUrl>");

                    //    sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
                    //    if (!string.IsNullOrEmpty(Convert.ToString(row["buffer_geom"])))
                    //    {
                    //        string t = Convert.ToString(row["buffer_geom"]).Substring(9, Convert.ToString(row["buffer_geom"]).Length - 12);
                    //        string[] x = t.Split(',');
                    //        foreach (string y in x)
                    //        {
                    //            sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                    //        }
                    //    }
                    //    sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                    //}
                    if (objLayerDetail.geom_type.ToUpper() == "POINT")
                    {
                        sbPoint.Append("<Placemark><name>" + dt.TableName + " (" + Convert.ToString(row["Network Id"]) + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><color>" + pointcolor + "</color><Icon><scale>.75</scale><href>https://maps.google.com/mapfiles/kml/paddle/red-blank.png</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                        //sbPoint.Append("<Placemark><name>" + dt.TableName + " (" + Convert.ToString(row["Network Id"]) + ")" + "</name><Style id=\"downArrowIcon\"><IconStyle><Icon><scale>.75</scale><href>" + string.Concat("Legend\\" + "\\" + objLayerDetail.layer_name, ".png") + "</href></Icon></IconStyle></Style><ExtendedData><SchemaData schemaUrl='#UG'>" + description + "</SchemaData></ExtendedData>");
                        sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                        if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
                        {
                            //string t = Convert.ToString(row["geom"]).Substring(6, Convert.ToString(row["geom"]).Length - 8);   
                            string t = Convert.ToString(row["geom"]).Replace("POINT", "").Replace("(", "").Replace(")", "").Trim();  
                            sbPoint.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                        }
                        sbPoint.Append("</coordinates></Point></Placemark>");

                    }
                    if (objLayerDetail.geom_type.ToUpper() == "POLYGON")
                    {
                        sbPolygon.Append("<Style id=\"transGreenPoly" + dt.TableName.ToString().ToUpper() +
                            "\"><LineStyle><color>" + polybordercolor + "</color><width>" + polyborderwidth + "</width></LineStyle><PolyStyle><color>" + polycolor +
                            "</color></PolyStyle></Style><Placemark><name>" + dt.TableName + " (" + Convert.ToString(row["Network Id"]) + ")" +
                            "</name><ExtendedData><SchemaData schemaUrl='#UG'>" + description +
                            "</SchemaData></ExtendedData><styleUrl>#transGreenPoly" + dt.TableName.ToString().ToUpper() +
                            "</styleUrl>");
                        sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
                        if (!string.IsNullOrEmpty(Convert.ToString(row["geom"])))
                        {
                           // string t = Convert.ToString(row["geom"]).Substring(9, Convert.ToString(row["geom"]).Length - 11);
                            string t = Convert.ToString(row["geom"]).Replace("POLYGON", "").Replace("(", "").Replace(")", "").Trim();

                            string[] x = t.Split(',');
                            foreach (string y in x)
                            {
                                sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                            }
                        }
                        sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                    }
                }
            }
            #endregion

            sbLine.Append("</Folder>");
            sbPoint.Append("</Folder>");
            sbPolygon.Append("</Folder>");
            StringBuilder filterValues = new StringBuilder();
            filterValues.Append("<table><tr><th>Filter Type</th><th>Value</th></tr>");
            filterValues.AppendLine();
            if (dtFilter != null)
            {
                for (int i = 0; i < dtFilter.Rows.Count; i++)
                {
                    filterValues.Append("<tr><td>" + dtFilter.Rows[i][0] + "</td><td>" + dtFilter.Rows[i][1] + "</td></tr>");
                    filterValues.AppendLine();
                }
            }
            filterValues.Append("</table>");

            finalKMLString = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                               "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                                "<filter>" + filterValues + "</filter>" +
                               "<Document>  <!-- Begin Style Definitions -->" +
                             // "<Style id=\"logical\"><LineStyle><color>#ff0000ff</color><width>4</width></LineStyle></Style>" +
                             "<Style id=\"logical\"><LineStyle><color>" + polycolor + "</color><width>4</width></LineStyle></Style>" +
                               "<Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                               sbPolygon.ToString() + sbLine.ToString() + sbPoint.ToString() + "</Document></kml>";

            return finalKMLString;
        }
    }
}

