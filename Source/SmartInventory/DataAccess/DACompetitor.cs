using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
   public class DACompetitor : Repository<Competitor>
    {
        public Competitor SaveCompetitor(Competitor objCompetitor, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objCompetitor.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objCompetitor.modified_on, objitem.modified_on, objCompetitor.modified_by, objitem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objCompetitor.objPM = objPageValidate;
                        return objCompetitor;
                    }
                    objitem.name = objCompetitor.name;
                    objitem.address = objCompetitor.address;
                    objitem.icon_path = objCompetitor.icon_path;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.status_remark = objCompetitor.status_remark;
                    objitem.other_info = objCompetitor.other_info;  //for additional-attributes
                    objitem.requested_by = objCompetitor.requested_by;
                    objitem.request_approved_by = objCompetitor.request_approved_by;
                    objitem.request_ref_id = objCompetitor.request_ref_id;
                    objitem.origin_ref_id = objCompetitor.origin_ref_id;
                    objitem.origin_ref_description = objCompetitor.origin_ref_description;
                    objitem.origin_from = objCompetitor.origin_from;
                    objitem.origin_ref_code = objCompetitor.origin_ref_code;
                    var CompetitorResp = repo.Update(objitem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(CompetitorResp.system_id, Models.EntityType.Competitor.ToString(), CompetitorResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Competitor.ToString(), CompetitorResp.province_id);
                    return CompetitorResp;
                }
                else
                {
                    objCompetitor.created_by = userId;
                    //objCompetitor.status = "A";
                    objCompetitor.status = String.IsNullOrEmpty(objCompetitor.status) ? "A" : objCompetitor.status;
                    objCompetitor.created_on = DateTimeHelper.Now;
                    var resultItem = repo.Insert(objCompetitor);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.Competitor.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Competitor.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Competitor.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }

        #region Additional-Attributes
        public string GetOtherInfoCompetitor(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
    }
}
