using System;
using System.Collections.Generic;
using System.Globalization;

namespace LightSwitchApplication.UserCode
{
    public class TimeComparator
    {
        private static DateTime oldestTime = new DateTime(2014, 01, 01);
        private const int moisDebutAnneeFiscale = 5;

        public static bool containsDay(DateTime dateContenue, DateTime? dateDebut, DateTime? dateFin)
        {
            if (dateDebut.HasValue) {
                if (dateFin.HasValue)
                {
                    if (dateContenue.CompareTo(dateDebut.Value) > 0 && dateContenue.CompareTo(dateFin.Value) <= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (dateContenue.CompareTo(dateDebut.Value) > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public static DateTime[] getFridaysBeforeNow()
        {
            DateTime now = DateTime.Now;
            DateTime timeNow = DateTime.Parse("" + now.Year + "-" + now.Month + "-" + now.Day);
            int fridayDistance = (int)(now.DayOfWeek + 1) % 7;
            timeNow = timeNow.AddDays(-fridayDistance);
            List<DateTime> firsts = new List<DateTime>();
            int daysIterator = 0;

            for (DateTime day = timeNow; day.CompareTo(oldestTime) > 0; day = timeNow.AddDays(daysIterator * 7))
            {
                firsts.Add(day);
                daysIterator -= 1;
            }

            return firsts.ToArray();
        }

        public static DateTime getOldestTime()
        {
            return oldestTime;
        }

        public static IEnumerable<string> getStringDates(DateTime[] dates)
        {
            if (dates != null)
            {
                List<string> stringDates = new List<string>();
                foreach (DateTime d in dates)
                {
                    stringDates.Add(d.ToShortDateString());
                }

                return stringDates;
            }
            return null;
        }

        public static bool dateIsBetween(DateTime? date, DateTime dateDebut, DateTime dateFin)
        {
            if (date.HasValue)
            {
                return (date.Value.CompareTo(dateDebut) > 0 && date.Value.CompareTo(dateFin) <= 0);
            } else
            {
                return false;
            }
        }

        public static bool isActiveNow(DateTime? dateDebut, DateTime? dateFin)
        {
            if (dateDebut.HasValue)
            {
                if (dateDebut.Value < DateTime.Now)
                {
                    if (dateFin.HasValue)
                    {
                        return (dateFin.Value > DateTime.Now);
                    } else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool isActiveAtTime(DateTime? dateDebut, DateTime? dateFin, DateTime bareme)
        {
            if (dateDebut.HasValue)
            {
                if (dateDebut.Value < bareme)
                {
                    if (dateFin.HasValue)
                    {
                        return (dateFin.Value > bareme);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool isActiveBetween(DateTime? dateDebut, DateTime? dateFin, DateTime baremeDebut, DateTime baremeFin)
        {
            if (dateDebut.HasValue)
            {
                if (dateDebut.Value < baremeDebut)
                {
                    if (dateFin.HasValue)
                    {
                        return (dateFin.Value > baremeFin);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool hasSameWeek(DateTime? date, DateTime comparedToDate)
        {
            if (date.HasValue)
            {
                GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
                int dateWeek = cal.GetWeekOfYear(date.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Saturday);
                int comparedWeek = cal.GetWeekOfYear(comparedToDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Saturday);

                return (date.Value.Year == comparedToDate.Year && dateWeek == comparedWeek);

            }
            else
            {
                return false;
            }
        }

        public static bool hasSameYear(DateTime? date, DateTime comparedToDate, bool anneeFiscale)
        {
            if (date.HasValue)
            {
                if (anneeFiscale)
                {
                    return (date.Value.AddMonths(moisDebutAnneeFiscale).Year == comparedToDate.Year);
                }
                else
                {
                    return (date.Value.Year == comparedToDate.Year);
                }
            }
            else
            {
                return false;
            }
        }
    }
}
