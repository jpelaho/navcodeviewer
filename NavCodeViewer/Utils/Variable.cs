using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class Variable
    {
        public VariableType Type { get; set; } = VariableType.Other;
        public int ID { get; set; }
        public int DefinitionLine { get; set; }
        /// <summary>
        /// Nom de la variable
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Vi Variable de type Option, enregistrer ici la valeur de OptionName
        /// </summary>
        public string OptionName { get; set; }
        /// <summary>
        /// Table temporaire pour eviter de collecter les fonction de mise a jour sur ces tables
        /// </summary>
        public bool IsTempVariable { get; set; }
        /// <summary>
        /// Temp var pour ne pas mettre a jour le nom si une propriéte VariableName a été définie
        /// </summary>
        public bool VariableNameIsSet { get; set; }
        /// <summary>
        /// Variable globale issue des elements d'un report ou xmlport, cette valeur permet de na pes enregistrer 2 references pour ce typa de variable
        /// </summary>
        public bool IsElementVariable { get; set; }

        public void SetTypeFromText(string type)
        {
            if (type == "Record") Type = VariableType.Record;
            if (type == "Page") Type = VariableType.Page;
            if (type == "Codeunit") Type = VariableType.CodeUnit;
            if (type == "Report") Type = VariableType.Report;
            if (type == "XMLport") Type = VariableType.XMLPort;
            if (type == "Query") Type = VariableType.Query;
        }
        public ObjectType GetTypeFromString(string type)
        {
            if (type == "Record") return  ObjectType.Table;
            if (type == "Page") return ObjectType.Page;
            if (type == "Codeunit") return ObjectType.CodeUnit;
            if (type == "Report") return ObjectType.Report;
            if (type == "XMLport") return ObjectType.XmlPort;
            if (type == "Query") return ObjectType.Query;
            return ObjectType.Table;
        }
        public ObjectType GetObjectType
        {
            get
            {
                return ConvertToObjectType(Type);
            }
        }
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Type);
        }
        public static ObjectType ConvertToObjectType(VariableType type)
        {
            //get
            {
                switch (type)
                {
                    case VariableType.Record:
                        return ObjectType.Table;
                    case VariableType.Page:
                        return ObjectType.Page;
                    case VariableType.CodeUnit:
                        return ObjectType.CodeUnit;
                    case VariableType.XMLPort:
                        return ObjectType.XmlPort;
                    case VariableType.Report:
                        return ObjectType.Report;
                    case VariableType.Query:
                        return ObjectType.Query;
                    case VariableType.MenuSuite:
                        return ObjectType.MenuSuite;
                    case VariableType.Other:
                        throw new ArgumentException();
                    default:
                        throw new ArgumentException();
                }
            }
        }

    }
    public enum CodeVariableType
    {
        None,
        Parameter,
        Function,
        Variable,
    }
    public enum VariableType
    {
        Record,
        Page,
        CodeUnit,
        XMLPort,
        Report,
        Query,
        MenuSuite,
        Option,
        Other
    }
}