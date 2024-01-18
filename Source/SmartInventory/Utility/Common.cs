
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
	public class Common
	{
       
        public InstallationInfo GetInstallationInfo(dynamic objWirelessItem)
		{
			InstallationInfo installation = new InstallationInfo();
			installation.installation_id = objWirelessItem.installation_id;
			installation.entity_system_id = objWirelessItem.system_id;
			installation.entity_type = objWirelessItem.entityType;
			installation.installation_number = objWirelessItem.installation_number;
			installation.installation_year = objWirelessItem.installation_year;
			installation.production_year = objWirelessItem.production_year;
			installation.installation_company = objWirelessItem.installation_company;
			installation.installation_technician = objWirelessItem.installation_technician;
			installation.installation = objWirelessItem.installation;

			return installation;
		}
		private static double ToRad(double angle)
		{
			return angle * Math.PI / 180;
		}
		private static double ToDeg(double angle)
		{
			return angle * 180 / Math.PI;
		}
		public static string GetSectorsGeometry(double latitude, double longitude, double angle, string sectorType)
		{
			// Computation of polygon points
			double vDistance = 0;

			double vLatitude = Convert.ToDouble(latitude);
			double vLongitude = Convert.ToDouble(longitude);
			double vAngle = Convert.ToDouble(angle);
			switch (sectorType)
			{
				case "UMTS":    //GSM
					vDistance = (0.1 / 6371) / 2;  // distance = 50 mtr
					break;
				case "3G":    //3G
					vDistance = (0.1 / 6371) / 1.5;  // distance = 100 mtr
					break;
				case "LTE":    //LTE
					vDistance = 0.1 / 6371;  // distance  to be discuss
					break;
				default:

					break;
			}

			double vAngle1 = ToRad(vAngle + 20);
			double vAngle2 = ToRad(vAngle - 20);

			double vLat1 = ToRad(vLatitude);
			double vLng1 = ToRad(vLongitude);

			double vNewLat1 = Math.Asin(Math.Sin(vLat1) * Math.Cos(vDistance) + Math.Cos(vLat1) * Math.Sin(vDistance) * Math.Cos(vAngle1));
			double vNewLng1 = vLng1 + Math.Atan2(Math.Sin(vAngle1) * Math.Sin(vDistance) * Math.Cos(vLat1), Math.Cos(vDistance) - Math.Sin(vLat1) * Math.Sin(vNewLat1));

			double vNewLat2 = Math.Asin(Math.Sin(vLat1) * Math.Cos(vDistance) + Math.Cos(vLat1) * Math.Sin(vDistance) * Math.Cos(vAngle2));
			double vNewLng2 = vLng1 + Math.Atan2(Math.Sin(vAngle2) * Math.Sin(vDistance) * Math.Cos(vLat1), Math.Cos(vDistance) - Math.Sin(vLat1) * Math.Sin(vNewLat2));

			//ST_GeomFromText('POLYGON((" + longLat + "))
			//string polygonSPGeometry = vLatitude + " " + vLongitude + "," + ToDeg(vNewLat1) + " " + ToDeg(vNewLng1) + "," + ToDeg(vNewLat2) + " " + ToDeg(vNewLng2) + "," + vLatitude + " " + vLongitude;
			string polygonSPGeometry = vLongitude + " " + vLatitude + "," + ToDeg(vNewLng1) + " " + ToDeg(vNewLat1) + "," + ToDeg(vNewLng2) + " " + ToDeg(vNewLat2) + "," + vLongitude + " " + vLatitude;
			return polygonSPGeometry;
		}
	}
}
