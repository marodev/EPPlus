﻿/*************************************************************************************************
  Required Notice: Copyright (C) EPPlus Software AB. 
  This software is licensed under PolyForm Noncommercial License 1.0.0 
  and may only be used for noncommercial purposes 
  https://polyformproject.org/licenses/noncommercial/1.0.0/

  A commercial license to use this software can be purchased at https://epplussoftware.com
 *************************************************************************************************
  Date               Author                       Change
 *************************************************************************************************
  06/26/2020         EPPlus Software AB       EPPlus 5.3
 ******0*******************************************************************************************/
using OfficeOpenXml.Utils.Extentions;
using System;
using System.Xml;

namespace OfficeOpenXml.Drawing.Slicer
{
    /*
      <xsd:complexType name="CT_Slicer">
       <xsd:sequence>
         <xsd:element name="extLst" type="x:CT_ExtensionList" minOccurs="0" maxOccurs="1"/>
       </xsd:sequence>
       <xsd:attribute name="name" type="x:ST_Xstring" use="required"/>
       <xsd:attribute ref="xr10:uid" use="optional"/>
       <xsd:attribute name="cache" type="x:ST_Xstring" use="required"/>
       <xsd:attribute name="caption" type="x:ST_Xstring" use="optional"/>
       <xsd:attribute name="startItem" type="xsd:unsignedInt" use="optional" default="0"/>
       <xsd:attribute name="columnCount" type="xsd:unsignedInt" use="optional" default="1"/>
       <xsd:attribute name="showCaption" type="xsd:boolean" use="optional" default="true"/>
       <xsd:attribute name="level" type="xsd:unsignedInt" use="optional" default="0"/>
       <xsd:attribute name="style" type="x:ST_Xstring" use="optional"/>
       <xsd:attribute name="lockedPosition" type="xsd:boolean" use="optional" default="false"/>
       <xsd:attribute name="rowHeight" type="xsd:unsignedInt" use="required"/>
     </xsd:complexType>
     */
    public class ExcelSlicer : ExcelDrawing
    {
        internal ExcelWorksheet _ws;
        XmlHelper _slicerXmlHelper;
        internal ExcelSlicer(ExcelDrawings drawings, XmlNode node, ExcelGroupShape parent=null) :
            base(drawings, node, "mc:AlternateContent/mc:Choice/xdr:graphicFrame", "xdr:nvGraphicFramePr/xdr:cNvPr", parent)
        {
            _ws = drawings.Worksheet;
            var slicerNode = _ws.SlicerXml.DocumentElement.SelectSingleNode($"x14:slicer[@name=\"{Name}\"]", NameSpaceManager);
            _slicerXmlHelper = XmlHelperFactory.Create(NameSpaceManager, slicerNode);
        }
        /// <summary>
        /// The caption text of the slicer.
        /// </summary>
        public string Caption 
        {
            get
            {
                return _slicerXmlHelper.GetXmlNodeString("@caption");
            }
            set
            {
                _slicerXmlHelper.SetXmlNodeString("@caption", value);
            }
        }
        /// <summary>
        /// Row height in points
        /// </summary>
        public double RowHeight 
        { 
            get
            {
                return _slicerXmlHelper.GetXmlNodeEmuToPt("@rowHeight");
            }
            set
            {
                _slicerXmlHelper.SetXmlNodeEmuToPt("@rowHeight", value);
            }
        }
        /// <summary>
        /// The start item
        /// </summary>
        public int StartItem
        {
            get
            { 
                return _slicerXmlHelper.GetXmlNodeInt("@startItem", 0);
            }
            set
            {
                _slicerXmlHelper.SetXmlNodeInt("@startItem", value, null, false);
            }
        }
        /// <summary>
        /// Number of columns. Default is 1.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return _slicerXmlHelper.GetXmlNodeInt("@columnCount", 1);
            }
            internal set
            {
                _slicerXmlHelper.SetXmlNodeInt("@columnCount", value, null, false);
            }
        }
        /// <summary>
        /// The build in slicer style.
        /// If set to Custom, the name in the <see cref="StyleName" /> is used 
        /// </summary>
        public eSlicerStyle Style
        {
            get
            {
                return StyleName.TranslateSlicerStyle();
            }
            set
            {
                if(value==eSlicerStyle.None)
                {
                    StyleName = "";
                }
                else if(value != eSlicerStyle.Custom)
                {
                    StyleName = "SlicerStyle" + value.ToString();
                }
            }
        }
        /// <summary>
        /// The style name used for the slicer.
        /// <seealso cref="Style"/>
        /// </summary>
        public string StyleName
        {
            get
            {
                return GetXmlNodeString("@style");
            }
            set
            {
                if(String.IsNullOrEmpty(value))
                {
                    Style = eSlicerStyle.None;
                }
                if(value.StartsWith("SlicerStyle", StringComparison.OrdinalIgnoreCase))
                {
                    var style = value.Substring(11).ToEnum<eSlicerStyle>(eSlicerStyle.Custom);
                    if(style!=eSlicerStyle.Custom || style!=eSlicerStyle.None)
                    {
                        Style = style;
                        return;
                    }
                }
                Style = eSlicerStyle.Custom;
                SetXmlNodeString("@style", value);
            }
        }
        internal string CacheName
        {
            get
            {
                return _slicerXmlHelper.GetXmlNodeString("@cache");
            }
            set
            {
                _slicerXmlHelper.SetXmlNodeString("@cache", value);
            }
        }
        ExcelSlicerCache _cache = null;
        public ExcelSlicerCache Cache
        {
            get
            {
                if(_cache==null)
                {
                    _cache = _drawings.Worksheet.Workbook.GetSlicerCaches(CacheName);
                }
                return _cache;
            }
        }
    }
}
