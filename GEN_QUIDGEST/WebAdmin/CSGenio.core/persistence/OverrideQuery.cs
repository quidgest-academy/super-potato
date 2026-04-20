using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using System.Collections;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioServer.framework
{
    public class OverrideQuery
    {
        public static Listing APPDELEGALOGIN(CriteriaSet condition, User user, PersistentSupport sp, int nrRecords, Listing Qlisting)
        {
            return sp.select("APPDELEGALOGIN"
                , Qlisting
                , condition.Equal(CSGenioAdelega.FldCodpswdw, user.Codpsw)
                          .NotEqual(CSGenioAdelega.FldRevoked, 1)
                          .GreaterOrEqual(SqlFunctions.Custom("Diferenca_entre_Datas", new ColumnReference(CSGenioAdelega.FldDateini), SqlFunctions.SystemDate(), "D"), 0)
                          .GreaterOrEqual(SqlFunctions.Custom("Diferenca_entre_Datas", SqlFunctions.SystemDate(), new ColumnReference(CSGenioAdelega.FldDateend), "D"), 0)
                , 50, false);
        }
		
		public static Listing APPAUTHORIZATIONLIST(CriteriaSet condition, User user, PersistentSupport sp, int nrRecords, Listing Qlisting)
        {

            Listing list = sp.select("APPAUTHORIZATIONLIST"
                , Qlisting
                , condition
                , -1,false);      

            Hashtable modules = new Hashtable();

            DataRowCollection rows = list.DataMatrix.Tables[0].Rows;
            foreach (DataRow r in rows)
            {
                var module = r.ItemArray[1];
                
                //Make sure that it doesn't give exception when multiple roles are assigned
                if (modules.ContainsKey(module))
                    continue;

                modules.Add(module, module);
            }
            if (!modules.ContainsKey("FOR"))
            {
                // must create a user authorization level
                if (Log.IsDebugEnabled) Log.Debug("Processa pedido INS. [id] APPAUTHORIZATIONLIST [aplicacao] pswuserauthlevels");

                //instanciação da area base
                CSGenioApswuserauthlevels area = Area.createArea("pswuserauthlevels", user, list.Module) as CSGenioApswuserauthlevels;
                area.ValNivel = 0;
                area.ValModulo = "FOR";
                area.ValSistema = "FOR";
                // Value da key PSW
                area.ValCodpsw = condition.SubSets[0].Criterias[0].RightTerm.ToString();
                area.insertPseud(sp);
                rows.Add(area.ValCodua, area.ValModulo, area.ValNivel, area.ValCodpsw);
            }

            return list;
		}

     
    }
}
