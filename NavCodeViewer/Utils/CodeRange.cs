using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class CodeRange
    {
        public Place Start { get; set; }
        public Place End { get; set; }
        public List<Variable> LocalVariables { get; set; } = new List<Variable>();
        public bool CanHaveReference { get; set; }
        public TypeOfCodeRange RangeType { get; set; } = TypeOfCodeRange.None;
        public string RangeName { get; set; }
        public bool IsFunctionDefinition { get; internal set; }
        /// <summary>
        /// Table utilisée dans le scope de la zone
        /// </summary>
        public int RecId { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public CodeRange(int iStartChar, int iStartLine, int iEndChar, int iEndLine)
        {
            Start = new Place(iStartChar, iStartLine);
            End = new Place(iEndChar, iEndLine);
        }

        public CodeRange()
        {
        }
        /// <summary>
        /// Constructor. Creates range of the line
        /// </summary>
        public CodeRange(int iLine, string line)
        {
            Start = new Place(0, iLine);
            End = new Place(line.Length, iLine);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CodeRange(Place start, Place end)
        {
            this.Start = start;
            this.End = end;
        }
        public CodeRange CreateSubRange(Place start, Place end)
        {
            var r = new CodeRange(start, end);
            r.CanHaveReference = CanHaveReference;
            r.IsFunctionDefinition = IsFunctionDefinition;
            r.LocalVariables = LocalVariables;
            r.RangeName = RangeName;
            r.RangeType = RangeType;
            r.RecId = RecId;

            return r;
        }
        /// <summary>
        /// Return minimum of end.X and start.X
        /// </summary>
        internal int FromX
        {
            get
            {
                if (End.iLine < Start.iLine) return End.iChar;
                if (End.iLine > Start.iLine) return Start.iChar;
                return Math.Min(End.iChar, Start.iChar);
            }
        }

        /// <summary>
        /// Return maximum of end.X and start.X
        /// </summary>
        internal int ToX
        {
            get
            {
                if (End.iLine < Start.iLine) return Start.iChar;
                if (End.iLine > Start.iLine) return End.iChar;
                return Math.Max(End.iChar, Start.iChar);
            }
        }

        public int FromLine
        {
            get { return Math.Min(Start.iLine, End.iLine); }
        }

        public int ToLine
        {
            get { return Math.Max(Start.iLine, End.iLine); }
        }




        public string GetRangeText(string[] sourceLines)
        {
            int fromLine = FromLine;
            int toLine = ToLine;
            int fromChar = FromX;
            int toChar = ToX;
            if (fromLine < 0) return null;
            //
            StringBuilder sb = new StringBuilder();
            for (int y = fromLine; y <= toLine; y++)
            {
                int fromX = y == fromLine ? fromChar : 0;
                int toX = y == toLine ? Math.Min(sourceLines[y].Length - 1, toChar - 1) : sourceLines[y].Length - 1;
                for (int x = fromX; x <= toX; x++)
                    sb.Append(sourceLines[y][x]);
                if (y != toLine && fromLine != toLine)
                    sb.AppendLine();
            }
            return sb.ToString();
        }

    }
    public enum TypeOfCodeRange
    {
        None,
        Trigger,
        Function,
        SourceExpr,

        ObjectsPropertiesDef,
        FieldsDef,
        PropertiesDef,
        ControlsDef,
        KeysDef,
        FieldGroupDef,
        MenuNodesDef,
        LabelsDef,
        RDLDataDef,
        EventsDef,
        ElementsDef,
        DataSetDef,
        //RequestPage_PropertiesDef,
        //RequestPage_ControlsDef,
        CodeDef,
        RequestPage
    }
    public enum NonCodeReferenceType
    {
        None,
        Permissions,
        DataCaptionFields,
        LookUpPage,
        DrillDownPage,
        SourceTable,
        TableRelation,
        AltSearchField,
        AccessByPermission,
        CalcFormula,
        KEYS,
        FIELDGROUPS,
        CanHaveGlobalVariable,
        SourceTableView,
        CardPageID,
        RunObject,
        RunPageLink,
        SourceExpr,
        SourceField,
        SubPageLink,
        PagePartID,
        RunPageView,
        DataItemTable,
        DataItemTableView,
        DataItemLink,
        DataItemLinkReference,
        ReqFilterFields,
        CalcFields,
        LinkFields,
        LinkTable,
        RunObjectType,
        RunObjectID,
        DataItemTableFilter,
        ColumnFilter,
        DataSource,
        
    }
}
