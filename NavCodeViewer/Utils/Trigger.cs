using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class Trigger
    {
        public TriggerType Type
        {
            get
            {
                TriggerType typ = TriggerType.None;
                GetNameAndType(ref typ);
                return typ;
            }
        }
        public string Prefix { get; set; }
        public int DefLine { get; set; }
        public TextBlockCodeType TextBlockType { get; set; }
        public string Name { 
            get
            {
                TriggerType typ = TriggerType.None;
                return GetNameAndType(ref typ);
            }
        }
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, TextBlockType);
        }
        string GetNameAndType(ref TriggerType triggerType)
        {
            switch (TextBlockType)
            {
                case TextBlockCodeType.None:
                    {

                        return "";
                    }
                case TextBlockCodeType.Function:
                    return "";
                case TextBlockCodeType.Table_OnValidate:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnValidate";
                    }
                case TextBlockCodeType.Table_OnLookUp:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnLookUp";
                    }
                case TextBlockCodeType.Table_OnInsert:
                    {
                        triggerType = TriggerType.Object;
                        return "OnInsert";
                    }
                case TextBlockCodeType.Table_OnModify:
                    {
                        triggerType = TriggerType.Object;
                        return "OnModify";
                    }
                case TextBlockCodeType.Table_OnDelete:
                    {
                        triggerType = TriggerType.Object;
                        return "OnDelete";
                    }
                case TextBlockCodeType.Table_OnRename:
                    {
                        triggerType = TriggerType.Object;
                        return "OnRename";
                    }
                case TextBlockCodeType.Page_OnInit:
                    {
                        triggerType = TriggerType.Object;
                        return "OnInit"; 
                    }
                case TextBlockCodeType.Page_OnOpen:
                    {
                        triggerType = TriggerType.Object;
                        return "OnOpenPage";
                    }
                case TextBlockCodeType.Page_OnClose:
                    {
                        triggerType = TriggerType.Object;
                        return "OnClose";
                    }
                case TextBlockCodeType.Page_OnFindRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnFindRecord";
                    }
                case TextBlockCodeType.Page_OnNextRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnNextRecord";
                    }
                case TextBlockCodeType.Page_OnAfterGetRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnAfterGetRecord";
                    }
                case TextBlockCodeType.Page_OnNewRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnNewRecord";
                    }
                case TextBlockCodeType.Page_OnInsertRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnInsertRecord";
                    }
                case TextBlockCodeType.Page_OnModifyRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnModifyRecord";
                    }
                case TextBlockCodeType.Page_OnDeleteRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnDeleteRecord";
                    }
                case TextBlockCodeType.Page_OnQueryClose:
                    {
                        triggerType = TriggerType.Object;
                        return "OnQueryClose";
                    }
                case TextBlockCodeType.Page_OnAfterGetCurrRecord:
                    {
                        triggerType = TriggerType.Object;
                        return "OnAfterGetCurrRecord";
                    }
                case TextBlockCodeType.Page_OnValidate:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnValidate"; 
                    }
                case TextBlockCodeType.Page_OnLookup:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnLookup";
                    }
                case TextBlockCodeType.Page_OnDrillDown:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnDrillDown";
                    }
                case TextBlockCodeType.Page_OnAssistEdit:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAssistEdit";
                    }
                case TextBlockCodeType.Page_OnControlAddIn:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnControlAddIn";
                    }
                case TextBlockCodeType.Page_OnAction:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAction";
                    }
                case TextBlockCodeType.Report_OnPreDataItem:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnPreDataItem";
                    }
                case TextBlockCodeType.Report_OnAfterGetRecord:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAfterGetRecord";
                    }
                case TextBlockCodeType.Report_OnPostDataItem:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnPostDataItem";
                    }
                case TextBlockCodeType.Report_OnInitReport:
                    {
                        triggerType = TriggerType.Object;
                        return "OnInitReport";
                    }
                case TextBlockCodeType.Report_OnPreReport:
                    {
                        triggerType = TriggerType.Object;
                        return "OnPreReport";
                    }
                case TextBlockCodeType.Report_OnPostReport:
                    {
                        triggerType = TriggerType.Object;
                        return "OnPostReport";
                    }
                case TextBlockCodeType.CodeUnit_OnRun:
                    {
                        triggerType = TriggerType.Object;
                        return  "OnRun";
                    }
                case TextBlockCodeType.XMLPort_OnInit:
                    {
                        triggerType = TriggerType.Object;
                        return "OnInitXMLport";
                    }
                case TextBlockCodeType.XMLPort_OnPre:
                    {
                        triggerType = TriggerType.Object;
                        return "OnPreXMLport";
                    }
                case TextBlockCodeType.XMLPort_OnPost:
                    {
                        triggerType = TriggerType.Object;
                        return "OnPostXMLport";
                    }
                case TextBlockCodeType.XMLPort_OnAfterAssignVariable:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAfterAssignVariable";
                    }
                case TextBlockCodeType.XMLPort_OnBeforePassVariable:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnBeforePassVariable";
                    }
                case TextBlockCodeType.XMLPort_OnAfterInitRecord:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAfterInitRecord";
                    }
                case TextBlockCodeType.XMLPort_OnBeforeInsertRecord:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnBeforeInsertRecord";
                    }
                case TextBlockCodeType.XMLPort_OnPreXMLItem:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnPreXMLItem";
                    }
                case TextBlockCodeType.XMLPort_OnAfterGetRecord:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAfterGetRecord";
                    }
                case TextBlockCodeType.XMLPort_OnAfterInsertRecord:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAfterInsertRecord";
                    }
                case TextBlockCodeType.XMLPort_OnBeforeModifyRecord:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnBeforeModifyRecord";
                    }
                case TextBlockCodeType.XMLPort_OnAfterModifyRecord:
                    {
                        triggerType = TriggerType.Field;
                        return Prefix + " - " + "OnAfterModifyRecord";
                    }
                default:return "";
            }
        }
        public enum TriggerType
        {
            None,
            Object,
            Field
        }
    }
}