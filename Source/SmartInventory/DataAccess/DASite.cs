using System;
using System.Linq;
using Models;
using DataAccess.DBHelpers;
using Models;
using System.Collections.Generic;

namespace DataAccess
{
    public class DASite : Repository<Site>
    {

        public Site Save(Site site, int userId)
        {
            try
            {
                var objSite = repo.Get(x => x.system_id == site.system_id);
                if (objSite != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(site.modified_on, objSite.modified_on, site.modified_by, objSite.modified_by);
                    if (objPageValidate.message != null)
                    {
                        site.objPM = objPageValidate;
                        return site;
                    }

                    objSite.site_id = site.site_id;
                    objSite.address = site.address;
                    objSite.site_name = site.site_name;
                    objSite.on_air_date = site.on_air_date;
                    objSite.removed_date = site.removed_date;
                    objSite.tx_type  = site.tx_type;
                    objSite.tx_technology = site.tx_technology;
                    objSite.tx_segment = site.tx_segment;
                    objSite.tx_ring = site.tx_ring;
                    objSite.region = site.region;
                    objSite.province = site.province;
                    objSite.district = site.district;
                    objSite.region_address   = site.region_address;
                    objSite.depot    = site.depot;
                    objSite.ds_division = site.ds_division;
                    objSite.local_authority = site.local_authority;
                    objSite.latitude = site.latitude;
                    objSite.longitude = site.longitude;
                    objSite.owner_name = site.owner_name;
                    objSite.modified_by = userId;
                    objSite.modified_on = DateTimeHelper.Now;

                    objSite.access_24_7 = site.access_24_7;
                    objSite.tower_type = site.tower_type;
                    objSite.tower_height = site.tower_height ;
                    objSite.cabinet_type = site.cabinet_type;
                    objSite.solution_type = site.solution_type;
                    objSite.site_rank = site.site_rank;
                    objSite.self_tx_traffic = site.self_tx_traffic;
                    objSite.agg_tx_traffic = site.agg_tx_traffic;
                    objSite.metro_ring_utilization = site.metro_ring_utilization;
                    objSite.csr_count = site.csr_count;
                    objSite.dti_circuit = site.dti_circuit;
                    objSite.agg_01 = site.agg_01;
                    objSite.agg_02 = site.agg_02;
                    objSite.bandwidth = site.bandwidth;
                    objSite.ring_type = site.ring_type;    //for additional-attributes
                    objSite.link_id = site.link_id;
                    objSite.alias_name = site.alias_name;
                   
                    //objSite.served_by_ring=site.served_by_ring;
                    var Resp = repo.Update(objSite);
                   // DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(Resp.system_id, Models.EntityType.Coupler.ToString(), Resp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Coupler.ToString(), CouplerResp.province_id);
                    return Resp;

                }
                else
                {
                    site.created_by = userId;
                    site.created_on = DateTimeHelper.Now;
                   
                    var resultItem = repo.Insert(site);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId =(int) resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Site.ToString();
                    geom.commonName = resultItem.site_name;
                    geom.geomType = GeometryType.Point.ToString();
                   // geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                   // DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Coupler.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Coupler.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch (Exception ex)
            { throw; }
        }
        public int DeleteById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }

        public List<Site> GelAll(DateTime lastSuccessDate)
        {
            List<Site> lst = new List<Site>();
            try
            {
                lst = repo.GetAll(a=>a.created_on>= lastSuccessDate).ToList();
            }
            catch { throw; }
            return lst;
        }
    }
}
