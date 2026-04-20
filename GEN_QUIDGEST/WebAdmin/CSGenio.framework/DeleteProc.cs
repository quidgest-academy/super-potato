using System;

namespace CSGenio.framework
{

    public sealed class DeleteProc
    {
		private readonly string id;
		private DeleteProc(string anID) {this.id = anID; }
		public string toString() {return this.id; }

		/// <summary>
		/// Don't delete
		/// </summary>
		public readonly static DeleteProc NA = new DeleteProc("NA");
		/// <summary>
		/// Delete
		/// </summary>
		public readonly static DeleteProc AP = new DeleteProc("AP");
		/// <summary>
		/// Clears relation
		/// </summary>
		public readonly static DeleteProc DM = new DeleteProc("DM");
		/// <summary>
		/// Deletes if in new state
		/// </summary>
		public readonly static DeleteProc AN = new DeleteProc("AN");

		public override bool Equals(object obj)
		{
			if(obj is DeleteProc pa)
				return pa.id.Equals(id, StringComparison.Ordinal);
			if (obj is string s)
				return s.Equals(id, StringComparison.Ordinal);
			return false;
		}

		public const string DONT_DELETE = "NA";
        public const string DELETE_RECORD = "AP";
        public const string CLEAR = "DM";
        public const string DELETE_IF_NEW = "AN";

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
		
	}
}
