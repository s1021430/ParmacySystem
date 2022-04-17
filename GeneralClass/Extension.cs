using System;
using System.Collections.Generic;
using AutoMapper;

namespace GeneralClass
{
    public enum DateTimeUnit
    {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Second
    }

    public static class Extension
    {
        public static List<T> CloneX<T>(this List<T> list)
        {
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T, T>();
            });
            IMapper mapper = config.CreateMapper();

            return list.ConvertAll(mapper.Map<T, T>); 
        }

        public static T Clone<T>(this T obj)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T, T>();
            });
            return new Mapper(config).Map<T>(obj);
        }

        public static string ToChineseDateTimeString(this DateTime date, DateTimeUnit unit,string separator = "")
        {
            var year = date.Year - 1911;
            if (unit == DateTimeUnit.Year)
                return $"{year:000}";
            var month = date.Month;
            if (unit == DateTimeUnit.Month)
                return $"{year:000}{separator}{month:00}";
            var day = date.Day;
            if (unit == DateTimeUnit.Day)
                return $"{year:000}{separator}{month:00}{separator}{day:00}";
            var hour = date.Hour;
            if (unit == DateTimeUnit.Hour)
                return $"{year:000}{separator}{month:00}{separator}{day:00}{separator}{hour:00}";
            var minute = date.Minute;
            if (unit == DateTimeUnit.Minute)
                return $"{year:000}{separator}{month:00}{separator}{day:00}{separator}{hour:00}{separator}{minute:00}";
            var second = date.Second;
            return $"{year:000}{separator}{month:00}{separator}{day:00}{separator}{hour:00}{separator}{minute:00}{separator}{second:00}";
        }
    }
}
