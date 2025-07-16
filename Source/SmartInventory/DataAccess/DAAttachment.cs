using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using Models.WFM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.GoogleMapAPI;


namespace DataAccess
{
    public class DAAttachment : Repository<LibraryAttachment>
    {
        public LibraryAttachment SaveLibraryAttachment(LibraryAttachment objAttachment)
        {
            try
            {
                var resultItem = repo.Insert(objAttachment);
                return resultItem;
            }
            catch { throw; }
        }

        public bool CheckEntityFileExist(string fileName, int system_id,string documentType,string DocExtension)
        {
            var status = false;
            var records = repo.GetAll(m => m.entity_system_id == system_id && m.org_file_name.ToUpper() == fileName.ToUpper() && m.upload_type.ToUpper() == documentType.ToUpper() && m.file_extension.ToUpper() == DocExtension.ToUpper()).FirstOrDefault();
            if (records != null)
            {
                status = true;
            }
            return status;
        }


        public List<LibraryAttachment> getEntityImages(int system_id, string entity_type, string upload_type)
        {
            try
            {
                return repo.GetAll(m => m.entity_system_id == system_id && m.entity_type.ToUpper() == entity_type.ToUpper() && m.upload_type.ToUpper() == upload_type.ToUpper()).ToList();
            }
            catch { throw; }
        }

        public List<LibraryAttachment> getAttachmentDetails(int system_id, string entity_type, string upload_type, string fetureName)
        {
            try
            {
                //return repo.GetAll(m => m.entity_system_id == system_id && m.entity_type.ToUpper() == entity_type.ToUpper() && m.upload_type.ToUpper() == upload_type.ToUpper()).ToList();
                return repo.ExecuteProcedure<LibraryAttachment>("fn_get_Library_Attachments_Details", new { entity_system_id = system_id, entity_type = entity_type, upload_type = upload_type, p_feature_name = fetureName });
            }
            catch { throw; }
        }
        public List<LibraryAttachment> getAttachmentDetailsbyId(int system_id, string entity_type, string upload_type, string fileName, int userId, string fetureName)
        {
            try
            {
                //return repo.GetAll(m => m.entity_system_id == system_id && m.entity_type.ToUpper() == entity_type.ToUpper() && m.upload_type.ToUpper() == upload_type.ToUpper()).ToList();
                return repo.ExecuteProcedure<LibraryAttachment>("fn_get_library_attachments_details_byId", new { entity_system_id = system_id, entity_type = entity_type, upload_type = upload_type, p_fileName = fileName, p_userId = userId, p_feature_name = fetureName });
            }
            catch { throw; }
        }
        public LibraryAttachment getEntityImageById(int system_id)
        {
            try
            {
                return repo.GetById(m => m.id == system_id);
            }
            catch { throw; }
        }
        public int GetImageCount(int system_id, string documentType)
        {
            try
            {
                var features = documentType.Replace("/", "");
                return repo.GetAll(m =>
            m.entity_system_id == system_id &&
            m.entity_feature_name != null &&
            m.entity_feature_name.ToLower().Contains(features.ToLower())
        ).Count();
            }
            catch
            {
                throw;
            }
        }




        public LibraryAttachment getEntityDocumentById(int system_id)
        {
            try
            {
                var result = repo.ExecuteProcedure<LibraryAttachment>("fn_get_downloadtextformatfile", new { p_entity_system_id = system_id });
                return result?.FirstOrDefault();
            }
            catch { throw; }
        }
        public LibraryAttachment getEntityAttachmentDetails(int id)
        {
            try
            {
               
                return repo.GetById(m => m.id == id);
            }
            catch { throw; }
        }

        public int DeleteFromLibraryImage(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(m => m.id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }

        public int DeleteAttachmentById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(m => m.id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
        public string UpdateLibraryAttachmentbyId(string attachmentIds, int attachmentSystemIds)
        {
            try
            {
                var chk = repo.ExecuteProcedure<string>("fn_update_library_attachments_by_ids", new { p_attachmentIds = attachmentIds, p_attachmentSystemIds = attachmentSystemIds });
                return chk[0];
                //return repo.ExecuteProcedure<LibraryAttachment>("fn_get_Library_Attachments_Details", new { p_attachmentIds = attachmentIds, p_attachmentSystemIds = attachmentSystemIds });
            }
            catch { throw; }
        }

        public List<DocumentResult> getAttachmentDetailsDocs(int system_id, string entity_type, string upload_type, string fetureName)
        {
            try
            {
                return repo.ExecuteProcedure<DocumentResult>("fn_get_library_attachments_details_opsisp", new { entity_system_id = system_id, entity_type = entity_type, upload_type = upload_type, p_feature_name = fetureName });
            }
            catch { throw; }
        }
        public List<DocumentResult> getAttachmentDetailsDocsByTicketId(int ticket_id,int system_id, string entity_type, string upload_type, string fetureName)
        {
            try
            {
                return repo.ExecuteProcedure<DocumentResult>("fn_get_library_attachments_details_opsispByTicketId", new { entity_system_id = system_id, entity_type = entity_type, upload_type = upload_type, p_feature_name = fetureName, p_ticket_id = ticket_id });
            }
            catch { throw; }
        }
        
    }

    public class DAAttachments : Repository<UploadVendorSpecificationImageDoc>
    {
        public UploadVendorSpecificationImageDoc SaveLibraryAttachment(UploadVendorSpecificationImageDoc objAttachment)
        {
            try
            {
                var resultItem = repo.Insert(objAttachment);
                return resultItem;
            }
            catch { throw; }
        }
        public List<UploadVendorSpecificationImageDoc> getSpecificationAttachments(int specification_id, string upload_type = "")
        {
            try
            {

                return repo.ExecuteProcedure<UploadVendorSpecificationImageDoc>("fn_get_specification_attachments", new { p_specification_id = specification_id, p_upload_type = upload_type }, true);

            }
            catch { throw; }
        }
        public UploadVendorSpecificationImageDoc getSpecificationAttachmentsbyId(int id)
        {
            try
            {
                return repo.GetById(m => m.id == id);
            }
            catch { throw; }
        }

        public bool DeleteFromSpecificationImgDoc(int Id)
        {
            try
            {
                var objSystmId = repo.Get(m => m.id == Id);
                repo.Delete(objSystmId.id);
                return true;

            }
            catch { throw; }
        }
        public bool CheckSpecificationFileExist(string fileName, int id,string documentType,string doctype)
        {
            var status = false;
            var records = repo.GetAll(m => m.specification_id == id && m.org_file_name.ToUpper() == fileName.ToUpper() && m.upload_type.ToUpper() == documentType.ToUpper() && m.file_extension.ToUpper() == doctype.ToUpper()).FirstOrDefault();
            if (records != null)
            {
                status = true;
            }
            return status;
        }


    }

    #region GeoTaggedImages BY ANTRA
    public class DAGeoTagAttachment : Repository<GeoTaggingImages>
    {
        public List<GeoTaggingImages> getGeoTaggingAttachmentDetailsbyId(string upload_type, string fileName, int userId)
        {
            try
            {
                return repo.GetAll(m => m.upload_type.ToUpper() == upload_type.ToUpper() && m.uploaded_by == userId && m.org_file_name == fileName).ToList();
                //return repo.ExecuteProcedure<GeoTaggingImages>("fn_get_geotagging_attachments_details_byId", new { upload_type = upload_type, p_fileName = fileName, p_userId = userId });
            }
            catch (Exception ex)
            { throw ex; }
        }
        public GeoTaggingImages SaveGeoTaggingAttachment(GeoTaggingImages objAttachment)
        {
            try
            {
                var resultItem = repo.Insert(objAttachment);
                return resultItem;
            }
            catch { throw; }
        }
        public GeoTaggingImages getGeoDocumentById(int system_id)
        {
            try
            {
                return repo.GetById(m => m.id == system_id);
            }
            catch { throw; }
        }

        public int DeleteGeoAttachmentById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(m => m.id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
        public List<GeoTaggingImages> getGeoTaggedImages(int userId, string upload_type)
        {
            try
            {
                return repo.GetAll(m => m.uploaded_by == userId && m.upload_type.ToUpper() == upload_type.ToUpper()).ToList();
            }
            catch { throw; }
        }

        public GeoTaggingImages getGeotaggingAttachmentDetails(int id)
        {
            try
            {
                return repo.GetById(m => m.id == id);
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> Get_GeoTaggedImageList(GeoTaggedImagesFilter objFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_geotaggedimages_list", new
                {
                    p_pageno = objFilter.currentPage,
                    p_pagerecord = objFilter.pageSize,
                    p_sortcolname = objFilter.sort,
                    p_sorttype = objFilter.sortdir,
                    p_userid = objFilter.userId,
                }, true);
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<GeoTaggedImagesFilter> GetGeoTaggedImageByRegionProvince(GeoTaggedImagesFilter obj)
        {
            try
            {
                var lst = repo.ExecuteProcedure<GeoTaggedImagesFilter>("fn_get_geotaggedimagesbyregionprovince", new
                {
                    p_regionids= obj.SelectedRegionIds,
                    p_provinceids = obj.SelectedProvinceIds,
                    p_userid = obj.userId,
                }, true);
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CheckGeoFileExist(string fileName, string attachmentType, string fileExtension, int userId)
        {
            var status = false;
            var records = repo.GetAll(m => m.file_name.ToUpper() == fileName.ToUpper() && m.upload_type.ToUpper() == attachmentType.ToUpper() && m.file_extension.ToUpper() == fileExtension.ToUpper() && m.uploaded_by == userId).FirstOrDefault();
            if (records != null)
            {
                status = true;
            }
            return status;
        }
    }
    #endregion

    public class DATrenchCustomerDetailsAttachment : Repository<TrenchCustomerDetailsAttachment>
    {
        public TrenchCustomerDetailsAttachment SaveTrenchCustomerAttachment(TrenchCustomerDetailsAttachment objAttachment)
        {
            try
            {
                var resultItem = repo.Insert(objAttachment);
                return resultItem;
            }
            catch { throw; }
        }
        public List<TrenchCustomerDetailsAttachment> getTrenchCustomerAttachment(int customer_id, int trench_id)
        {
            try
            {

                return repo.ExecuteProcedure<TrenchCustomerDetailsAttachment>("fn_get_trench_customer_details_attachments", new { p_customer_id = customer_id, p_trench_id = trench_id }, true);

            }
            catch { throw; }
        }
        public TrenchCustomerDetailsAttachment getTrenchCustomerAttachmentbyId(int id)
        {
            try
            {
                return repo.GetById(m => m.id == id);
            }
            catch { throw; }
        }

        public bool DeleteTrenchCustomerAttachment(int Id)
        {
            try
            {
                var objSystmId = repo.Get(m => m.id == Id);
                repo.Delete(objSystmId.id);
                return true;

            }
            catch { throw; }
        }
        public bool CheckTrenchCustomerAttachmentExist(string fileName, int id, string doctype)
        {
            var status = false;
            var records = repo.GetAll(m => m.customer_id == id && m.org_file_name.ToUpper() == fileName.ToUpper() && m.file_extension.ToUpper() == doctype.ToUpper()).FirstOrDefault();
            if (records != null)
            {
                status = true;
            }
            return status;
        }



    }
}
