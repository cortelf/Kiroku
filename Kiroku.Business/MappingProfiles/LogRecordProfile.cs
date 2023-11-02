using AutoMapper;
using Kiroku.Business.Models;
using Kiroku.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.MappingProfiles
{
    public class LogRecordProfile: Profile
    {
        public LogRecordProfile()
        {
            CreateMap<CreateLogRecord, Log>()
                .ForMember(x => x.Data, opt => opt.MapFrom(x => x.Properties))
                .ForMember(x => x.Time, opt => opt.MapFrom(x => x.Timestamp));
        }
    }
}
