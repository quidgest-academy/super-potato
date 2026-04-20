using System;
using System.Collections;
using CSGenio.framework;
using System.Collections.Generic;
using CSGenio.business;

namespace CSGenio.persistence
{
    /// <summary>
    /// As areas devem obedecer a este interface to que o suporte persistente saiba o que pode chamar delas
    /// </summary>
    /// <remarks>
    /// Este interface garante o isolamento de camadas, permitindo no entanto ao suporte persistente manipular objectos
    /// de business, lendo e escrevendo to estes. 
    /// Nunca usar directamente uma implementação de area como parametro, usar sempre ou extender este interface.
    /// </remarks>
    public interface IArea
    {
        string QSystem { get; }
        string TableName { get; }
        string ShadowTabName { get; }
        string Alias { get; }
        string PrimaryKeyName { get; }
        //TODO: O suporte persistente não deveria de precisar desta noção
        string ShadowTabKeyName { get; }
        Dictionary<string, Field> DBFields { get; }
        Dictionary<string, RequestedField> Fields { get; }

        AreaInfo Information { get; }
        Dictionary<string, Relation> ParentTables { get; }

        User User { get; }

        object returnValueField(string fieldName);

        void insertNameValueField(string fieldName, object Qvalue, bool fromDatabase = false);

        string QPrimaryKey { get; }
        
        bool IsBookmarkLocked { get; set; }
    }
}