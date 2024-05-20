using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
using Models.ISP;
using DataAccess.ISP;
using System.Data;
using Models.Admin;

namespace BusinessLogics
{
    public class BLAttachment
    {
        public LibraryAttachment SaveLibraryAttachment(LibraryAttachment objAttachment)
        {
            return new DAAttachment().SaveLibraryAttachment(objAttachment);
        }
      

        public bool CheckEntityFileExist(string fileName, int system_id,string documentType,string DocExtension)
        {
            return new DAAttachment().CheckEntityFileExist(fileName, system_id, documentType, DocExtension);
        }
        public string UpdateLibraryAttachmentbyId(string attachmentIds, int attachmentSystemIds)
        {
            return new DAAttachment().UpdateLibraryAttachmentbyId(attachmentIds, attachmentSystemIds);
        }

        public List<LibraryAttachment> getEntityImages(int system_id, string entity_type, string upload_type)
        {
            return new DAAttachment().getEntityImages(system_id, entity_type, upload_type);
        }

        public List<LibraryAttachment> getAttachmentDetails(int system_id, string entity_type, string upload_type, string fetureName = "")
        {
            return new DAAttachment().getAttachmentDetails(system_id, entity_type, upload_type, fetureName);
        }
        public List<LibraryAttachment> getAttachmentDetailsbyId(int system_id, string entity_type, string upload_type, string fileName, int userId, string fetureName = "")
        {
            return new DAAttachment().getAttachmentDetailsbyId(system_id, entity_type, upload_type, fileName, userId, fetureName);
        }

        public LibraryAttachment getEntityImageById(int system_id)
        {
            return new DAAttachment().getEntityImageById(system_id);
        }

        public LibraryAttachment getEntityDocumentById(int system_id)
        {
            return new DAAttachment().getEntityDocumentById(system_id);
        }
        public LibraryAttachment getEntityAttachmentDetails(int id)
        {
            return new DAAttachment().getEntityAttachmentDetails(id);
        }


        public int DeleteFromLibraryImage(int systemId)
        {
            return new DAAttachment().DeleteFromLibraryImage(systemId);
        }

        public int DeleteAttachmentById(int systemId)
        {
            return new DAAttachment().DeleteAttachmentById(systemId);
        }
        //--------------------------------------------------specification
        public UploadVendorSpecificationImageDoc SaveSpecificationAttachment(UploadVendorSpecificationImageDoc objAttachment)
        {
            return new DAAttachments().SaveLibraryAttachment(objAttachment);
        }
        public List<UploadVendorSpecificationImageDoc> getSpecificationAttachments(int specification_id, string upload_type = "")
        {
            return new DAAttachments().getSpecificationAttachments(specification_id, upload_type);
        }
        public UploadVendorSpecificationImageDoc getSpecificationAttachmentsbyid(int id)
        {
            return new DAAttachments().getSpecificationAttachmentsbyId(id);
        }
        public bool DeleteFromSpecificationImgDoc(int Id)
        {
            return new DAAttachments().DeleteFromSpecificationImgDoc(Id);
        }
        public List<DocumentResult> getAttachmentDetailsDocs(int system_id, string entity_type, string upload_type, string fetureName = "")
        {
            return new DAAttachment().getAttachmentDetailsDocs(system_id, entity_type, upload_type, fetureName);
        }
        public bool CheckSpecificationFileExist(string fileName, int system_id,string documentType,string doctype)
        {
            return new DAAttachments().CheckSpecificationFileExist(fileName, system_id, documentType, doctype);
        }
    }

    #region GeoTaggedImages BY ANTRA
    public class BLGeoTaggingAttachment
    {
        public List<GeoTaggingImages> getGeoTaggingAttachmentDetailsbyId(string upload_type, string fileName, int userId)
        {
            return new DAGeoTagAttachment().getGeoTaggingAttachmentDetailsbyId(upload_type, fileName, userId);
        }

        public GeoTaggingImages SaveGeoTaggingAttachment(GeoTaggingImages objAttachment)
        {
            return new DAGeoTagAttachment().SaveGeoTaggingAttachment(objAttachment);
        }
        public GeoTaggingImages getGeoDocumentById(int system_id)
        {
            return new DAGeoTagAttachment().getGeoDocumentById(system_id);
        }
        public int DeleteGeoAttachmentById(int systemId)
        {
            return new DAGeoTagAttachment().DeleteGeoAttachmentById(systemId);
        }
        public List<GeoTaggingImages> getGeoTaggedImages(int user_id, string upload_type)
        {
            return new DAGeoTagAttachment().getGeoTaggedImages(user_id, upload_type);
        }
        public GeoTaggingImages getGeotaggingAttachmentDetails(int id)
        {
            return new DAGeoTagAttachment().getGeotaggingAttachmentDetails(id);
        }
        public List<Dictionary<string, string>> Get_GeoTaggedImageList(GeoTaggedImagesFilter objfilter)
        {
            return new DAGeoTagAttachment().Get_GeoTaggedImageList(objfilter);
        }
        public List<GeoTaggedImagesFilter> GetGeoTaggedImageByRegionProvince(GeoTaggedImagesFilter obj)
        {
            return new DAGeoTagAttachment().GetGeoTaggedImageByRegionProvince(obj);
        }
        public bool CheckGeoFileExist(string fileName, string attachmentType, string fileExtension,int userId)
        {
            return new DAGeoTagAttachment().CheckGeoFileExist(fileName, attachmentType, fileExtension, userId);
        }
    }
    #endregion

    public class BLTrenchCustomerDetailsAttachment
    {
        public TrenchCustomerDetailsAttachment SaveTrenchCustomerAttachment(TrenchCustomerDetailsAttachment objAttachment)
        {
            return new DATrenchCustomerDetailsAttachment().SaveTrenchCustomerAttachment(objAttachment);
        }
        public List<TrenchCustomerDetailsAttachment> getTrenchCustomerAttachment(int customer_id, int trench_id)
        {
            return new DATrenchCustomerDetailsAttachment().getTrenchCustomerAttachment(customer_id, trench_id);
        }
        public TrenchCustomerDetailsAttachment getTrenchCustomerAttachmentbyId(int id)
        {
            return new DATrenchCustomerDetailsAttachment().getTrenchCustomerAttachmentbyId(id);
        }
        public bool DeleteTrenchCustomerAttachment(int Id)
        {
            return new DATrenchCustomerDetailsAttachment().DeleteTrenchCustomerAttachment(Id);
        }

        public bool CheckTrenchCustomerAttachmentExist(string fileName, int system_id, string doctype)
        {
            return new DATrenchCustomerDetailsAttachment().CheckTrenchCustomerAttachmentExist(fileName, system_id, doctype);
        }

    }
}
