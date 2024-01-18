using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO.Compression;
using SharpKml.Engine;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Runtime.InteropServices.WindowsRuntime;
//using Ionic.Zip;

namespace Lepton.GISConvertor
{
    public class ConvertorResponse
    {
        public bool Status { get; set; }
        public dynamic Output { get; set; }
        public string Message { get; set; }
        public string OutputFile { get; set; }
        public string OutputType { get; set; }
    }
    public class Convertor
    {
        private string InputFolder { get; set; }
        private string BaseFolder { get; set; }
        private string ProjectName { get; set; }
        private string KmlFolder { get; set; }
        private string GeoJsonFolder { get; set; }
        private string DirPath { get; set; }
        private string KmlFilePath { get; set; }
        private string KmlFileName { get; set; }
        private string SourceSrid { get; set; }
        public ConvertorResponse response { get; set; }
        public Convertor(string basefolder, string projectName, string kmlFolder, string geoJsonFolder)
        {
            BaseFolder = basefolder;
            ProjectName = "GIS_Convertor\\";
            DirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\"));
            KmlFolder = kmlFolder;
            GeoJsonFolder = geoJsonFolder;
            response = new ConvertorResponse();

        }
        public Convertor()
        {
            BaseFolder = "GIS_Convertor\\upload\\";
            ProjectName = "GIS_Convertor\\";
            DirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\"));
            KmlFolder = DirPath + BaseFolder + "KML\\";
            GeoJsonFolder = DirPath + BaseFolder + "GeoJson\\";
            response = new ConvertorResponse();
        }

        /// <summary>
        /// Support file type for KML conversion Shape, Tab, Kmz (Parameters value inside the request object)
        /// </summary>
        /// <param name="filetype"> i.e. Shape, Tab, Kmz</param>
        ///  <param name="txtfilename"> i.e. abc, xyz</param>
        /// <returns></returns>
        public ConvertorResponse getKML(HttpRequestBase Request)
        {
            InputFolder = DirPath + BaseFolder + Request.Form["filetype"];
            string file_extention = string.Empty;

            //Folder check
            if (!Directory.Exists(InputFolder))
            {
                response.Status = false;
                response.Message = InputFolder + ", Please create required folders.";
                return response;
            }

            //Check all required filed uploaded or Not
            List<string> missingfiles = new List<string>();
            missingfiles = ValidateExtention(Request, out file_extention);
            if (missingfiles.Count > 0)
            {
                response.Status = false;
                response.Message = "Please provide file with following extentions : " + string.Join(",", missingfiles);
                return response;
            }

            //Save valid files to respective folder
            bool savefile = SaveFiletoLocal(Request);
            if (savefile)
            {
                if (ConvertFileToKML(file_extention))
                {
                    response.Status = true;
                    response.Message = "File conversion successful.";
                    response.OutputType = "KML";
                    response.Output = File.ReadAllText(response.OutputFile);
                }
            }
            return response;
        }
        public ConvertorResponse getKML(string fileType, string filePath, string fileName, string fileInternalName, string source_srid)
        {
            //InputFolder = DirPath + BaseFolder + Request.Form["filetype"];
            InputFolder = filePath;
            string file_extention = string.Empty;
            KmlFileName = fileInternalName;

            if (string.IsNullOrEmpty(source_srid))
                SourceSrid = "";
            else
                SourceSrid = " -s_srs EPSG:" + source_srid;


            //Folder check
            if (!Directory.Exists(InputFolder))
            {
                response.Status = false;
                response.Message = InputFolder + ", Please create required folders.";
                return response;
            }

            //Check all required filed uploaded or Not
            List<string> missingfiles = new List<string>();
            missingfiles = ValidateExtention(fileType, filePath, fileName, fileInternalName, out file_extention);
            if (missingfiles.Count > 0)
            {
                response.Status = false;
                response.Message = "Please provide file with following extentions : " + string.Join(",", missingfiles);
                return response;
            }

            //Save valid files to respective folder
            //bool savefile = SaveFiletoLocal(Request);
            //if (savefile)
            //{
            if (ConvertFileToKML(file_extention))
            {
                response.Status = true;
                response.Message = "File conversion successful.";
                response.OutputType = "KML";
                response.Output = File.ReadAllText(response.OutputFile);
            }
            //}
            return response;
        }

        public ConvertorResponse getKMLfromTabFile(string fname, string FileName, string file_extention)
        {
            if (ConvertTabFileToKML(fname, FileName, file_extention))
            {
                response.Status = true;
                response.Message = "File conversion successful.";
                response.OutputType = "KML";
                response.Output = File.ReadAllText(response.OutputFile);
            }
            //}
            return response;
        }

        /// <summary>
        /// Support file type for GeoJson conversion Shape, Tab, Kmz (Parameters value inside the request object)
        /// </summary>
        /// <param name="filetype"> i.e. Shape, Tab, Kmz</param>
        ///  <param name="txtfilename"> i.e. abc, xyz</param>
        /// <returns></returns>
        public ConvertorResponse getGeoJson(HttpRequestBase Request)
        {
            ConvertorResponse resp = getKML(Request);
            if (KmlToGeoJsonConvertor())
            {
                response.Status = true;
                response.Message = "File conversion successful.";
                response.OutputType = "Json";
                response.Output = File.ReadAllText(response.OutputFile);
            }
            return response;
        }

        private bool ShapeTabConvertor(string file_extention)
        {
            try
            {
                string shp2kml_convertorfolder = "ogr2ogr_convertor";
                // var arguments = "-f KML " + KmlFolder + KmlFileName + ".kml " + InputFolder + "\\" + KmlFileName + file_extention + " -t_srs EPSG:4326 "+ SourceSrid;

                var arguments = "-f KML " + "\"" + KmlFolder + KmlFileName + ".kml" + "\"" + " \"" + InputFolder + "\\" + KmlFileName + file_extention + "\"" + " -t_srs EPSG:4326 " + SourceSrid;
                var workingdir = DirPath + ProjectName + shp2kml_convertorfolder;
                var exe_filename = DirPath + ProjectName + shp2kml_convertorfolder + "\\ogr2ogr.exe";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = workingdir,
                        FileName = exe_filename,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };
                proc.Start();
                proc.WaitForExit();

                DeleteInputFiles();
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "File conversion problem.";
                return false;
            }
            response.OutputFile = KmlFolder + KmlFileName + ".kml";
            return true;
        }


        private bool TabConvertor(string file_extention, string InputFolder, string KmlFileName)
        {
            try
            {
                string shp2kml_convertorfolder = "ogr2ogr_convertor";
                var arguments = "-f KML " + "\"" + KmlFolder + KmlFileName + ".kml" + "\"" + " \"" + InputFolder + "\\" + KmlFileName + file_extention + "\"" + " -t_srs EPSG:4326 " + SourceSrid;
                var workingdir = DirPath + ProjectName + shp2kml_convertorfolder;
                var exe_filename = DirPath + ProjectName + shp2kml_convertorfolder + "\\ogr2ogr.exe";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = workingdir,
                        FileName = exe_filename,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "File conversion problem.";
                return false;
            }
            response.OutputFile = KmlFolder + KmlFileName + ".kml";
            return true;
        }

        public ConvertorResponse KmltoDXFConverter(string filePath, string FileName)
        {
            try
            {
                //DirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\"));
                string shp2kml_convertorfolder = "ogr2ogr_convertor";
                var arguments = "-f DXF " + filePath + FileName + ".dxf " + filePath + FileName + ".kml";
                var workingdir = DirPath + ProjectName + shp2kml_convertorfolder;
                var exe_filename = DirPath + ProjectName + shp2kml_convertorfolder + "\\ogr2ogr.exe";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = workingdir,
                        FileName = exe_filename,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };
                proc.Start();
                proc.WaitForExit();

                //DeleteInputFiles();
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "File conversion problem.";
                return response;
            }
            response.OutputFile = KmlFolder + FileName + ".dxf";
            response.Status = true;
            response.Message = "File conversion successful.";
            response.OutputType = "KML";
            response.Output = File.ReadAllText(response.OutputFile);
            return response;
        }
        public bool KMZtoKMLConvertor(string file_extention)
        {
            try
            {
                string fname = string.Empty;
                string kmzpath = Path.Combine(InputFolder, KmlFileName + file_extention);
                using (StreamReader sr = new StreamReader(kmzpath))
                {
                    using (var kmzFile = KmzFile.Open(sr.BaseStream))
                    {
                        var kml = kmzFile.ReadKml();
                        fname = Path.Combine(KmlFolder, KmlFileName + ".kml");
                        System.IO.File.WriteAllText(fname, kml);
                    }
                }
                DeleteInputFiles();
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "File conversion problem.";
                return false;
            }

            response.OutputFile = KmlFolder + KmlFileName + ".kml";
            return true;
        }
        private bool KmlToGeoJsonConvertor()
        {
            try
            {
                string shp2kml_convertorfolder = "ogr2ogr_convertor";
                var arguments = "-f GeoJSON " + GeoJsonFolder + KmlFileName + ".json " + response.OutputFile + " -t_srs EPSG:4326";
                var workingdir = DirPath + ProjectName + shp2kml_convertorfolder;
                var exe_filename = DirPath + ProjectName + shp2kml_convertorfolder + "\\ogr2ogr.exe";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = workingdir,
                        FileName = exe_filename,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };
                proc.Start();
                proc.WaitForExit();

                File.Delete(response.OutputFile);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "File conversion problem.";
                return false;
            }

            response.OutputFile = GeoJsonFolder + KmlFileName + ".json";
            return true;
        }

        private List<string> validatefile(string filetype, out string file_extention)
        {
            List<string> vfile = new List<string>();
            file_extention = "";
            switch (filetype.ToLower())
            {
                case "tab":
                    file_extention = ".tab";
                    vfile.AddRange(new[] { ".DAT", ".ID", ".MAP", ".TAB" });
                    break;
                case "shape":
                    file_extention = ".shp";
                    vfile.AddRange(new[] { ".shp", ".dbf", ".prj", ".shx" });
                    break;
                case "kmz":
                    file_extention = ".kmz";
                    vfile.AddRange(new[] { ".kmz" });
                    break;
                case "dxf":
                    file_extention = ".dxf";
                    vfile.AddRange(new[] { ".dxf" });
                    break;
            }
            return vfile;
        }

        private List<string> ValidateExtention(HttpRequestBase Request, out string file_extention)
        {
            file_extention = "";
            List<string> validfiles = validatefile(Request.Form["filetype"], out file_extention);
            List<string> missingfiles = new List<string>();
            List<string> filesext = new List<string>();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var filename = Request.Files[i].FileName;
                if (validfiles.Contains(Path.GetExtension(filename)))
                {
                    filesext.Add(Path.GetExtension(filename));
                }
            }

            missingfiles = validfiles.Except(filesext).ToList();
            return missingfiles;
        }

        private static bool CheckIfFileExists(string fileName, string directory)
        {
            var exists = false;
            var fileNameToCheck = Path.Combine(directory, fileName);
            if (Directory.Exists(directory))
            {
                //check directory for file
                exists = Directory.GetFiles(directory).Any(x => x.Equals(fileNameToCheck, StringComparison.OrdinalIgnoreCase));

            }
            return exists;
        }
        private List<string> ValidateExtention(string fileType, string filePath, string fileName, string fileInternalName, out string file_extention)
        {
            file_extention = "";
            List<string> validfiles = validatefile(fileType, out file_extention);
            List<string> missingfiles = new List<string>();

            foreach (string validExtension in validfiles)
            {

                bool isExists = CheckIfFileExists(fileInternalName + validExtension, filePath);
                if (!isExists)
                {
                    missingfiles.Add(fileName + validExtension);
                }

            }

            return missingfiles;
        }

        private bool SaveFiletoLocal(HttpRequestBase Request)
        {
            try
            {
                if (Request != null)
                {
                    KmlFileName = Request.Form["txtfilename"] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var objfile = Request.Files[i];
                        var filepath = Path.Combine(InputFolder, KmlFileName + Path.GetExtension(objfile.FileName));
                        objfile.SaveAs(filepath);
                    }
                }
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Input files not saved, for further processing.";
                return false;
            }

            return true;
        }

        private bool ConvertFileToKML(string inputExtention)
        {
            try
            {
                //Get KML string according to file type
                switch (inputExtention.ToLower())
                {
                    case ".shp":
                    case ".tab":
                    case ".dxf":
                        ShapeTabConvertor(inputExtention);
                        break;
                    case ".kmz":
                        KMZtoKMLConvertor(inputExtention);
                        break;

                }

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool ConvertTabFileToKML(string fname, string FileName, string file_extention)
        {
            try
            {
                InputFolder = fname;
                KmlFileName = FileName;
                TabConvertor(file_extention, InputFolder, KmlFileName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool DeleteInputFiles()
        {
            try
            {
                DirectoryInfo folder = new DirectoryInfo(InputFolder);
                if (folder.Exists) // else: Invalid folder!
                {
                    FileInfo[] files = folder.GetFiles(KmlFileName + ".*");

                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        } 
        
        public DataTable readkml(string file)
        {
            var table = new DataTable("kmldt");
            XDocument doc = XDocument.Load(file);
            string ext = Path.GetExtension(file);
            string GeometryColName = (ext == ".kml" ? "sp_geometry" : "Geometry");
            //string GeometryColName = "sp_geometry";
            // List<XElement> placemarks = doc.Descendants().Where(x => x.Name.LocalName == "SimpleData").ToList();
            List<XElement> placemarks = doc.Descendants().Where(x => x.Name.LocalName == "Placemark").ToList();

            //Set table Header name according 
            var firstPlacemark = placemarks.FirstOrDefault();
            table.Columns.Add("Name");
            //  var s = firstPlacemark.Descendants().Where(y => y.Name.LocalName == "Name");
            //by ExtendedData
            List<XElement> s = firstPlacemark.Descendants().Where(y => y.Name.LocalName == "SimpleData").ToList();
            foreach (XElement z in s)
            {
                table.Columns.Add(z.Attribute("name").Value);
            }


            var xcdata = firstPlacemark.Descendants().Where(y => y.Name.LocalName == "description").DescendantNodes().OfType<XCData>().FirstOrDefault();
            if (xcdata != null)
            {
                HtmlDocument htmldoc = new HtmlDocument();
                htmldoc.LoadHtml(xcdata.Value);
                HtmlNode[] nodes = htmldoc.DocumentNode.SelectNodes("//tr").ToArray();
                foreach (HtmlNode keytd in nodes)
                {
                    if (!table.Columns.Contains(keytd.FirstChild.InnerText))
                        table.Columns.Add(keytd.FirstChild.InnerText);
                }
            }


            table.Columns.Add(GeometryColName);

            HtmlNode[] trs = null;

            foreach (XElement x in placemarks)
            {
                var dataRow = table.NewRow();
                //Insert value in Name field
                if (x.Descendants().ElementAt(0) != null)
                {
                    dataRow["Name"] = x.Descendants().ElementAt(0).Value;
                }

                //dataRow["Name"] = x.Descendants().ElementAt(0)?.Value;

                //Insert values in SimpleData data fields
                List<XElement> desc = x.Descendants().Where(y => y.Name.LocalName == "SimpleData").ToList();
                foreach (XElement z in desc)
                {
                    if (table.Columns.Contains(z.Attribute("name").Value))
                        dataRow[z.Attribute("name").Value] = z.Value;
                    else
                    {
                        table.Columns.Add(z.Attribute("name").Value);
                        dataRow[z.Attribute("name").Value] = z.Value;
                    }
                }

                //Insert values in Description data fields
                var readDesc = x.Descendants().Where(y => y.Name.LocalName == "description").DescendantNodes().OfType<XCData>().FirstOrDefault();
                if (readDesc != null)
                {
                    HtmlDocument htmldoc = new HtmlDocument();
                    htmldoc.LoadHtml(readDesc.Value);
                    trs = htmldoc.DocumentNode.SelectNodes("//tr").ToArray();
                    foreach (HtmlNode keytd in trs)
                    {
                        dataRow[keytd.FirstChild.InnerText] = keytd.LastChild.InnerText;
                    }
                }

                //Get Value of Geometry according to Geometry Type
                //Polygon
                List<XElement> poly = x.Descendants().Where(y => y.Name.LocalName == "Polygon").ToList();
                foreach (XElement z in poly)
                {
                    dataRow[GeometryColName] = getGeomObject(z.Value, "polygon");
                }
                //Line
                List<XElement> line = x.Descendants().Where(y => y.Name.LocalName == "LineString").ToList();
                foreach (XElement z in line)
                {
                    dataRow[GeometryColName] = getGeomObject(z.Value, "linestring");
                }
                //Point
                List<XElement> point = x.Descendants().Where(y => y.Name.LocalName == "Point").ToList();
                foreach (XElement z in point)
                {
                    dataRow[GeometryColName] = getGeomObject(z.Value, "point");
                }

                table.Rows.Add(dataRow);
            }
            return table;

        }
        private static string getGeomObject(string rawGeom, string geomType)
        {
            var coordinates = string.Empty;
            var outGeometry = string.Empty;
            coordinates = rawGeom;
            coordinates = coordinates.Replace("  ", "");
            if (coordinates.Contains("11\n"))
            {
                coordinates = coordinates.Replace("11\n", "");
                coordinates = coordinates.Replace("\n", "");
                coordinates = coordinates.Replace("\t", "");
                coordinates = coordinates.Replace("\r\n", "##");
                coordinates = coordinates.Replace("\\", "");
                coordinates = coordinates.Replace("\r", "");
            }

            string[] Result = null;
            string Delimiter = string.Empty;
            List<string> lstOutLatLng = new List<string>();
            int ResultLength = 0;

            if (coordinates.Contains(",0"))
            {
                Delimiter = ",0";
                ResultLength = 1;
                Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
            }
            else
            {
                Delimiter = " ";
                Result = coordinates.Split(new[] { Delimiter }, StringSplitOptions.None);
            }

            if (Result.Length > ResultLength && Result[ResultLength].ToString() != "")
            {
                foreach (string latLng in Result)
                {
                    if (!string.IsNullOrEmpty(latLng))
                    {
                        lstOutLatLng.Add(latLng.Replace(",", " "));
                    }
                }
            } 
            outGeometry = string.Join(",", lstOutLatLng);
            switch (geomType)
            {
                case "polygon":
                    outGeometry = "POLYGON((" + outGeometry.Trim().TrimEnd(',') + "))";
                    break;
                case "linestring":
                    outGeometry = "LINESTRING(" + outGeometry.Trim().TrimEnd(',') + ")";
                    break;
                case "point":
                    outGeometry = "POINT(" + outGeometry.Trim().TrimEnd(',') + ")";
                    break;
            } 
            return outGeometry;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbHost">localhost or server ip</param>
        /// <param name="dbport">5432</param>
        /// <param name="dbname">mydb</param>
        /// <param name="dbuserid">postgres</param>
        /// <param name="dbpassword">abcd</param>
        /// <param name="sqlquery">sql query must have geometry field</param>
        /// <returns></returns>
        public ConvertorResponse SQL2ShapeConvertor(string dbHost, string dbport, string dbname, string dbuserid, string dbpassword, string sqlquery)
        {
            try
            {
                KmlFileName = "file_" + DateTime.Now.ToFileTime();

                InputFolder = DirPath + BaseFolder + "shape";
                string file_extention = string.Empty;

                //Folder check
                if (!Directory.Exists(InputFolder))
                {
                    response.Status = false;
                    response.Message = InputFolder + ", Please create required folders.";
                    return response;
                }

                string shp2kml_convertorfolder = "ogr2ogr_convertor";
                var arguments = "-h " + dbHost + " -p " + dbport + " -u " + dbuserid + " -P " + dbpassword + " -f " + InputFolder + "\\" + KmlFileName + " " + dbname + " \"" + sqlquery + "\"";

                var workingdir = DirPath + ProjectName + shp2kml_convertorfolder;
                var exe_filename = DirPath + ProjectName + shp2kml_convertorfolder + "\\pgsql2shp.exe";

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = workingdir,
                        FileName = exe_filename,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };
                proc.Start();
                proc.WaitForExit();

                ShapeTabConvertor(".shp");

            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "File conversion problem.";
                return response;
            }
            response.Status = true;
            response.Message = "File conversion successful.";
            response.OutputType = "KML";
            response.OutputFile = KmlFolder + KmlFileName + ".kml";
            return response;
        }    
    }
}
