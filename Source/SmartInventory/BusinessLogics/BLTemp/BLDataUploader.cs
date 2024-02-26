using DataAccess;
using Microsoft.SqlServer.Server;
using Models;
using Models.Admin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace BusinessLogics
{
	public class BLDataUploader
	{
		public static int BulkUploadBatchCount = Convert.ToInt32(ConfigurationManager.AppSettings["BulkUploadBatchCount"]);
		public static int LineUploadBufferInMeter = Convert.ToInt32(ConfigurationManager.AppSettings["LineUploadBufferInMeter"]);

		public delegate void NotificationEventHandlerBusinessLogic(dynamic data);
		public event NotificationEventHandlerBusinessLogic DataUploaderNotifyEventHandler;
		private int CurrentStep = 0;

		private readonly PostgreSQL postgreSQL;
		public BLDataUploader()
		{
			postgreSQL = new PostgreSQL();
			postgreSQL.PostgresNotificationEvent += NotifyUpdatedStatus;
		}

		public void NotifyUpdatedStatus(dynamic dynamic)
		{
			if (DataUploaderNotifyEventHandler != null)
				DataUploaderNotifyEventHandler.Invoke(dynamic);
		}
		private void UpdateStatus(UploadSummary summary, string Message)
		{
			summary.status_message = Message;
			UpdateStatus(summary);
		}
		public UploadSummary Validate(UploadSummary summary, string tempTableName, string geomtype)
		{
			CurrentStep = 0;
			ValidationStatus(summary, "Validation Started");
             
                ValidateParentNetworkId(summary);
            
			if (geomtype == Models.GeometryType.Line.ToString())
			{
				CheckTerminationPoints(summary);
			}
            //ValidateGeometry(summary, tempTableName, geomtype);
            //ValidateRegionProvince(summary, tempTableName, geomtype);
            if (summary.entity_type.ToUpper()!="LANDBASE" && summary.entity_type.ToUpper() != "BUILDING" && summary.entity_type.ToUpper() != "UNIT" && summary.entity_type.ToUpper() != "ROW")
            {
                ValidateVendorSpecification(summary, tempTableName);
            } 
			//Check if Manual Network ID already exists in system or NOT. 
			ValidateDuplicateNetworkCode(summary, tempTableName);
            //Check for Manual Network ID if latitude and longitude falls in  Region Province with Abbrevaiton matches with Given Network ID
            //ValidateNetworkCode(summary, tempTableName, geomtype);//correct 

            //Check for Shaft and floor name and update their ID if entity's parent entity type is Structure or Unit.
           
            ValidateShaftFloorinfo(summary, tempTableName);

            ValidateEntityDimension(summary, tempTableName);

            ValidateUnitNetworkId(summary, tempTableName);

            if (summary.entity_type == "Loop")
            {
				Validateloop(summary, tempTableName);
            }
            return ValidationStatus(summary, "Validation Completed");

		}
		private void CheckTerminationPoints(UploadSummary summary)
		{
			string strQuery = string.Format("select * from fn_uploader_check_termination_points({0},'{1}')", summary.id, summary.entity_type);
			UpdateStatus(summary, "Checking Termination Points");
			postgreSQL.ExecuteQuery(strQuery);
		}
		private void ValidateParentNetworkId(UploadSummary summary)
		{

			CurrentStep++;
			UpdateStatus(summary, "Validating Parent Network ID");
            string strQuery = string.Format("select * from fn_uploader_validate_parent_details({0},'{1}')", summary.id, summary.entity_type);
           // string strQuery = string.Format("select * from fn_uploader_validate_parent_details_bkp_31jan2021({0},'{1}')", summary.id, summary.entity_type);
            postgreSQL.ExecuteQuery(strQuery);
		}

		private void ValidateVendorSpecification(UploadSummary summary, string tempTableName)
		{
			CurrentStep++;
			UpdateStatus(summary, "Validating Vendor Specification");
			string strQuery = string.Format("select * from fn_uploader_update_vendor_specification({0},'{1}')", summary.id, summary.entity_type);
            
             // string strQuery = string.Format("select * from fn_uploader_update_vendor_specification_backup_07062021({0},'{1}')", summary.id, summary.entity_type);

            postgreSQL.ExecuteQuery(strQuery);
		}

        private void ValidateShaftFloorinfo(UploadSummary summary, string tempTableName)
        {
            CurrentStep++;
            UpdateStatus(summary, "Validating Shaft and Floor name");
           // p_shaft_name = shaft_name, p_floor_name = floor_name, p_parent_network_id = parent_network_id
            string strQuery = string.Format("select * from fn_uploader_update_shaft_floor_name({0},'{1}')", summary.id, summary.entity_type);

            postgreSQL.ExecuteQuery(strQuery);
        }
        private void ValidateEntityDimension(UploadSummary summary, string tempTableName) 
        {
            CurrentStep++;
            UpdateStatus(summary, "Validating Entity Template"); 
            string strQuery = string.Format("select * from fn_uploader_validate_entity_dimension({0},'{1}')", summary.id, summary.entity_type);

            postgreSQL.ExecuteQuery(strQuery);
        }
        
        private void ValidateUnitNetworkId(UploadSummary summary, string tempTableName)
        {
            CurrentStep++;
            UpdateStatus(summary, "Validating Entity Template");
            string strQuery = string.Format("select * from fn_validate_unit_network_id({0},'{1}')", summary.id, summary.entity_type);

            postgreSQL.ExecuteQuery(strQuery);
        }
		private void Validateloop(UploadSummary summary, string tempTableName)
		{
			CurrentStep++;
			UpdateStatus(summary, "Validating Entity Template");
			string strQuery = string.Format("select * from fn_uploader_validate_loop({0},'{1}')", summary.id, summary.entity_type);

			postgreSQL.ExecuteQuery(strQuery);
		}
		private void ValidateDuplicateNetworkCode(UploadSummary summary, string tempTableName)
		{
			CurrentStep++;
			UpdateStatus(summary, "Checking Duplicate Network Code");
			string strQuery = string.Format("select * from fn_uploader_check_network_id_exist({0},'{1}')", summary.id, summary.entity_type);
			postgreSQL.ExecuteQuery(strQuery);
		}

		private UploadSummary ValidationStatus(UploadSummary summary, string message)
		{
			int InvalidRecords = GetInValidRecordCount(summary);
			summary.failed_record = InvalidRecords;
			summary.success_record = 0;
			UpdateStatus(summary, message);
			summary.success_record = summary.total_record - summary.failed_record;
			return summary;
		}
		public static string CalculateEstimatedRemainingTime(DateTime processStarted, int totalElements, int processedElements)
		{

			TimeSpan timeRemaining = TimeSpan.FromTicks(DateTimeHelper.Now.Subtract(processStarted).Ticks * (totalElements - (processedElements + 1)) / (processedElements + 1));
			string formattedTimeSpan = string.Format("{0:D2} hrs, {1:D2} mins, {2:D2} secs", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
			return formattedTimeSpan;
		}
		public static string EstimatedRemainingTimeMessage(int currentValue, string remainingTime)
		{

			return "Uploading..." + currentValue + "% Remaining Time: " + remainingTime;

		}


		public void ParallelInsert(int TotalBatchCount, List<dynamic> lstEntity, dynamic tempUploadObject, UploadSummary summary)
		{


			var list = lstEntity;
			List<List<dynamic>> lstChunks = lstEntity.ToChunks(TotalBatchCount).ToList();
			var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
			object lockObj = new object();
			int currentStatus = 0;
			int CurrentValue = 0;
			DateTime dtStarts = DateTimeHelper.Now;
			var exceptions = new ConcurrentQueue<Exception>();
			dynamic blTempPole = tempUploadObject;
			int batchCount = 0;
			string EstimatedRemainingTime;
			ParallelLoopResult result = Parallel.ForEach(lstChunks, options, (item) =>
			{
				try
				{
					lock (lockObj)
					{
						batchCount++;
						item.ForEach(m => m.batch_id = batchCount);
						blTempPole.Save(item);
						Interlocked.Increment(ref currentStatus);
						CurrentValue = (currentStatus * 100) / lstChunks.Count;
						EstimatedRemainingTime = CalculateEstimatedRemainingTime(dtStarts, lstChunks.Count, currentStatus);
						summary.status_message = "Uploading..." + CurrentValue + "% Remaining Time: " + EstimatedRemainingTime;
						if (CurrentValue == 100)
							summary.status_message = "Files uploaded successfully";
						UpdateStatus(summary);
					}
				}
				catch (Exception e)
				{
                  
					exceptions.Enqueue(e);
				}

			});
			if (exceptions.Count > 0) throw new AggregateException(exceptions);
		}

		public int GetSuccessCount(EntityType entityType, UploadSummary summary)
		{
			return new BLMisc().GetCount(entityType, summary);
		}
		public void UpdateFailedStatus(UploadSummary summary, ErrorMessage status)
		{
			summary.status = status.status.ToString();// StatusCodes.FAILED.ToString();
			summary.status_message = status.status.ToString();
			summary.end_on = DateTimeHelper.Now;
			summary.success_record = 0;
			summary.err_description = status.error_msg;
			summary.failed_record = summary.total_record;
			UpdateStatus(summary);
		}
        
        public UploadSummary Save(UploadSummary uploadSummary)
		{
			return DAUploadSummary.Instance.Save(uploadSummary);
		}
		public UploadSummary Get(int uploadId)
		{
			return DAUploadSummary.Instance.Get(uploadId);
		}
		public int GetInValidRecordCount(UploadSummary summary)
		{
			return new DAMisc().GetInValidRecordCount(summary);
		}
		public bool Delete(int id)
		{
			return DAUploadSummary.Instance.Delete(id);
		}

		public DataTable GetUploadLogs(int uploadid, string status)
		{
			return DAUploadSummary.Instance.GetUploadLogs(uploadid, status);
		}

        public DataTable GetUploadId(string planId, int user_id,string entity_type)
        {
            return DAUploadSummary.Instance.GetUploadId(planId, user_id, entity_type);
        }
        public List<ViewUploadSummary> GetUploadSummaryForGrid(UploadLogFilter objFilter)
		{
			return DAUploadSummary.Instance.GetUploadSummaryForGrid(objFilter);
		}
		public void UpdateStatus(UploadSummary uploadSummary)
		{

			//PostgreSQL.Instance.PostgresNotificationEvent += NotifyUpdatedStatus;
			string query = "listen upload_summary;";
			query = query + "update upload_summary set " +
				"user_id=" + uploadSummary.user_id + "," +
				"file_name='" + uploadSummary.file_name + "'," +
				"end_on='" + DateTimeHelper.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
				"entity_type='" + uploadSummary.entity_type + "'," +
				"total_record='" + uploadSummary.total_record + "'," +
				"failed_record='" + uploadSummary.failed_record + "'," +
				"success_record='" + uploadSummary.success_record + "'," +
				"err_description='" + uploadSummary.err_description + "'," +
				"line_number='" + uploadSummary.line_number + "'," +
				"total=" + uploadSummary.total + "," +
				"status='" + uploadSummary.status + "'," +
				"status_message='" + uploadSummary.status_message + "' " +
				"where id=" + uploadSummary.id + ";";
			postgreSQL.ExecuteQuery(query);
		}

		public void DeleteDataFromTempTable(UploadSummary summary)
		{
			BLLayer objBLLayer = new BLLayer();
			layerDetail networkLayerDetails = objBLLayer.GetLayerDetails(summary.entity_type);
			string sqlDelete = "DELETE FROM " + networkLayerDetails.data_upload_table + " WHERE IS_VALID=true and is_processed=true and UPLOAD_ID=" + summary.id + "";
			postgreSQL.ExecuteQuery(sqlDelete);
		}

		public string ShowOnMap(int id)
		{
			return DAUploadSummary.Instance.ShowOnMap(id);
		}
		public List<Mapping> GetMappings(string layerName)
		{
			return new DAMisc().GetMapping(layerName);
		}
        public List<Mapping> GetRegionProvinceMapping(string boundary_type)
        {
            return new DAMisc().GetRegionProvinceMapping(boundary_type);
        }
        
        public List<Dictionary<string, string>> getUploadTemplateSampleRecords(string entityType)
		{
			return DAUploadSummary.Instance.getUploadTemplateSampleRecords(entityType);
		}
		public List<Dictionary<string, string>> getUploadTemplateGuideLines(string entityType)
		{
			return DAUploadSummary.Instance.getUploadTemplateGuideLines(entityType);
		}
		public DbMessage checkTemplateExists(string entityType)
		{
			return DAUploadSummary.Instance.checkTemplateExists(entityType);
		}
		public List<Dictionary<string, string>> getKMLTemplate(string entityType)
		{
			return DAUploadSummary.Instance.getKMLTemplate(entityType);
		}
		public List<ErrorMessage> GetAggregateException(ConcurrentQueue<Exception> exceptions, UploadSummary summary)
		{
			List<ErrorMessage> errorMessage = new List<ErrorMessage>();
			string message = string.Empty;
			var exception = new AggregateException(exceptions);
			foreach (var e in exception.InnerExceptions)
			{
				ErrorMessage error = new ErrorMessage();
				error.status = StatusCodes.FAILED.ToString();
				error.exMessage = e;
				error.error_msg = message;
				errorMessage.Add(error);
			}
			return errorMessage;
		}
		public List<ColumnMapping> getColumnsMapping(int templateId)
		{
			return new DAColumnsMapping().getColumnsMapping(templateId);
		}
		public ColumnMappingTemplate SaveMappingTemplate(ColumnMappingTemplate mappingTemplate, int userId)
		{
			return new DAMappingTemplate().SaveMappingTemplate(mappingTemplate, userId);
		}
		public List<ColumnMappingTemplate> getMappingTemplates(int layerId, int userId)
		{
			return new DAMappingTemplate().getMappingTemplates(layerId, userId);
		}
        public List<ColumnMappingTemplate> getRegionProvinceMappingTemplates(int layerId, int userId, string boundary_type)
        {
            return new DAMappingTemplate().getRegionProvinceMappingTemplates(layerId, userId, boundary_type); 
        } 

        public List<ColumnMapping> getEntityTemplateColumns(string layerName)
		{
			var columns = new DAMisc().GetMapping(layerName); 
			return columns.Select(m => new ColumnMapping { template_db_column_name = m.DbColName, template_column_name = m.TemplateColName, imported_column_name = m.DbColName, is_mandatory = m.IsMandatory, is_template_column_required = m.is_template_column_required }).Where(x => x.template_column_name != "client_id" && x.template_column_name != "parent_client_id").ToList(); 
		}
        public List<ColumnMapping> getRegionProvinceTemplateColumns(string boundary_type)
        {
            var columns = new DAMisc().GetRegionProvinceMapping(boundary_type);
            return columns.Select(m => new ColumnMapping { template_db_column_name = m.DbColName, template_column_name = m.TemplateColName, imported_column_name = m.DbColName, is_mandatory = m.IsMandatory }).ToList();
        }
        public List<SourceIdList> getDxfSourceList()
		{
			return new DASridList().getDxfSourceList();
			
		}
		public void DeleteRecordFromTempTable(string EntityType,int id)
		{
			string strQuery = string.Format("select * from fn_uploader_delete_record('{0}',{1})", EntityType, id);
			postgreSQL.ExecuteQuery(strQuery);
		}
		public List<fileTypes> getfiletype(string moduleAbbr)
        {
			return new DAfiletype().getFileType(moduleAbbr);
		}
	}
	internal static class Utilities
	{
		public static List<List<T>> ToChunks<T>(this List<T> source, int chunkSize)
		{
			return source
				.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / chunkSize)
				.Select(x => x.Select(v => v.Value).ToList())
				.ToList();
		}
	}
}
