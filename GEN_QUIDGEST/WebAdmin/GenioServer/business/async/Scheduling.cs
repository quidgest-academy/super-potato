using System;
using System.Collections.Generic;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using System.Linq;
using System.Reflection;


namespace CSGenio.business.async
{
    using ArgumentsMap = Dictionary<String, AsyncProcessArgument>;
    /// <summary>
    /// Class that represents a scheduled process. Each task must identify their process type and the mode.
    /// </summary>
    public abstract class ProcessScheduler
    {
        /// <summary>
        /// Checks if a process has the necessary conditions to be executed.
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="utilizador"></param>
        /// <returns></returns>
        public virtual bool GetPermission(PersistentSupport sp, User user)
        {
            return true;
        }

        /// <summary>
        /// Schedules a new process.
        /// </summary>
        /// <param name="sp">The persistent support.</param>
        /// <param name="user">The user.</param>
        public virtual string Schedule(PersistentSupport sp, User user)
        {
            //este agendar é feito pelo interface logo pode repetir sempre
            return this.Agenda(sp, user, null, true);
        }

        protected string Agenda(PersistentSupport sp, User user, string codpesoa, bool repeat)
        {
            if (GetPermission(sp, user))
            {
                GenioProcessManager pm = GenioProcessManager.SimpleProcessManager(sp, user);
                return pm.Agenda(this, codpesoa, user.Codpsw, repeat);
            }
            else
            {
                throw new BusinessException(Translations.Get("MSG_NO_PERMISSION_PROCESS", user.Language), "Agendamento.Agenda", "");
            }
        }

        /// <summary>
        /// Indicates the mode associated with the process (global, individual, etc)
        /// </summary>
        public virtual string getMode()
        {
            Type type = this.GetType();
            GenioProcessMode[] attributes = type.GetCustomAttributes(typeof(GenioProcessMode), true) as GenioProcessMode[];
            if (attributes.Length == 1)
            {
                return attributes[0].id;
            }
            else
            {
                throw new BusinessException("O processo não tem um tipo corretamente associado.", "getTipoProcesso", "O processo não tem um tipo corretamente associado.");
            }
        }

        /// <summary>
        /// Indicates the process type of associated with this scheduling.
        /// Should have an attribute of GenioProcessType
        /// </summary>
        public virtual string getProcessType()
        {
            Type type = this.GetType();
            GenioProcessType[] attributes = type.GetCustomAttributes(typeof(GenioProcessType), true) as GenioProcessType[];
            if (attributes.Length == 1)
            {
                return attributes[0].Id;
            }
            else
            {
                var msg = $"The process {type.Name} doesn't have a process type defined";
                throw new BusinessException(msg, "getTipoProcesso", msg);
            }
        }

        /// <summary>
        /// Obtains a dictionary containing a pair <FieldName, Value> from the class fields containing the attribute ProcessArgument.
        /// </summary>
        public ArgumentsMap GetArgumentsValues()
        {
            List<MemberInfo> members = GetArguments();
            ArgumentsMap arguments = new ArgumentsMap();
            foreach (var member in members)
            {
                ICollection<String> value = null;
                if (member.MemberType == MemberTypes.Field)
                {
                    FieldInfo field = (FieldInfo)member;
                    //Obtain the value and store it
                    value = Convert(field.FieldType, field.GetValue(this));

                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo property = (PropertyInfo)member;
                    //Obtain the value and store it
                    Object obj = property.GetValue(this, null);
                    value = Convert(property.PropertyType, obj);
                }
                else
                {
                    throw new Exception("Only properties and fields can be marked as ProcessArguments!");
                }
                GenioProcessArgument attribute = member.GetCustomAttribute(typeof(GenioProcessArgument)) as GenioProcessArgument;
                AsyncProcessArgument argument = new AsyncProcessArgument(value);
                if (!String.IsNullOrEmpty(attribute.Name))
                    argument.Name = attribute.Name;
                if (!String.IsNullOrEmpty(attribute.Field))
                {
                    string[] areaField = attribute.Field.Split('.');
                    argument.Field = new FieldRef(areaField[0], areaField[1]);
                }
                if (!String.IsNullOrEmpty(attribute.Array))
                    argument.Array = new ArrayInfo(attribute.Array);
                if (!String.IsNullOrEmpty(attribute.Function))
                {
                    argument.Function = this.GetType().GetMethod(attribute.Function, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                    //if (argument.Function == null)
                    //    GlobalFunctions.RegistarErro("Não foi possível encontrar a função " + attribute.Function, "GetArgumentValues",
                    //        "Não foi possível encontrar a função " + attribute.Function);
                }

                argument.Hide = attribute.Hide;
                argument.Docum = attribute.Docum;
                argument.KeyName = attribute.Key;
                arguments[member.Name] = argument;
            }
            return arguments;
        }

        /// <summary>
        /// Returns a list of members containing the attribute ProcessArg
        /// </summary>
        public List<MemberInfo> GetArguments()
        {
            Type tipo = this.GetType();
            List<MemberInfo> lstMembers = new List<MemberInfo>();
            do
            {
                var members = tipo.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                lstMembers.AddRange(members.Where(x => x.GetCustomAttributes(typeof(GenioProcessArgument), false).Length > 0).ToList());
                tipo = tipo.BaseType;

            } while (tipo != null);

            return lstMembers;
        }


        /// <summary>
        /// Function responsible by converting the object to a string to be stored in the DB.
        /// If you need to store a new type add an else if.
        /// </summary>
        /// <param name="type">Type of the value received</param>
        /// <returns>A string representing the object</returns>
        private static ICollection<String> Convert(Type type, object value)
        {
            List<String> lista = new List<string>();
            if (type.Equals(typeof(System.Int32)))
                lista.Add(Conversion.internalInt2String(value));
            else if (type.Equals(typeof(System.Double)) || type.Equals(typeof(System.Decimal)))
                lista.Add(Conversion.internalNumeric2String(value));

            else if (type.Equals(typeof(System.DateTime)))
                lista.Add(Conversion.internalDateTime2String(value, FieldFormatting.DATAHORA));

            else if (type.Equals(typeof(System.String)) || type.Equals(typeof(System.Guid)))
                lista.Add(Conversion.internalString2String(value));

            else if (type.Equals(typeof(System.Boolean)))
                lista.Add(Conversion.internalInt2String(Conversion.string2Int(value)));

            else if (value.GetType().GetInterface("ICollection") != null)
                return (ICollection<String>)value;

            else if (type.IsEnum)
                lista.Add(Conversion.internalInt2String((int)value));

            if (lista.Count == 0)
                throw new FrameworkException("Erro na conversão de tipo de campo interno para string.", "Conversao.interno2InternoString", "Erro na conversão de tipo de campo interno para string, o tipo de formatação do campo não está definido");
            else
                return lista;
        }

        public virtual bool MustNotify()
        {
            return true;
        }
    }

    
}