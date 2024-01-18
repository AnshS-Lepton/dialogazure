using DataAccess;
using DataAccess.DBHelpers;
using Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
	public class BLKPIMaster
	{
		public List<KeyValueDropDown> GetCategoryName()
		{
			return new DAKPIMasterCategory().GetCategoryName();
		}

		public List<KPI_Master_Json> GetKPIMasterJson()
		{
			return new DAKPIMaster().GetKPIMasterJson();
		}

		public dynamic GetKPIMasterData(int kpi_id)
		{
			return new DAKPIMaster().GetKPIMasterData(kpi_id);
		}

		public Kpi_Master SaveKPIDetails(Kpi_Master objCreateKPI)
		{
			return new DAKPIMaster().SaveKPIDetails(objCreateKPI);
		}
		public Kpi_Master UpdateKPIDetails(Kpi_Master objCreateKPI)
		{
			return new DAKPIMaster().UpdateKPIDetails(objCreateKPI);
		}
		public int ChkCategoryNameExist(string name)
		{
			return new DAKPIMasterCategory().ChkCategoryNameExist(name);
		}
		//SaveKPICategory
		public Kpi_Category SaveKPICategory(Kpi_Category objCreateCategory)
		{
			return new DAKPIMasterCategory().SaveKPICategory(objCreateCategory);
		}
		public int ChkKPIMasterNameExist(string name, int id)
		{
			return new DAKPIMaster().ChkKPIMasterNameExist(name, id);
		}
		public int ChkKPINameExist(string name)
		{
			return new DAKPIMaster().ChkKPINameExist(name);
		}

		public List<Kpi_Master_View> GetKPICategorylist()
		{
			return new DAKPIMasterView().GetKPICategorylist();
		}
		//public List<Kpi_Master> DeleteKPICategorylist(int kpi_id)
		//{
		//    return new DAKPIMaster().DeleteKPICategorylist(kpi_id);
		//}
		public List<Kpi_Master_View> DeleteKPICategorylist(int kpi_id)
		{
			return new DAKPIMasterView().DeleteKPICategorylist(kpi_id);
		}
		public Kpi_Master GetKPIDetailsByID(int kpi_id)
		{
			return new DAKPIMaster().GetKPIDetailsByID(kpi_id);
		}

		// public kpi_template Get
		public int ChkTemplateNameExist(string name, int id)
		{
			return new DAKPITemplate().ChkKPITemplateNameExist(name, id);
		}
		public int TemplateNameExist(string name)
		{
			return new DAKPITemplate().ChkKPITemplateName(name);
		}
		//UpdateKPITemplateDetails
		public kpi_template SaveKPITemplateDetails(kpi_template objKPITemplate)
		{
			return new DAKPITemplate().SaveKPITemplate(objKPITemplate);
		}
		public kpi_template UpdateKPITemplateDetails(kpi_template objKPITemplate)
		{
			return new DAKPITemplate().UpdateKPITemplate(objKPITemplate);
		}
		public List<kpi_template> GetKPITemplateList()
		{
			return new DAKPITemplate().GetKPITemplate();
		}

		
		public kpi_template GetKPITempateDetails(int template_id)
		{
			return new DAKPITemplate().GetKPITemplateDetails(template_id);
		}
		public List<kpi_template> DeleteKPITemplate(int template_id)
		{
			return new DAKPITemplate().DeleteKPITemplate(template_id);
		}
		public kpi_template GetKPITemplateByID(int template_id)
		{
			return new DAKPITemplate().GetKPITemplateByID(template_id);
		}

		public dynamic GetKPIExportData(string kpi_source)
		{
			return new DAKPIMaster().GetKPIExportData(kpi_source);
		}
	}
}


