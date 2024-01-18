using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dashboard
{
	public class KPIMasterResult
	{
		public List<KPI_Master_Json> KPI_Master_Json { get; set; }
		public kpi_template kpi_Template { get; set; }
	}
}
