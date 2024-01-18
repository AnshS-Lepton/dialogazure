using DataAccess.DBHelpers;
using Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
	public class DAKPIMaster : Repository<Kpi_Master>
	{
		
		//SaveKPIDetails
		public Kpi_Master SaveKPIDetails(Kpi_Master objCreateKPI)
		{
			try
			{
				return repo.Insert(objCreateKPI);
			}
			catch { throw; }

		}
		public Kpi_Master UpdateKPIDetails(Kpi_Master objCreateKPI)
		{
			try
			{
				return repo.Update(objCreateKPI);
			}
			catch { throw; }

		}


		//GetKPIDetailsByID

		public Kpi_Master GetKPIDetailsByID(int kpi_id)
		{
			return repo.Get(m => m.kpi_id == kpi_id);
		}

		public List<Kpi_Master_View> DeleteKPICategorylist(int kpi_id)
		{
			try
			{
				int result = 0;
				var kpiCategory = repo.Get(u => u.kpi_id == kpi_id);
				if (kpiCategory != null)
				{
					result = repo.Delete(kpiCategory.kpi_id);

				}

				return new DAKPIMasterView().GetKPICategorylist();

			}
			catch (Exception ex)
			{
				throw;
			}
		}


		public int ChkKPIMasterNameExist(string name, int id)
		{
			try
			{
				var res = repo.Get(u => u.kpi_name.Trim().ToLower() == name.Trim().ToLower() && u.kpi_id != id);

				return res != null ? 1 : 0;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public int ChkKPINameExist(string name)
		{
			try
			{
				var res = repo.Get(u => u.kpi_name.Trim().ToLower() == name.Trim().ToLower());

				return res != null ? 1 : 0;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<KPI_Master_Json> GetKPIMasterJson()
		{
			try
			{
				var result = repo.ExecuteProcedure<KPI_Master_Json>("fn_get_kpi_master_json", new { }, true);
				return result;
			}
			catch
			{
				throw;
			}
		}

		public dynamic GetKPIMasterData(int kpi_id)
		{
			try
			{
				var result = repo.ExecuteProcedure<dynamic>("fn_get_kpi_data_json", new { v_kpi_id = kpi_id }, true);
				return result;
			}
			catch
			{
				throw;
			}
		}

		public dynamic GetKPIExportData(string kpi_source)
		{
			try
			{
				var result = repo.ExecuteProcedure<dynamic>("fn_get_kpi_export_json",
					new { v_kpi_source = kpi_source }, true);
				return result;
			}
			catch
			{
				throw;
			}
		}

		//public bool InsertFeeddeclineDescriptionTomTom(List<string> Ids, string customMessage)
		//{
		//	string insertDumpValues = string.Empty;
		//	for (int ik = 0; ik < Ids.Count; ik++)
		//	{
		//		insertDumpValues = insertDumpValues.Equals(string.Empty) ? "((SELECT id FROM `tomtomservice` WHERE tomtomid = '" + Ids[ik] + "'),'" + customMessage + "',DATE_ADD(NOW(), INTERVAL 330 MINUTE))\n"
		//										 : insertDumpValues + ",\n" + "((SELECT id FROM `tomtomservice` WHERE tomtomid = '" + Ids[ik] + "'),'" + customMessage + "',DATE_ADD(NOW(), INTERVAL 330 MINUTE))";
		//	}

		//	string srtqry = "INSERT  INTO`feeddeclinedescription`(`FeedID`,`FeedDescription`,`CreateDate`) values " + insertDumpValues;
		//	var p = repoAdo.FromSqlToExecuteNonQuery(srtqry);
		//	if (p > 0)
		//	{
		//		return true;
		//	}
		//	return false;
		//}


	}
	public class DAKPIMasterCategory : Repository<Kpi_Category>
	{
		private static DAKPIMasterCategory daKPIMastercCategory = null;
		private static readonly object lockObject = new object();
		public static DAKPIMasterCategory Instance
		{
			get
			{
				lock (lockObject)
				{
					if (daKPIMastercCategory == null)
					{
						daKPIMastercCategory = new DAKPIMasterCategory();
					}
				}
				return daKPIMastercCategory;
			}
		}
		public DAKPIMasterCategory()
		{

		}

		public List<KeyValueDropDown> GetCategoryName()
		{
			var res = repo.GetAll().Select(x => new KeyValueDropDown
			{
				key = x.category_id,
				value = x.category_name
			})
				.Distinct().ToList();
			return res;


		}
		public int ChkCategoryNameExist(string name)
		{
			try
			{
				var res = repo.Get(u => u.category_name.Trim().ToLower() == name.Trim().ToLower());

				return res != null ? 1 : 0;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public Kpi_Category SaveKPICategory(Kpi_Category objCreateCategory)
		{
			try
			{
				return repo.Insert(objCreateCategory);
			}
			catch { throw; }

		}
	}
	public class DAKPIMasterView : Repository<Kpi_Master_View>
	{
		private static DAKPIMasterView daKPIMasterView = null;
		private static readonly object lockObject = new object();
		public static DAKPIMasterView Instance
		{
			get
			{
				lock (lockObject)
				{
					if (daKPIMasterView == null)
					{
						daKPIMasterView = new DAKPIMasterView();
					}
				}
				return daKPIMasterView;
			}
		}
		public DAKPIMasterView()
		{

		}

		public List<Kpi_Master_View> GetKPICategorylist()
		{
			try
			{
				return (List<Kpi_Master_View>)repo.GetAll();
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public List<Kpi_Master_View> DeleteKPICategorylist(int kpi_id)
		{
			try
			{
				int result = 0;
				var kpiCategory = repo.Get(u => u.kpi_id == kpi_id);
				if (kpiCategory != null)
				{
					result = repo.Delete(kpiCategory.kpi_id);

				}

				return new DAKPIMasterView().GetKPICategorylist();
			}
			catch (Exception ex)
			{
				throw;
			}
		}


	}
	public class DAKPITemplate : Repository<kpi_template>
	{
		private static DAKPITemplate daKPITemplate = null;
		private static readonly object lockObject = new object();
		public static DAKPITemplate Instance
		{
			get
			{
				lock (lockObject)
				{
					if (daKPITemplate == null)
					{
						daKPITemplate = new DAKPITemplate();
					}
				}
				return daKPITemplate;
			}
		}
		public DAKPITemplate()
		{

		}
		public kpi_template SaveKPITemplate(kpi_template objKPITemplate)
		{
			try
			{
				return repo.Insert(objKPITemplate);
			}
			catch { throw; }

		}
		public kpi_template UpdateKPITemplate(kpi_template objKPITemplate)
		{
			try
			{
				return repo.Update(objKPITemplate);
			}
			catch { throw; }

		}
		public List<kpi_template> GetKPITemplate()
		{
			try
			{
				return (List<kpi_template>)repo.GetAll();
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public kpi_template GetKPITemplateDetails(int template_id)
		{
			try
			{
				return repo.Get(a => a.template_id == template_id);
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public List<kpi_template> DeleteKPITemplate(int template_id)
		{
			try
			{
				int result = 0;
				var kpiTemplate = repo.Get(u => u.template_id == template_id);
				// var cableDetails = repo.Get(u => u.system_id == cable_Id && u.network_status == "P");
				if (kpiTemplate != null)
				{
					result = repo.Delete(kpiTemplate.template_id);

				}

				return GetKPITemplate();
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public kpi_template GetKPITemplateByID(int template_id)
		{
			return repo.Get(m => m.template_id == template_id);
		}
		public int ChkKPITemplateNameExist(string name, int id)
		{
			try
			{
				var res = repo.Get(u => u.template_name.Trim().ToLower() == name.Trim().ToLower() && u.template_id != id);

				return res != null ? 1 : 0;

			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
		public int ChkKPITemplateName(string name)
		{
			try
			{
				var res = repo.Get(u => u.template_name.Trim().ToLower() == name.Trim().ToLower());

				return res != null ? 1 : 0;

			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
	}
}


