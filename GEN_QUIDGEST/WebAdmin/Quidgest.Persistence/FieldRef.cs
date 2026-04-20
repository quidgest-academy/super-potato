using System;

namespace Quidgest.Persistence
{
    public class FieldRef
    {
        public string Area
        {
            get;
            private set;
        }

        public string Field
        {
            get;
            private set;
        }

		public string FullName
		{
			get;
			private set;
		}

        public FieldRef(string area, string field)
        {
            if (String.IsNullOrEmpty(area))
            {
                throw new ArgumentNullException("area");
            }

            if (String.IsNullOrEmpty(field))
            {
                throw new ArgumentNullException("field");
            }

            Area = area;
            Field = field;
			FullName = area + "." + field;
        }

        public static implicit operator string(FieldRef fr)
        {
            if (fr == null)
            {
                return null;
            }

            return fr.FullName;
        }
    }
}
