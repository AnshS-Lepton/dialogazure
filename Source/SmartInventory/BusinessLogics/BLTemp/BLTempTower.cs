using DataAccess;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogics
{
	public class BLTempTower : BLDataUploader
	{
		DATempTower daTemplate;
		public BLTempTower()
		{
			daTemplate = new DATempTower();
			daTemplate.DataUploaderNotificationHandler += NotifyUpdatedStatus;
		}
		public void Save(List<TempTower> lst)
		{
			daTemplate.Save(lst);
		}

		public List<ErrorMessage> InsertTowerIntoMainTable(UploadSummary summary)
		{
			return ParallelInsert(summary);
		}
		public List<TempTower> GetAll(int uploadId)
		{
			return daTemplate.GetAll(uploadId);
		}
		private List<ErrorMessage> ParallelInsert(UploadSummary summary)
		{
			List<ErrorMessage> lstErrorMessage = new List<ErrorMessage>();
			List<TempTower> lstTemp = GetAll(summary.id);
			int totalBatch = lstTemp.Select(x => x.batch_id).Distinct().Count();
			List<int> lstBatch = Enumerable.Range(1, totalBatch).ToList();
			var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
			object lockObj = new object();
			int currentStatus = 0;
			int CurrentValue = 0;
			DateTime dtStarts = DateTimeHelper.Now;
			var exceptions = new ConcurrentQueue<Exception>();
			string EstimatedRemainingTime;
			ParallelLoopResult result = Parallel.ForEach(lstBatch, options, (batchId) =>
			{
				try
				{
					lock (lockObj)
					{
						daTemplate.InsertIntoMainTable(summary, batchId);
						Interlocked.Increment(ref currentStatus);
						CurrentValue = (currentStatus * 100) / totalBatch;
						EstimatedRemainingTime = CalculateEstimatedRemainingTime(dtStarts, totalBatch, currentStatus);
						summary.status_message = EstimatedRemainingTimeMessage(CurrentValue, EstimatedRemainingTime);
						UpdateStatus(summary);
					}
				}
				catch (Exception e)
				{
					exceptions.Enqueue(e);
				}
			});
			if (exceptions.Count > 0)
			{
				return GetAggregateException(exceptions, summary);
			}
			return lstErrorMessage;
		}
	}
}
