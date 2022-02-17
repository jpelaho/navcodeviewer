using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Domain
{
    public enum TextBlockType
    {
        None,
        ObjectsProperties,
        Properties,
        Elements,
        Controls,
        Fields,
        FieldGroups,
        Keys,
        Code,
        DataSet,
        RequestPage,
        RequestPageProperties,
        RequestPageControls,
        Labels,
        RdlData,
        XMLEvents,
        MenuNodes
    }
    public enum TextBlockCodeType
    {
        None,
        Function,

        //Table
        Table_OnValidate,
        Table_OnLookUp,
        Table_OnInsert,
        Table_OnModify,
        Table_OnDelete,
        Table_OnRename,

        //Page General
        Page_OnInit,
        Page_OnOpen,
        Page_OnClose,
        Page_OnFindRecord,
        Page_OnNextRecord,
        Page_OnAfterGetRecord,
        Page_OnNewRecord,
        Page_OnInsertRecord,
        Page_OnModifyRecord,
        Page_OnDeleteRecord,
        Page_OnQueryClose,
        Page_OnAfterGetCurrRecord,

        //Page field
        Page_OnValidate,
        Page_OnLookup,
        Page_OnDrillDown,
        Page_OnAssistEdit,
        Page_OnControlAddIn,
        Page_OnAction,

        //Report General
        Report_OnPreDataItem,
        Report_OnAfterGetRecord,
        Report_OnPostDataItem,

        //Report DataSet
        Report_OnInitReport,
        Report_OnPreReport,
        Report_OnPostReport,

        //Codeunit
        CodeUnit_OnRun,
        XMLPort_OnInit,
        XMLPort_OnPre,
        XMLPort_OnPost,
        XMLPort_OnAfterAssignVariable,
        XMLPort_OnBeforePassVariable,
        XMLPort_OnAfterInitRecord,
        XMLPort_OnBeforeInsertRecord,
        XMLPort_OnPreXMLItem,
        XMLPort_OnAfterGetRecord,
        XMLPort_OnAfterInsertRecord,
        XMLPort_OnBeforeModifyRecord,
        XMLPort_OnAfterModifyRecord,
        Page_ActionList,
        Query_OnBeforeOpen,
    }
}
