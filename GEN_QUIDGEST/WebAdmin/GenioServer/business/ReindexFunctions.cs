using ExecuteQueryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CSGenio.persistence;
using CSGenio.business;
using CSGenio.framework;
using Quidgest.Persistence.GenericQuery;
using Quidgest.Persistence;

namespace CSGenio.business
{
    public class ReindexFunctions
    {
        public PersistentSupport sp { get; set; }
        public User user { get; set; }
        public bool Zero { get; set; }

        public ReindexFunctions(PersistentSupport sp, User user, bool Zero = false) {
            this.sp = sp;
            this.user = user;
            this.Zero = Zero;
        }   

        public void DeleteInvalidRows(CancellationToken cToken) {
            List<int> zzstateToRemove = new List<int> { 1, 11 };
            DataMatrix dm;
            sp.openConnection();

            /* --- FORCOUNT --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAcount.FldCodcount)
                .From(CSGenioAcount.AreaCOUNT)
                .Where(CriteriaSet.And().In(CSGenioAcount.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAcount model = new CSGenioAcount(user);
                model.ValCodcount = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- FORMEM --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAmem.FldCodmem)
                .From(CSGenioAmem.AreaMEM)
                .Where(CriteriaSet.And().In(CSGenioAmem.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAmem model = new CSGenioAmem(user);
                model.ValCodmem = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- UserLogin --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioApsw.FldCodpsw)
                .From(CSGenioApsw.AreaPSW)
                .Where(CriteriaSet.And().In(CSGenioApsw.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioApsw model = new CSGenioApsw(user);
                model.ValCodpsw = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- AsyncProcess --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_apr.FldCodascpr)
                .From(CSGenioAs_apr.AreaS_APR)
                .Where(CriteriaSet.And().In(CSGenioAs_apr.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_apr model = new CSGenioAs_apr(user);
                model.ValCodascpr = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- NotificationEmailSignature --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_nes.FldCodsigna)
                .From(CSGenioAs_nes.AreaS_NES)
                .Where(CriteriaSet.And().In(CSGenioAs_nes.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_nes model = new CSGenioAs_nes(user);
                model.ValCodsigna = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- NotificationMessage --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_nm.FldCodmesgs)
                .From(CSGenioAs_nm.AreaS_NM)
                .Where(CriteriaSet.And().In(CSGenioAs_nm.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_nm model = new CSGenioAs_nm(user);
                model.ValCodmesgs = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- FORCITY --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAcity.FldCodcity)
                .From(CSGenioAcity.AreaCITY)
                .Where(CriteriaSet.And().In(CSGenioAcity.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAcity model = new CSGenioAcity(user);
                model.ValCodcity = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- AsyncProcessArgument --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_arg.FldCodargpr)
                .From(CSGenioAs_arg.AreaS_ARG)
                .Where(CriteriaSet.And().In(CSGenioAs_arg.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_arg model = new CSGenioAs_arg(user);
                model.ValCodargpr = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- AsyncProcessAttachments --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_pax.FldCodpranx)
                .From(CSGenioAs_pax.AreaS_PAX)
                .Where(CriteriaSet.And().In(CSGenioAs_pax.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_pax model = new CSGenioAs_pax(user);
                model.ValCodpranx = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- UserAuthorization --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_ua.FldCodua)
                .From(CSGenioAs_ua.AreaS_UA)
                .Where(CriteriaSet.And().In(CSGenioAs_ua.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_ua model = new CSGenioAs_ua(user);
                model.ValCodua = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- FORAGENT --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAagent.FldCodagent)
                .From(CSGenioAagent.AreaAGENT)
                .Where(CriteriaSet.And().In(CSGenioAagent.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAagent model = new CSGenioAagent(user);
                model.ValCodagent = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- FORCTAX --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioActax.FldCodctax)
                .From(CSGenioActax.AreaCTAX)
                .Where(CriteriaSet.And().In(CSGenioActax.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioActax model = new CSGenioActax(user);
                model.ValCodctax = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- FORproperty --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAprope.FldCodprope)
                .From(CSGenioAprope.AreaPROPE)
                .Where(CriteriaSet.And().In(CSGenioAprope.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAprope model = new CSGenioAprope(user);
                model.ValCodprope = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- FORcontact --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAconta.FldCodconta)
                .From(CSGenioAconta.AreaCONTA)
                .Where(CriteriaSet.And().In(CSGenioAconta.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAconta model = new CSGenioAconta(user);
                model.ValCodconta = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- FORPHOTO --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAphoto.FldCodphoto)
                .From(CSGenioAphoto.AreaPHOTO)
                .Where(CriteriaSet.And().In(CSGenioAphoto.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAphoto model = new CSGenioAphoto(user);
                model.ValCodphoto = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                
            
            //Hard Coded Tabels
            //These can be directly removed

            /* --- FORmem --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORmem")
                .Where(CriteriaSet.And().In("FORmem", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORcfg --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORcfg")
                .Where(CriteriaSet.And().In("FORcfg", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORlstusr --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORlstusr")
                .Where(CriteriaSet.And().In("FORlstusr", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORlstcol --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORlstcol")
                .Where(CriteriaSet.And().In("FORlstcol", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORlstren --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORlstren")
                .Where(CriteriaSet.And().In("FORlstren", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORusrwid --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORusrwid")
                .Where(CriteriaSet.And().In("FORusrwid", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORusrcfg --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORusrcfg")
                .Where(CriteriaSet.And().In("FORusrcfg", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORusrset --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORusrset")
                .Where(CriteriaSet.And().In("FORusrset", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORwkfact --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORwkfact")
                .Where(CriteriaSet.And().In("FORwkfact", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORwkfcon --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORwkfcon")
                .Where(CriteriaSet.And().In("FORwkfcon", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORwkflig --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORwkflig")
                .Where(CriteriaSet.And().In("FORwkflig", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORwkflow --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORwkflow")
                .Where(CriteriaSet.And().In("FORwkflow", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORnotifi --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORnotifi")
                .Where(CriteriaSet.And().In("FORnotifi", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORprmfrm --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORprmfrm")
                .Where(CriteriaSet.And().In("FORprmfrm", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORscrcrd --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORscrcrd")
                .Where(CriteriaSet.And().In("FORscrcrd", "ZZSTATE", zzstateToRemove)));
                
            /* --- docums --- */
            sp.Execute(new DeleteQuery()
                .Delete("docums")
                .Where(CriteriaSet.And().In("docums", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORpostit --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORpostit")
                .Where(CriteriaSet.And().In("FORpostit", "ZZSTATE", zzstateToRemove)));
                
            /* --- hashcd --- */
            sp.Execute(new DeleteQuery()
                .Delete("hashcd")
                .Where(CriteriaSet.And().In("hashcd", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORalerta --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORalerta")
                .Where(CriteriaSet.And().In("FORalerta", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORaltent --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORaltent")
                .Where(CriteriaSet.And().In("FORaltent", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORtalert --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORtalert")
                .Where(CriteriaSet.And().In("FORtalert", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORdelega --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORdelega")
                .Where(CriteriaSet.And().In("FORdelega", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORTABDINAMIC --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORTABDINAMIC")
                .Where(CriteriaSet.And().In("FORTABDINAMIC", "ZZSTATE", zzstateToRemove)));
                
            /* --- UserAuthorization --- */
            sp.Execute(new DeleteQuery()
                .Delete("UserAuthorization")
                .Where(CriteriaSet.And().In("UserAuthorization", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORaltran --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORaltran")
                .Where(CriteriaSet.And().In("FORaltran", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORworkflowtask --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORworkflowtask")
                .Where(CriteriaSet.And().In("FORworkflowtask", "ZZSTATE", zzstateToRemove)));
                
            /* --- FORworkflowprocess --- */
            sp.Execute(new DeleteQuery()
                .Delete("FORworkflowprocess")
                .Where(CriteriaSet.And().In("FORworkflowprocess", "ZZSTATE", zzstateToRemove)));
                

            sp.closeConnection();
        }





        // USE /[MANUAL RDX_STEP]/
    }
}