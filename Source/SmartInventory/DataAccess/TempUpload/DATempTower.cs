using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System.Collections.Generic;
using System.Linq;


namespace DataAccess
{
	public class DATempTower : Repository<TempTower>
	{
		PostgreSQL postgreSql;
		public DATempTower()
		{
			postgreSql = new PostgreSQL();
			postgreSql.PostgresNotificationEvent += NotifyUpdatedStatus;
		}
		public void Save(List<TempTower> lst)
		{
			repo.Insert(lst);
		}

		public int InsertIntoMainTable(UploadSummary summary, int batchId)
		{
			var lstItems = repo.ExecuteProcedure<int>("fn_uploader_insert_tower", new { p_uploadid = summary.id, p_batchid = batchId }, false);
			return lstItems != null && lstItems.Count > 0 ? lstItems[0] : 0;
		}

		public List<TempTower> GetAll(int uploadId)
		{
			return repo.GetAll(m => m.upload_id == uploadId && m.is_valid == true).ToList();
		}
	}
}
