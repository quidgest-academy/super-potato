using System;
using System.IO;

namespace GenioMVC.Helpers
{
	public class FileUpload
	{
		private string pathdocums;

		public FileUpload(string model, string field, string fileName)
		{
			this.Model = model.ToLower();
			this.Field = field.ToLower();
			this.FileName = fileName;
			this.MaxFileNameSize = GetFileNameSize();
			this.OverrideExistingFile = true;
			this.Pathdocums = CSGenio.framework.Configuration.PathDocuments;

		}

		public int MaxFileNameSize { get; private set; }

		public string SavedFileName { get; private set; }

		public string FileName { get; set; }

		public string Pathdocums
		{
			get { return pathdocums; }
			private set
			{
				if (String.IsNullOrEmpty(value))
					throw new ArgumentException("Path of documents not defined.");
				else
					pathdocums = value;
			}
		}

		public string Model { get; set; }

		public string Field { get; set; }

		public bool OverrideExistingFile { get; set; }

		private int GetFileNameSize()
		{
			string areaField = Field.Substring(0, 3) == "val" ? Field.Substring(3).ToLower() : Field.ToLower();
			CSGenio.business.AreaInfo ainfo = CSGenio.business.Area.GetInfoArea(Model);
			CSGenio.framework.Field Qfield;
			if (ainfo.DBFields.TryGetValue(areaField, out Qfield))
				return Qfield.FieldSize;
			else
				throw new ArgumentException("Field name is not valid.");
		}

		private string NewName(string filename)
		{
			this.FileName = filename.Substring(filename.LastIndexOf('\\') + 1);

			filename = Guid.NewGuid().ToString("N") + "_" + this.FileName;

			if (filename.Length > MaxFileNameSize)
				filename = filename.Substring(filename.Length - MaxFileNameSize);

			return filename;
		}

		public static string OriginalName(string filename)
		{
			if (filename.IndexOf('_') != -1)
				filename = filename.Substring(filename.IndexOf('_') + 1);
			return filename;
		}

		public bool Save(byte[] file)
		{
			return internalSave(file);
		}

		public bool Save(Stream file)
		{
			var buffer = new byte[file.Length];
			file.Read(buffer, 0, buffer.Length);
			file.Dispose();
			return internalSave(buffer);
		}

		private bool internalSave(byte[] file)
		{
			if (!OverrideExistingFile)
				SavedFileName = NewName(this.FileName);
			else
				SavedFileName = this.FileName;

			try
			{
				File.WriteAllBytes(Path.Combine(this.Pathdocums, SavedFileName), file);
				return true;
			}
			catch (IOException ex)
			{
				SavedFileName = string.Empty;
				throw new Exception(ex.Message);
			}
		}

		public bool Delete()
		{
			return Delete(this.FileName);
		}

		public bool Delete(string fileName)
		{
			try
			{
				string path = Path.Combine(this.Pathdocums, fileName);
				if (File.Exists(path))
					File.Delete(path);
				return true;
			}
			catch (IOException ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
