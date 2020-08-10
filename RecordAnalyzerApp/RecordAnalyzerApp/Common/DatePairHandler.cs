using RecordAnalyzerApp.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;

namespace RecordAnalyzerApp.Common
{
    public class DatePairHandler : INotifyPropertyChanged
    {
        const string SQL_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public event Action<DateTime, DateTime> DatePeriodChanged = null;
        public event PropertyChangedEventHandler PropertyChanged = null;

        public string Title { get; set; }

        public DateTime FromNow { get; set; }
        public DateTime ToThen { get; set; }

        public string FromDateString
        {
            get
            {
                return FromNow.ToString(SQL_DATE_FORMAT);
            }
        }

        public string ToDateString
        {
            get
            {
                return ToThen.ToString(SQL_DATE_FORMAT);
            }
        }

        public DatePairHandler()
        {
            Title = "Selected Criteria";
            UpdateDatePeriod(nameof(DatePairOption.ThisMonth));
        }

        public void CreateAndHandleClickEvent(ToolbarItem menu)
        {
            menu.Clicked += (object sender, EventArgs e) =>
            {
                UpdateDatePeriod((sender as ToolbarItem).Text);
                DatePeriodChanged?.Invoke(FromNow, ToThen);
            };
        }

        public void UpdateDatePeriod(string commandText)
        {
            var now = DateTime.Now;
            FromNow = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            switch (commandText)
            {
                case nameof(DatePairOption.Today):
                    ToThen = FromNow.AddDays(1);
                    break;

                case nameof(DatePairOption.Yesterday):
                    FromNow = FromNow.AddDays(-1);
                    ToThen = FromNow.AddDays(1);
                    break;

                case nameof(DatePairOption.ThisWeek):
                    FromNow = FromNow.AddDays(-Convert.ToDouble(FromNow.DayOfWeek) + 1);
                    ToThen = FromNow.AddDays(6);
                    break;

                case nameof(DatePairOption.LastWeek):
                    FromNow = FromNow.AddDays(-Convert.ToDouble(FromNow.DayOfWeek) - 6);
                    ToThen = FromNow.AddDays(6);
                    break;

                case nameof(DatePairOption.ThisMonth):
                    FromNow = FromNow.AddDays(1 - FromNow.Day);
                    ToThen = FromNow.AddMonths(1);
                    break;

                case nameof(DatePairOption.LastMonth):
                    FromNow = FromNow.AddMonths(-1).AddDays(1 - FromNow.AddMonths(-1).Day);
                    ToThen = FromNow.AddMonths(1);
                    break;

                case nameof(DatePairOption.ThisYear):
                    FromNow = new DateTime(FromNow.Year, 1, 1, 0, 0, 0);
                    ToThen = FromNow.AddYears(1);
                    break;

                case nameof(DatePairOption.LastYear):
                    FromNow = new DateTime(FromNow.Year - 1, 1, 1, 0, 0, 0);
                    ToThen = new DateTime(FromNow.Year + 1, 1, 1, 0, 0, 0);
                    break;

                case nameof(DatePairOption.ALL):
                    FromNow = new DateTime(2000, 1, 1, 0, 0, 0);
                    ToThen = new DateTime(2100, 1, 1, 0, 0, 0);
                    break;

                default:
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DatePair"));
        }

        internal List<DatePairHandler> GetDatePairList(DateGroupingOption dateGroupingCriteria)
        {
            List<DatePairHandler> list = new List<DatePairHandler>();

            DateTimeFormatInfo dfi = DateTimeFormatInfo.InvariantInfo;
            Calendar cal = dfi.Calendar;

            var startDate = FromNow;
            while (startDate < ToThen)
            {
                switch (dateGroupingCriteria)
                {
                    case DateGroupingOption.Hourly:
                        list.Add(new DatePairHandler
                        {
                            Title = startDate.Hour.ToString(),
                            FromNow = startDate,
                            ToThen = startDate.AddHours(1)
                        });

                        startDate = startDate.AddHours(1);

                        break;
                    case DateGroupingOption.Daily:
                        list.Add(new DatePairHandler
                        {
                            Title = string.Format("{0},{1}", startDate.Day, dfi.GetAbbreviatedMonthName(startDate.Month)),
                            FromNow = startDate,
                            ToThen = startDate.AddDays(1)
                        });

                        startDate = startDate.AddDays(1);

                        break;
                    case DateGroupingOption.Weekly:
                        list.Add(new DatePairHandler
                        {
                            Title = string.Format("Week:{0}", cal.GetWeekOfYear(startDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek)),
                            FromNow = startDate,
                            ToThen = startDate.AddDays(7)
                        });

                        startDate = startDate.AddDays(7);

                        break;
                    case DateGroupingOption.Monthly:
                        list.Add(new DatePairHandler
                        {
                            Title = dfi.GetAbbreviatedMonthName(startDate.Month),
                            FromNow = startDate,
                            ToThen = startDate.AddMonths(1)
                        });

                        startDate = startDate.AddMonths(1);

                        break;
                    case DateGroupingOption.Yearly:
                        list.Add(new DatePairHandler
                        {
                            Title = startDate.Year.ToString(),
                            FromNow = startDate,
                            ToThen = startDate.AddYears(1)
                        });

                        startDate = startDate.AddYears(1);

                        break;
                    case DateGroupingOption.None:
                    default:
                        startDate = ToThen;
                        break;
                }
            }
            
            if (list.Count == 0)
            {
                list.Add(this);
            }

            return list;
        }

        public override string ToString()
        {
            return string.Format("{0:dd MMM yyyy} to {1:dd MMM yyyy}", FromNow, ToThen);
        }
    }
}