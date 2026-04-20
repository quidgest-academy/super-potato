using System;
using CSGenio.business;
using CSGenio.framework;

namespace GenioMVC.ViewModels
{
	public class CalendarVariables
	{
        public CalendarVariables() { }

        public string startDateField { get; set; }
		public string endDateField { get; set; }
        public string minTime { get; set; }
        public string maxTime { get; set; }
        public string allDayField { get; set; }
        public string startTimeField { get; set; }
        public string endTimeField { get; set; }
        public string selectedDateField { get; set; }
        public string validDateStart { get; set; }
        public string validDateEnd { get; set; }

        public bool isScheduler { get; set; }
        public bool allDay { get; set; }
        public bool noDates { get; set; }
        public bool newEdit { get; set; }
        public bool hasNewResource { get; set; }
        public bool hasChildren { get; set; }

        public string resourceId { get; set; }
        public string dateTimeINI { get; set; }
        public DateTime? selectedDate { get; set; }

        public bool HasCalendarFields
		{
			get
			{
				return !string.IsNullOrWhiteSpace(startDateField) && !string.IsNullOrWhiteSpace(endDateField);
			}
		}

		public string DateMin
		{
			get
			{
				return (minTime ?? "00:00").Substring(0, 5);
			}
		}

		public string DateMax
		{
			get
			{
				return (maxTime ?? "23:59").Substring(0, 5);
			}
		}

		public DateTime? DateStart
		{
			get
			{
				if (selectedDate.HasValue)
					return allDay ? GenFunctions.DateSetTime(GenFunctions.DateFloorDay(selectedDate.Value), DateMin) : selectedDate.Value;
				return null;
			}
		}

		public DateTime? DateEnd
		{
			get
			{
				if (selectedDate.HasValue)
					return allDay ? GenFunctions.DateSetTime(GenFunctions.DateFloorDay(selectedDate.Value), DateMax) : selectedDate.Value;
				return null;
			}
		}
	}
}
