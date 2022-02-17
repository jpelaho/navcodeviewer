using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class Reference
    {
        public static string strInsertRef = "insert";
        public static string strUpdateRef = "update";
        public static string strFilterRef = "filter";
        public static string strDeleteRef = "delete";
        /// <summary>
        /// Type de l'objet conteneur (qui reference)
        /// </summary>
        public ObjectType RefBy_ObjetType { get; set; }
        /// <summary>
        /// ID de l'objet conteneur (qui reference)
        /// </summary>
        public int RefBy_ObjetID { get; set; }
        /// <summary>
        /// Type de l'objet référencé 
        /// </summary>
        public ObjectType Ref_ObjectType { get; set; }
        /// <summary>
        /// ID de l'objet référencé 
        /// </summary>
        public int Ref_ObjectID { get; set; }
        /// <summary>
        /// Ligne de la référence
        /// </summary>
        public int ReferenceLine { get; set; }
        public int ReferenceColumn { get; set; }
        /// <summary>
        /// Description/Nom de la référence
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nom de l'option
        /// </summary>
        public string OptionName { get; set; }
        /// <summary>
        /// Nature de l'objet référencé 
        /// (Qu'est ce qui est référencé)
        /// </summary>
        public RefType ReferenceType { get; set; }
        /// <summary>
        /// Object or Variable
        /// </summary>
        //public bool IsObject { get; set; }
        /// <summary>
        /// Scope de la reference
        /// </summary>
        //public RefScope ReferenceScope { get; set; }
        /// <summary>
        /// Scope pour les variable locales et parametres
        /// </summary>
        public CodeRange Scope { get; set; }
        ///// <summary>
        ///// Réference interne ou externe
        ///// </summary>
        //public bool IsExternalRef { get; set; }
        //public bool IsObject
        //{
        //    get
        //    {
        //        return RefBy_ObjetID > 0 && ReferenceType
        //            }
        //}
        public string LineText { get; set; }
    }
    public class Marker
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public int ICol { get; set; }
        //public bool EndingDelimiter { get; set; }
        //public bool StartingDelimiter { get; set; }
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Order);     
        }
    }
    public class DataItem
    {
        public string Name { get; set; }
        public string DataItemLinkReference { get; set; }
        public int Niveau { get; set; }
        public int DataItemTable { get; set; }
        public string IDRef { get; set; }
        public string PagePartID { get; set; }
    }
    public enum RefScope
    {
        /// <summary>
        /// Ref interne dans scope
        /// </summary>
        Local,
        /// <summary>
        /// Réference dans l'objet, variable globale
        /// </summary>
        Interne,
        /// <summary>
        /// Reférence externe
        /// </summary>
        Externe,
    }
    public enum RefType
    {
        Parameter,
        GlobalVariable,
        LocalVariable,
        ExternalObject,
        InternalFunction,
        ExternalFunction,
        InternalField,
        ExternalField,
        OptionVariable,
        /// <summary>
        /// Comme MODIFY, INSERT, DELETE
        /// </summary>
        RecordedFunction,
        None
    }
}
