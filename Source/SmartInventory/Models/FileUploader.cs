using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ExternalDataUploader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string file_name { get; set; }
        public string type { get; set; }
        public bool is_public { get; set; }
        public int created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string display_filename { get; set; }
        public int file_size { get; set; }
    }
    public class ExternalDataFileList
    {
        public string file_name { get; set; }
        public string type { get; set; }
        public int id { get; set; }
        public bool is_public { get; set; }
        public int created_by { get; set; }
        public string created_on { get; set; }
        public string file_size { get; set; }
        public int totalRecords { get; set; }
        public string display_filename { get; set; }
        [NotMapped]
        public string filepath { get; set; }
        [NotMapped]
        public int login_user { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }

    }
    public class LibraryAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int entity_system_id { get; set; }
        public string entity_type { get; set; }
        public string org_file_name { get; set; }
        public string file_name { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public string upload_type { get; set; }
        public string uploaded_by { get; set; }
        public int file_size { get; set; }
        public string entity_feature_name { get; set; }
        [NotMapped]
        public string file_size_converted { get; set; }
        [NotMapped]
        public byte[] attachmentSource { get; set; }
        public DateTime uploaded_on { get; set; } 
        public bool is_barcode_image { get; set; }
        public bool is_meter_reading_image { get; set; }
        public string document_type { get; set; }
        public int ticket_id { get; set; }
    }
    public class VailidateAttachment 
    {
        public string invalidattachmentType { get; set; }
        public string invalidattachmentsize { get; set; }
        public string invalidattachmentename { get; set; }

    }

    public class ImageResult
    {
        public string ImgSrc { get; set; }
        public string ImgName { get; set; }
        public int ImgId { get; set; }
        public string uploadedBy { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public Nullable<double> latitude { get; set; }
        public Nullable<double> longitude { get; set; }
        public string uploadedOn { get; set; }
        public string created_on { get; set; }
        public string file_ShortName { get; set; }
    }

    public class ImageDownload
    {
        public int systemId { get; set; }
        public string name { get; set; }
        public string location { get; set; }
    }

    public class FileModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class DocumentResult
    {
        //public int FileId {get;set;}
        //public string FileName{get;set;}
        //public string FileSize{get;set;}
        //public int UploadedBy { get; set; }
        public int Id { get; set; }
        public int EntitySystemId { get; set; }
        public string EntityType { get; set; }
        public string OrgFileName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileLocation { get; set; }
        public string UploadType { get; set; }
        public string UploadedBy { get; set; }
        public string file_size { get; set; }
        public string created_on { get; set; }
        public DateTime Uploaded_on { get; set; }
        public string File_ShortName { get; set; }        
        public string categorytype { get; set; } = "";
        public string document_type { get; set; } 
        [NotMapped]
        public Boolean delete_action { get; set; } = false;
        public string entity_feature_name { get; set; }
      
    }
    public class ExternalDataFilter : CommonGridAttributes
    {
        // will add more pproperties based on requirement.
    }

    public class ModelExternalDataDetails
    {
        public List<ExternalDataFileList> lstFileDetails { get; set; }
        public ExternalDataFilter objExternalDataFilter { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        [NotMapped]
        public string document_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> doctypeddllist { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstImageUpload { get; set; }
        [NotMapped]
        public string eType { get; set; }
        
        public ModelExternalDataDetails()
        {
            lstFileDetails = new List<ExternalDataFileList>();
            objExternalDataFilter = new ExternalDataFilter();
            lstUserModule = new List<string>();
            doctypeddllist = new List<DropDownMaster>();
        }
    }

    #region GeoTaggedImages BY ANTRA
    public class GeoTaggingImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string file_name { get; set; }
        public string file_description { get; set; }
        public string image_link { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public string upload_type { get; set; }
        public int uploaded_by { get; set; }
        public int file_size { get; set; }
        public string thumbimage_location { get; set; }
        public DateTime uploaded_on { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public Nullable<double> latitude { get; set; }
        public Nullable<double> longitude { get; set; }
        public string org_file_name { get; set; }
        [NotMapped]
        public string file_size_converted { get; set; }
        [NotMapped]
        public byte[] attachmentSource { get; set; }
        [NotMapped]
        public string Thumbgeotaggedpath { get; set; }
        [NotMapped]
        public string Org_geotaggedpath { get; set; }

    }
    public class GeoImageResult
    {
        public string ImgSrc { get; set; }
        public string ImgName { get; set; }
        public int ImgId { get; set; }
        public string uploadedBy { get; set; }
        public Nullable<double> latitude { get; set; }
        public Nullable<double> longitude { get; set; }
        public string created_on { get; set; }
        public string FileDescription { get; set; }
        public string FileExtension { get; set; }
        public string FileLocation { get; set; }
        public string UploadType { get; set; }
        public int FileSize { get; set; }
        public string ThumbImgLocation { get; set; }
        public int RegionId { get; set; }
        public int ProvinceId { get; set; }
        public string ImageLink { get; set; }
    }
    #endregion

    public class TrenchCustomerDetailsAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int customer_id { get; set; }
        public int trench_id { get; set; }
        public string org_file_name { get; set; }
        public string file_name { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public int file_size { get; set; }
        public int uploaded_by { get; set; }
        public DateTime? uploaded_on { get; set; }
        [NotMapped]
        public string uploaded_by_name { get; set; }
        [NotMapped]
        public string uploadedon { get; set; }
        [NotMapped]
        public string file_size_converted { get; set; }
    }
    public class TrenchCustomerDetailsAttachmentResult
    {
        public int id { get; set; }
        public int customer_id { get; set; }
        public int trench_id { get; set; }
        public string org_file_name { get; set; }
        public string file_name { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public int file_size { get; set; }
        public int uploaded_by { get; set; }
        public string uploaded_on { get; set; }
        public string uploaded_by_name { get; set; }
        public string uploadedon { get; set; }
        public string file_size_converted { get; set; }
    }
    public class FETOOLS_Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int tools_mapping_id { get; set; }
        public string file_name { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public string upload_type { get; set; }
        public string uploaded_by { get; set; }
        public DateTime? uploaded_on { get; set; }
        public int file_size { get; set; }
        //public string entity_feature_name { get; set; }
        //[NotMapped]
        //public string file_size_converted { get; set; }
        //[NotMapped]
       // public byte[] attachmentSource { get; set; }
        //public DateTime uploaded_on { get; set; }
        //public bool is_barcode_image { get; set; }
        //public bool is_meter_reading_image { get; set; }

    }

}
