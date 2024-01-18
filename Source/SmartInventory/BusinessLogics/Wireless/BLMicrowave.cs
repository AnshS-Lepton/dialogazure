using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public class BLMicrowave
    {
        public MicrowaveLinkMaster Save(MicrowaveLinkMaster objMicrowaveLinkMaster, int userId)
        {
            return new DAMicrowave().Save(objMicrowaveLinkMaster, userId);
        }
        public int DeleteMicrowaveLinkById(int systemId)
        {
            return new DAMicrowave().DeleteMicrowaveLinkById(systemId);
        }
        public MicrowaveLinkMaster getMicrowaveLinkDetails(int systemId)
        {
            return new DAMicrowave().getMicrowaveLinkDetails(systemId);
        }
    }

    public class BLMicrowavelinkFeeder
    {
        public MicrowavelinkFeederSystem Save(MicrowavelinkFeederSystem objMicrowaveLinkFeeder, int userId)
        {
            return new DAMicrowavelinkFeeder().Save(objMicrowaveLinkFeeder, userId);
        }
        public int DeleteMicrowavelinkFeederById(int systemId)
        {
            return new DAMicrowavelinkFeeder().DeleteMicrowaveLinkFeederById(systemId);
        }
        public MicrowavelinkFeederSystem getMicrowavelinkFeederDetails(int systemId)
        {
            return new DAMicrowavelinkFeeder().getMicrowaveLinkFeederDetails(systemId);
        }
    }
    public class BLMicrowavelinkPower
    {
        public MicrowavelinkPower Save(MicrowavelinkPower objMicrowaveLinkMaster, int userId)
        {
            return new DAMicrowavelinkPower().Save(objMicrowaveLinkMaster, userId);
        }
        public int DeleteMicrowaveLinkById(int systemId)
        {
            return new DAMicrowavelinkPower().DeleteMicrowaveLinkPowerById(systemId);
        }
        public MicrowavelinkPower getMicrowaveLinkPowerDetails(int systemId)
        {
            return new DAMicrowavelinkPower().getMicrowaveLinkPowerDetails(systemId);
        }
    }
    public class BLMicrowaveAntenna
    {
        public MicrowavelinkAntenna Save(MicrowavelinkAntenna objMicrowaveLinkMaster, int userId)
        {
            return new DAMicrowaveAntenna().Save(objMicrowaveLinkMaster, userId);
        }
        public int DeleteMicrowaveLinkById(int systemId)
        {
            return new DAMicrowaveAntenna().DeleteMicrowaveLinkAntennaById(systemId);
        }
        public MicrowavelinkAntenna getMicrowaveLinkAntennaDetails(int systemId)
        {
            return new DAMicrowaveAntenna().getMicrowaveLinkAntennaDetails(systemId);
        }
    }

}
