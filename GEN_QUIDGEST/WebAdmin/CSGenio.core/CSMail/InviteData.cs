using System;
using System.Text;

namespace CSGenio.core
{
    /// <summary>
    /// Class that represents the information to send an invite
    /// The invite will be sent in attach in ics extension so that each interface read it in it's own way (gmail, outlook, etc)
    /// </summary>
    public class InviteData
    {
        /// <summary>
        /// The start date and time of the event
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// The end date and time of the event.
        /// </summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// The location where the event will take place.
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// The name of the event organizer.
        /// </summary>
        public string OrganizerName { get; set; }
        /// <summary>
        /// The email address of the event organizer.
        /// </summary>
        public string OrganizerEmail { get; set; }
        /// <summary>
        /// The time in minutes before the event to set a reminder alarm.
        /// </summary>
        public int SetAlarm { get; set; }
        /// <summary>
        /// A detailed description of the event with additional information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A brief summary of the event, used as a title or highlight in the invitation.
        /// </summary>
        public string Summary { get; set; }


        /// <summary>
        /// Create the invite structure (ics extension) to send attached to the email
        /// </summary>
        /// <param name="To">Invite destination</param>
        /// <returns>Returns the ics structure to be attached to the email</returns>
        public string CreateCalendarEvent(string To)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("BEGIN:VCALENDAR");
            builder.AppendLine("PRODID:-//Company//Outlook MIMEDIR//EN");
            builder.AppendLine("VERSION:2.0");
            builder.AppendLine("METHOD:REQUEST");
            builder.AppendLine("BEGIN:VEVENT");
            builder.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", this.StartDateTime));
            builder.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
            builder.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", this.EndDateTime));
            builder.AppendLine($"ORGANIZER;CN={this.OrganizerName}:{this.OrganizerEmail}");
            builder.AppendLine($"ATTENDEE;CN={string.Empty};RSVP=TRUE:{To}");
            builder.AppendLine($"DESCRIPTION:{this.Description}");
            builder.AppendLine($"SUMMARY:{this.Summary}");
            builder.AppendLine($"LOCATION:{this.Location}");
            builder.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            builder.AppendLine("SEQUENCE:0");
            builder.AppendLine("STATUS:CONFIRMED");
            builder.AppendLine("TRANSP:OPAQUE");
            if (this.SetAlarm > 0)
            {
                builder.AppendLine("BEGIN:VALARM");
                builder.AppendLine($"TRIGGER:-PT{this.SetAlarm}M");
                builder.AppendLine("ACTION:Accept");
                builder.AppendLine("DESCRIPTION:Reminder");
                builder.AppendLine("X-MICROSOFT-CDO-BUSYSTATUS:BUSY");
                builder.AppendLine("END:VALARM");
            }
            builder.AppendLine("END:VEVENT");
            builder.AppendLine("END:VCALENDAR");
            return builder.ToString();
        }
    }
}