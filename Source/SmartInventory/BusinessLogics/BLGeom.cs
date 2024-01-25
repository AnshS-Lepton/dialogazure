using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BASaveEntityGeometry
    {
        private static BASaveEntityGeometry objSaveEntityGeom = null;
        private static readonly object lockObject = new object();
        public static BASaveEntityGeometry Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSaveEntityGeom == null)
                    {
                        objSaveEntityGeom = new BASaveEntityGeometry();
                    }
                }
                return objSaveEntityGeom;
            }
        }

        public string SaveEntityGeometry(InputGeom objgeom)
        {
            return DASaveEntityGeometry.Instance.SaveEntityGeom(objgeom);
        }
        public bool BoundaryIntersectCheck(string geom, string entityType)
        {
            return DASaveEntityGeometry.Instance.BoundaryIntersectCheck(geom, entityType);
        }

        public DbMessage ValidateEntityByGeom(EditGeomIn objgeom)
        {
            return DASaveEntityGeometry.Instance.ValidateEntityByGeom(objgeom);
        }

        public DbMessage EditEntityGeometry(EditGeomIn objgeom)
        {
            return DASaveEntityGeometry.Instance.EditEntityGeometry(objgeom);
        }
        public GeometryDetail PointToPolygon(int systemId, string entityType)
        {
            return DASaveEntityGeometry.Instance.PointToPolygon(systemId, entityType);
        }
        public DbMessage PolygonToPoint(int systemId, string entityType)
        {
            return DASaveEntityGeometry.Instance.PolygonToPoint(systemId, entityType);
        }
		public bool UpdateMicroductGeom(int systemId)
		{
			return DASaveEntityGeometry.Instance.UpdateMicroductGeom(systemId);

		}
        public bool CheckGeomWithin(string geomBig, string geomSmall)
        {
            return DASaveEntityGeometry.Instance.CheckGeomWithin(geomBig, geomSmall);
        }
        public void UpdateDuctLocation(int systemId, string distance , string OffsetDir)
        {
             DASaveEntityGeometry.Instance.UpdateDuctLocation(systemId, distance, OffsetDir);
        } 
        public void UpdateDuctColorCode(int systemId, int trench_id, int ductcount)
        {
             DASaveEntityGeometry.Instance.UpdateDuctColorCode(systemId, trench_id, ductcount);
        }

    }
}
