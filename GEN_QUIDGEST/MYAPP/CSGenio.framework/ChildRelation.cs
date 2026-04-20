using System;

namespace CSGenio.framework
{
	/// <summary>
	/// This class represents a database relations. 
	/// It's used to verify related tables that need to be deleted when this record is deleted.
	/// </summary>
	public class ChildRelation
	{

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="areaChild">Child area</param>
		/// <param name="relatedFields">Field names that are related</param>
		/// <param name="procApagar">Enum with information of cascade deletion</param>
		public ChildRelation(string areaChild, string[] relatedFields, DeleteProc procApagar)
		{
			this.ChildArea=areaChild;
			this.ProcWhenDelete=procApagar;
			this.RelatedFields = relatedFields;
		}

		/// <summary>
		/// Compara este objecto com outro
		/// </summary>
		/// <param name="obj">Objecto a comparar</param>
		/// <returns>true se forem iguais, false caso contrário</returns>
		public override bool Equals(Object obj)
		{
			ChildRelation rf = obj as ChildRelation;
			if (rf != null)
			{
				if(ChildArea.Equals(rf.ChildArea) && ProcWhenDelete.Equals(rf.ProcWhenDelete))
				{	
					String[] camposRelRF = rf.RelatedFields;	
					if(camposRelRF.Length == RelatedFields.Length)
					{
						for(int i=0; i< RelatedFields.Length; i++)
							if(!RelatedFields[i].Equals(camposRelRF[i]))
								return false;
						return true;
					}		
					else
						return false;
				}
				else
					return false;
			}
			else
				return false;
		}

		/// <summary>
		/// Override obrigatório da classe Object
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return ChildArea.GetHashCode() + RelatedFields.GetHashCode();
		}

		/// <summary>
		/// Propriedade to acesso aos fields relacionados
		/// </summary>
		public string[] RelatedFields 
		{
			get;
		}

		/// <summary>
		/// Propriedade to acesso ao procedimento ao apagar
		/// </summary>
		public DeleteProc ProcWhenDelete
		{
			get;
		}

		/// <summary>
		/// Propriedade to acesso à área filha
		/// </summary>
		public string ChildArea
		{
			get;
		}
	}
}
