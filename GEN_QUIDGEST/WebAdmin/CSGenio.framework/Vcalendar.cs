using System;
using System.IO;

namespace CSGenio.framework
{
    /// <summary>
    /// Outlook vcalendar data structure
    /// </summary>
    public class Vcalendar
    {
        public Vcalendar(string prodid,
                         string version,
                         DateTime dtstart,
                         DateTime dtend,
                         string location,
                         string sumary,
                         string description,
                         int priority
        )
        {
            Prodid = prodid;
            Version = version;
            Dtstart = dtstart;
            Dtend = dtend;
            Location = location;
            Sumary = sumary;
            Description = description;
            Priority = priority;
        }

        public Vcalendar()
        {
            Prodid = "-//Microsoft Corporation//Outlook 10.0 MIMEDIR//EN";
            Version = "1.0";
            Dtstart = DateTime.Now;
            Dtend = DateTime.Now;
            Location = "";
            Sumary = "";
            Description = "";
            Priority = 1;
        }


        public string Prodid { get; set; }

        public string Version { get; set; }

        public DateTime Dtstart { get; set; }

        public DateTime Dtend { get; set; }

        public string Location { get; set; }

        public string Sumary { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public string buildDtstart()
        {
            return constroiDtaux(Dtstart);
        }

        public string buildDtend()
        {
            return constroiDtaux(Dtend);
        }

        private string constroiDtaux(DateTime dtaux)
        {
            string dtst = dtaux.Year.ToString();

            if (dtaux.Month < 10)
                dtst += "0" + dtaux.Month.ToString();
            else
                dtst += dtaux.Month.ToString();

            if (dtaux.Day < 10)
                dtst += "0" + dtaux.Day.ToString();
            else
                dtst += dtaux.Day.ToString();

            dtst += "T";

            if (dtaux.Hour < 10)
                dtst += "0" + dtaux.Hour.ToString();
            else
                dtst += dtaux.Hour.ToString();

            if (dtaux.Minute < 10)
                dtst += "0" + dtaux.Minute.ToString();
            else
                dtst += dtaux.Minute.ToString();

            if (dtaux.Second < 10)
                dtst += "0" + dtaux.Second.ToString();
            else
                dtst += dtaux.Second.ToString();

            return dtst;
        }

        /// <summary>
        /// Saves vcalendar to a file
        /// </summary>
        public bool buildVcalendar(string pathFile)
        {
            if(!string.IsNullOrEmpty(pathFile))
            {
                using(StreamWriter SW = new StreamWriter(pathFile, false, System.Text.Encoding.Default))
                {
                    SW.WriteLine("BEGIN:VCALENDAR");
                    SW.WriteLine("PRODID:" + Prodid);
                    SW.WriteLine("VERSION:" + Version);//1.0
                    SW.WriteLine("BEGIN:VEVENT");
                    SW.WriteLine("DTSTART:" + buildDtstart());//20060706T070000
                    SW.WriteLine("DTEND:" + buildDtend());//20060706T073000
                    SW.WriteLine("LOCATION;ENCODING=QUOTED-PRINTABLE:" + Location);//Local
                    SW.WriteLine("UID:040000008200E00074C5B7101A82E00800000000D057697107A1C6010000000000000000100");
                    SW.WriteLine(" 000007E21EDF318757341A9013C72A2B896F9");
                    SW.WriteLine("DESCRIPTION;ENCODING=QUOTED-PRINTABLE:" + Description); //body da tarefa
                    SW.WriteLine("SUMMARY;ENCODING=QUOTED-PRINTABLE:" + Sumary);//tarrghghjgjdgfdghjfgdjfgjdsf 
                    SW.WriteLine("PRIORITY:" +Priority );//3
                    SW.WriteLine("END:VEVENT");
                    SW.WriteLine("END:VCALENDAR");
                }
                return true;
            } else 
                return false;
        }

    }
}
