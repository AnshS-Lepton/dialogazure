using BusinessLogics;
using Models;
using Models.Dashboard;
using Newtonsoft.Json;
using NPOI.OpenXml4Net.OPC.Internal;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SmartInventory.Controllers
{
	public class KPIController : Controller
	{
		// GET: Admin/KPI
		public ActionResult Index(int template_id)
		{
			// ExportPDF();
			KPIMasterResult kpiMaster = new KPIMasterResult();
			// ExportPDF();
			var kpi_master = new BLKPIMaster().GetKPIMasterJson();
			kpiMaster.KPI_Master_Json = kpi_master;
			if (template_id > 0) //fn_get_kpi_template_json
			{
				var kpitemplatedetails = new BLKPIMaster().GetKPITempateDetails(template_id);
				kpiMaster.kpi_Template = kpitemplatedetails;
			}
			return View(kpiMaster);
		}

		public ActionResult KPITemplateName()
		{
			// ExportPDF();
			var ddltamplate = new BLKPIMaster().GetKPITemplateList();
			ViewBag.tamplatebind = ddltamplate;
			return View("_ReportBinding", ViewBag.tamplatebind);
		}

		public ActionResult KPIList(int kpi_id)
		{
			if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				if (kpi_id > 0)
				{
					var kpi_data = new BLKPIMaster().GetKPIMasterData(kpi_id);
					DataTable dt = (DataTable)JsonConvert.DeserializeObject(kpi_data, (typeof(DataTable)));
					string[] dtcolumnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

					//if (Request.Query.Select(i => i.Key.Split("-")[0]).Intersect(dtcolumnNames).Any())
					//{
					//	dt = filterdatabyQueryString(Request.Query, dt);
					//}

					ViewBag.kpidata = JsonConvert.SerializeObject(dt); ;
					ViewBag.TotalRecords = dt.Rows.Count;
					Session["kpidata"] = dt;
					//_session.SetObject("kpidata", dt);


					return View("_KPIList", dt);
				}
			}
			return View("_KPIList", new DataTable());
		}
		//private DataTable filterdatabyQueryString(IQueryCollection Query, DataTable dt)
		//{
		//	DataTable retvalue = new DataTable();
		//	if (dt != null)
		//	{
		//		DataView dv = new DataView(dt);
		//		string x = string.Empty;
		//		foreach (var item in Request.Query)
		//		{
		//			if (item.Key != "rows" && item.Key != "kpi_id" && item.Key != "page")
		//			{
		//				x = $"[{item.Key.Split("-")[0]}] {getOperator(item.Key.Split("-")[1])} '{item.Value}'";
		//				dv.RowFilter = x;
		//			}
		//		}
		//		DataTable d = dv.ToTable();

		//		retvalue = d;
		//	}
		//	return retvalue;
		//}

		public ActionResult KPIExcelDownload()
		{
			//DataTable dt = _session.GetObject<DataTable>("kpidata");
			
				DataTable dt = (DataTable)Session["kpidata"];
				var excelfileName =  "\\tempfiles\\KPIData.xlsx";
				var e = Helper.FileHelper.WriteExcel(dt, excelfileName, "");
				byte[] fileBytes = System.IO.File.ReadAllBytes(excelfileName);
				System.IO.File.Delete(excelfileName);
				return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "KPIData.xlsx");
		}


		private string getOperator(string op)
		{
			switch (op)
			{
				case "equals":
					return "=";
				case "not":
					return "<>";
			}
			return "not-equals";
		}
		public JsonResult KPIChart(int kpi_id)
		{
			var kpi_data = new BLKPIMaster().GetKPIMasterData(kpi_id);

			return Json(Convert.ToString(kpi_data));
		}


		[System.Web.Http.HttpPost]
		public ActionResult ExportPdf(KPI_VM kpi)
		{
			DataTable dt = (DataTable)Session["kpidata"];
			//DataTable dt = _session.GetObject<DataTable>("kpidata");

			StringBuilder sb = new StringBuilder();
			sb.Append(@"<!DOCTYPE html><html class='no-js' lang='en-US'><head><meta charset='UTF-8' /><meta name='viewport' content='width=device-width, initial-scale=1.0' /><title></title></head>");
			sb.Append("<body style='margin: 0; padding: 0; font-family: Arial, Helvetica, sans-serif; box-sizing:border-box'>");
			sb.Append("<div style='width: 100%;margin: 0 auto;border: 1px solid #cccccc;padding: 10px 7px; box-sizing:border-box'>");
			sb.Append("<table border-collapse='collapse' style='width: 98%'><tbody>");
			sb.Append($"<tr><td style='text-align: right'><label><span>Date</span><span style = 'display: inline-block;margin-left: 0.4rem;font-weight: 600;'> {DateTime.Now.ToLongDateString()} </span></label></td></tr>");
			sb.Append($"<tr><td><label style='display: block; margin: 1rem 0 0.5rem'><span>KPI Category : </span><span style='display: inline-block;margin-left: 0.4rem;font-weight: 600;'>{kpi.kpi_category}</span></label><label style='display: block; margin: 1rem 0 0.5rem'><span>KPI Name : </span><span style='display: inline-block;margin-left: 0.4rem;font-weight: 600;'>{kpi.kpi_name}</span></label></td></tr>");
			sb.Append("<tr><td><div style='border-bottom: 1px solid #cccccc; margin: 0.5rem 0 0'></div></td></tr>");
			sb.Append("<tr><td><div style='margin: 1rem 0'><label style='font-weight: 600;display: inline-block;margin-bottom: 0.5rem;'>Chart</label>");

			//Chart Image
			sb.Append($"<div style='padding: 0.5rem; box-sizing:border-box';><img src='data:image/png;base64,{kpi.chart}' style='width:95%;display:block;'  /></div></div></td></tr>");
			sb.Append("<tr><td>");

			//Content Table start
			sb.Append("<br/><br/><br/><br/><br/><br/><table style='width: 100%;border: 1px solid #cccccc;border-collapse: collapse;'>");

			//Table Heading
			sb.Append("<thead>");
			sb.Append("<tr style='background-color: #e7e7e7'>");
			foreach (var item in dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName))
			{
				sb.Append($"<td style='border: 1px solid #cccccc;padding: 0.5rem;font-weight: 600;'>{item.Replace("_", " ")} </td>");
			}
			sb.Append("</tr>");
			sb.Append("</thead>");

			//Table Body
			sb.Append("<tbody>");

			for (int i = 0; i <= dt.Rows.Count - 1; i++)
			{
				sb.Append("<tr>");
				for (int j = 0; j <= dt.Columns.Count - 1; j++)
				{
					var cell = dt.Rows[i][j];
					sb.Append($"<td style='border: 1px solid #cccccc; padding: 0.5rem'>{cell} </td>");
				}
				sb.Append("</tr>");
			}
			sb.Append("</tbody>");

			sb.Append("</table>");
			sb.Append("</td></tr></tbody></table></div></body></html>");


			// read parameters from the webpage
			string htmlString = sb.ToString();

			PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), PdfPageSize.A4.ToString(), true);
			PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), "Landscape", true);

			// instantiate a html to pdf converter object
			HtmlToPdf converter = new HtmlToPdf();

			// set converter options
			converter.Options.PdfPageSize = pageSize;
			converter.Options.PdfPageOrientation = pdfOrientation;
			converter.Options.WebPageWidth = 1024;
			converter.Options.WebPageHeight = 0;

			//converter.Options.MarginLeft = 10;
			// converter.Options.MarginRight = 10;
			//// converter.Options.MarginTop = 10;
			// converter.Options.MarginBottom = 10;

			// create a new pdf document converting an url
			PdfDocument doc = converter.ConvertHtmlString(htmlString);

			// save pdf document
			var filename = "temp.pdf";
			doc.Save(filename);
			doc.Close();
			byte[] fileBytes = System.IO.File.ReadAllBytes(filename);
			System.IO.File.Delete(filename);

			return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "KPI_Data.pdf");
		}

		[System.Web.Http.HttpPost]
		public ActionResult SaveKPItemplate( kpi_template objkpi_Template)
		{
			if (objkpi_Template != null)
			{
				PageMessage objMsg = new PageMessage();
				var isNew = objkpi_Template.template_id > 0 ? false : true;
				objkpi_Template.created_on = DateTime.Now.ToLocalTime();
				int IsTemplateNameExists = new BLKPIMaster().ChkTemplateNameExist(objkpi_Template.template_name, objkpi_Template.template_id);
				if (IsTemplateNameExists > 0)
				{
					objMsg.status = ResponseStatus.FAILED.ToString();
					objMsg.message = "This Template Name already exists";
					objkpi_Template.pageMsg = objMsg;
					return View("Index", objkpi_Template);
				}
				if (isNew)
				{
					objkpi_Template = new BLKPIMaster().SaveKPITemplateDetails(objkpi_Template);
					objMsg.status = ResponseStatus.OK.ToString();
					objMsg.isNewEntity = isNew;
					objMsg.message = "Template Saved Successfull!";

				}
				else
				{
					objkpi_Template = new BLKPIMaster().UpdateKPITemplateDetails(objkpi_Template);
					objMsg.status = ResponseStatus.OK.ToString();
					objMsg.message = "Template Updated successfully!";
				}
				objkpi_Template.pageMsg = objMsg;
				return View("Index", objkpi_Template);
			}
			return View("Index");
		}
		public ActionResult ListTemplate()
		{
			var lst = new BLKPIMaster().GetKPITemplateList();
			var status = lst.Count > 0 ? "OK" : "Fail";
			return Json(new { status = status, data = lst });
		}
		public ActionResult DeleteKPITemplate(int template_id)
		{

			var lst = new BLKPIMaster().DeleteKPITemplate(template_id);

			return View("Index", lst);//KPITemplateName
		}
		public JsonResult ValidateTemplate(string source)
		{
			int IsExists = new BLKPIMaster().TemplateNameExist(source);
			if (IsExists <= 0)
			{
				return Json(new { status = "OK" });
			}
			else
			{
				return Json(new { status = "ERROR", message = "This Template Name Already Exists!" });
			}
		}
		private static List<T> ConvertDataTable<T>(DataTable dt)
		{
			List<T> data = new List<T>();
			foreach (DataRow row in dt.Rows)
			{
				T item = GetItem<T>(row);
				data.Add(item);
			}
			return data;
		}
		private static T GetItem<T>(DataRow dr)
		{
			Type temp = typeof(T);
			T obj = Activator.CreateInstance<T>();

			foreach (DataColumn column in dr.Table.Columns)
			{
				foreach (PropertyInfo pro in temp.GetProperties())
				{
					if (pro.Name == column.ColumnName)
						pro.SetValue(obj, dr[column.ColumnName], null);
					else
						continue;
				}
			}
			return obj;
		}

		public ActionResult ExportKPIExcel(string kpi_source, string kpi_name)
		{
			DataTable dt = new DataTable();
			dt = new BLKPIMaster().GetKPIExportData(kpi_source);

			if (dt.Rows.Count == 0)
			{
				dt.Clear();
				dt.Columns.Add("Message");
				DataRow row = dt.NewRow();
				row["Message"] = "No data found!";
				dt.Rows.Add(row);

			}
			var excelfileName =  "\\tempfiles\\KPIData.xlsx";
			var e = Helper.FileHelper.WriteExcel(dt, excelfileName, "");
			byte[] fileBytes = System.IO.File.ReadAllBytes(excelfileName);
			System.IO.File.Delete(excelfileName);

			return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", $"{kpi_name}.xlsx");
		}
	}
}
