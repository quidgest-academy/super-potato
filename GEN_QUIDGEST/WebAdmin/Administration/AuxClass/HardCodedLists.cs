using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Administration.AuxClass
{
    /// <summary>
    /// Hard Coded Lists
    /// </summary>
	    public static class Extensions
		{
				/// <summary>
				///     A generic extension method that aids in reflecting 
				///     and retrieving any attribute that is applied to an `Enum`.
				/// </summary>
				public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
						where TAttribute : Attribute
				{
					return enumValue.GetType()
									.GetMember(enumValue.ToString())
									.First()
									.GetCustomAttribute<TAttribute>();
				}
		}

    public class HardCodedLists
    {
        public static Plataform GetPlatEnum(string plataforma)
        {
            Plataform plat = (Plataform)Enum.Parse(typeof(Plataform), plataforma, true);
            return plat;
        }

        /// <summary>
        /// Type de autenticação no Impersonate
        /// </summary>
        public enum ImpersonateAuth
        {
            Domain,
            Workgroup
        }

        /// <summary>
        /// Type de SGBD
        /// </summary>
        public enum DBMS
        {
            [Display(Name = @"SQL Server")]
            SQLSERVER,
            [Display(Name = @"SQL Server(compat)")]
            SQLSERVERCOMPAT,
            [Display(Name = @"Access")]
            ACCESS,
            [Display(Name = @"Oracle")]
            ORACLE,
            [Display(Name = @"MySQL")]
            MYSQL,
            [Display(Name = @"SQL Lite")]
            SQLITE,
            [Display(Name = @"Postgres")]
            POSTGRES
            //[Display(Name = "NÃO USAR")]
            //ERRO
        }

        /// <summary>
        /// Number Formats [ deciman & group]
        /// </summary>
        public enum DisplayNumberFormatDecimal
        {
            [Display(Name = "[ . ] dot")]
            Dot,
            [Display(Name = "[ , ] comma")]
            Comma     
        }
        public enum DisplayNumberFormatGroup
        {
            [Display(Name = "none")]
            None,
            [Display(Name = "[ . ] dot")]
            Dot,
            [Display(Name = "[ , ] comma")]
            Comma,
            [Display(Name = "[   ] blank")]
            Blank            
        }
        public enum DisplayNumberFormatNegative
        {
            [Display(Name = "[ - ] minus sign")]
            Minus,
            [Display(Name = "[ () ] parentheses")]
            Parentheses
        }

        /// <summary>
        /// Tipos de SGBD to BD's auxiliares
        /// </summary>
        public enum DBMSAux
        {
            SQL,
            ORACLE,
        }

        /// <summary>
        /// Type de Message Queue
        /// </summary>
        public enum TpMSQ
        {
            Classic,
            Journal
        }

        /// <summary>
        /// Type de plataforma
        /// </summary>
        public enum Plataform
        {
            BackOffice,
            CSharp_Web,
            ReIndex_Scripts
        }

        /// <summary>
        /// Formato das imagem externas
        /// </summary>
        public enum ImageFormat
        {
            bmp,
            gif,
            jpg, jpeg,
            png,
            mng, jng,
            ico,
            tif, tiff
        }


        /// <summary>
        /// Authentication 
        /// </summary>
        public static Dictionary<string, string> AuthIdent
        {
            get
            {
                return new Dictionary<string, string>{
                                                        {"GenioServer.security.QuidgestIdentityProvider,GenioServer","Quidgest"},
                                                        {"GenioServer.security.LdapIdentityProvider,GenioServer", "LDAP"},
                                                        {"", "Outro"}
                                                      };
            }
            //private set { }
        }

        /// <summary>
        /// Authentication 
        /// </summary>
        public static Dictionary<string, string> AuthRole
        {
            get
            {
                return new Dictionary<string, string>{
                                                        {"GenioServer.security.QuidgestRoleProvider,GenioServer", "Quidgest"},
                                                        {"", "Outro"}
                                                      };
            }
            //private set { }
        }
		
		public enum RelationsMode
        {
            [Display(Name = "Relações diretas")]
            DIRETAS,
            [Display(Name = "Relações indiretas")]
            INDIRETAS,
            [Display(Name = "Ambos os tipos")]
            AMBAS
    }
    }
}