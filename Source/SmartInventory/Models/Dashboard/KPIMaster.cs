using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dashboard
{
	public class Kpi_Master
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int kpi_id { get; set; }
		public int category_id { get; set; }
		[Required(ErrorMessage = "KPI Name Field is Required")]
		public string kpi_name { get; set; }

		[NotMapped]
		public string category_name { get; set; }
		[Required(ErrorMessage = "KPI Source Field is Required")]
		public string kpi_source { get; set; }
		[Required(ErrorMessage = "X-Axis Column Field is Required")]
		public string x_axis_column { get; set; }
		[Required(ErrorMessage = "X-Axis Column Field is Required")]
		public string y_axis_columns { get; set; }

		public bool is_visible { get; set; }
		[NotMapped]
		public bool is_selected { get; set; }

		[NotMapped]
		public PageMessage pageMsg { get; set; } = new PageMessage();
		[NotMapped]
		public List<KeyValueDropDown> ddlCategoryName { get; set; } = new List<KeyValueDropDown>();



	}
	public class KPI_VM
	{
		public int kpi_id { get; set; }
		public string kpi_name { get; set; }
		public string kpi_category { get; set; }
		public string chart { get; set; }

	}
	public class KeyValueDropDown
	{
		public int key { get; set; }
		public string value { get; set; }
		public string ddtype { get; set; }
	}
	public class Kpi_Category
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int category_id { get; set; }
		[Required(ErrorMessage = "Category Name is required.")]
		public string category_name { get; set; }
		public bool is_visible { get; set; }

		[NotMapped]
		public PageMessage pageMsg { get; set; } = new PageMessage();
		//[NotMapped]
		//public List<KeyValueDropDown> ddlCategoryName { get; set; }


	}
	public class KPI_Master_Json
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int kpi_id { get; set; }
		public int category_id { get; set; }

		public string kpi_name { get; set; }

		public string category_name { get; set; }

		public string kpi_source { get; set; }

		public string x_axis_column { get; set; }

		public string y_axis_columns { get; set; }
	}
	public class MessageSummaryDDL
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}
	public class Kpi_Master_View : KPI_Master_Json
	{

		[NotMapped]
		public bool is_visible { get; set; }
		[NotMapped]
		public bool is_selected { get; set; }

	}
	public class kpi_template
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int template_id { get; set; }
		public string template_name { get; set; }

		public int kpi_category_id { get; set; }

		public int kpi_category_name_id { get; set; }

		public string x_axis_column { get; set; }

		public string y_axis_columns { get; set; }

		//public string created_on { get; set; }
		public DateTime created_on { get; set; }

		public int created_by { get; set; }
		[NotMapped]
		public PageMessage pageMsg { get; set; } = new PageMessage();



	}

}

