using System;
using System.IO;
using System.Web.UI.WebControls;
using Root.Reports;
namespace NuevoSicop.Models
{
    //----------------------------------------------------------------------------------------------------x
    /// <summary></summary>
    public class ReportePDF : Report
    {
        private FontDef _fontDefinition;
        private FontProp _fontProperty;
        private Orientation page = Orientation.Horizontal;
        private double rMarginLeft = 10; // millimeters
        private double rWidth = 175;
        //private Root.Reports View =   ;


        protected override void Create()
        {
            // Este llamado es muy importante para generar la página
            NewPage();
            // Inicialia las propiedades del reporte
            InitializeReportProperties();
        }

        public void InitializeReportProperties()
        {
            //this.
            //    // Este estilo de fuente debe ser definido sólo una vez
            //    _fontDefinition = new FontDef(this, FontDef.StandardFont.Helvetica);
            //_fontProperty = new FontProp(_fontDefinition, 10, Color.Black);

            var pf = (PdfFormatter)formatter;

            pf.sTitle = "Generador";
            pf.sAuthor = "ITIFE TABASCO";
            pf.sSubject = "Oficios";
            pf.sKeywords = "Reporte";
            pf.sCreator = "David Salomón";

            pf.dt_CreationDate = new DateTime().ToLocalTime();
            pf.pageLayout = PageLayout.TwoColumnLeft;
            pf.bHideToolBar = false;
            pf.bHideMenubar = false;
            pf.bHideWindowUI = true;
            pf.bFitWindow = true;
            pf.bCenterWindow = true;
            pf.bDisplayDocTitle = true;
            pf.pageMode = PdfFormatter.PageMode.FullScreen;


        }

        public void NewPage( )
        {

            new Page(this);
            page_Cur.rAlignH = page_Cur.rAlignV;
            page_Cur.rHeightMM = 216.0;
            page_Cur.rWidthMM = 280.0;
        }

        public void NewPage(double ancho,  double alto)
        {

            new Page(this);
            page_Cur.rAlignH = page_Cur.rAlignV;
            page_Cur.rHeightMM = ancho;
            page_Cur.rWidthMM = alto;
        }

        public void NewPageLegal()
        {

            new Page(this);
            page_Cur.rAlignH = page_Cur.rAlignV;
            page_Cur.rHeightMM = 216.0;
            page_Cur.rWidthMM = 356.0;
        }
        public void NewPageTabloide()
        {

            new Page(this);
            page_Cur.rAlignH = page_Cur.rAlignV;
            page_Cur.rHeightMM = 279.0;
            page_Cur.rWidthMM = 432.0;
        }

        public void NewPageVertical()
        {

            new Page(this);
            page_Cur.rHeightMM = 280.0;
            page_Cur.rWidthMM = 210.0;
        }
    }
}

namespace Root.Reports
{
    /// <summary>Image Data Object</summary>
    public class ImageData
    {
        /// <summary>Internal structure used by the formatter</summary>
        internal object oImageResourceX;

        /// <summary>Image stream</summary>
        internal Stream stream;

        //----------------------------------------------------------------------------------------------------x
        /// <summary>Creates a new image data object</summary>
        /// <param name="stream">Image stream</param>
        public ImageData(Stream stream)
        {
            this.stream = stream;
        }
    }
}