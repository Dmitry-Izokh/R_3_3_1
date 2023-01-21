using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R_3_3_1
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        /*public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Выберете элемент");
            var secectedElement = doc.GetElement(selectedRef);
            
            if (secectedElement is Wall)
            {
                // Вариант обращения через имя в английской версии Revit
                Parameter lengthParametr1 = secectedElement.LookupParameter("Length");                
                    if (lengthParametr1.StorageType == StorageType.Double)
                    {
                        TaskDialog.Show("Length1", lengthParametr1.AsDouble().ToString());
                    }
                // Вариант обращения любой версии Revit (ПРЕДПОЧТИТЕЛЬНЫЙ ВАРИАНТ)
                Parameter lengthParametr2 = secectedElement.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                if (lengthParametr1.StorageType == StorageType.Double)
                {
                    TaskDialog.Show("Length1", lengthParametr2.AsDouble().ToString());
                }
            }
            return Result.Succeeded;
        }
    } */
    
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {

        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

            IList<Reference> selectedElementRefList = null;
           try
            {
                selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Face, "Выберете элемент");
                var wallList = new List<Wall>();
                foreach (var selectedElement in selectedElementRefList)
                {
                    Element element = doc.GetElement(selectedElement);
                    if (element is Wall)
                    {
                        Wall oWall = (Wall)element;
                        wallList.Add(oWall);
                    }
                }
                foreach (Wall oWall in wallList)
                {
                    Parameter lengthParametr = oWall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                    if (lengthParametr.StorageType == StorageType.Double)
                    {
                        TaskDialog.Show("Length", lengthParametr.AsDouble().ToString());
                    }
                }
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
            { }

            if (selectedElementRefList == null)
            {
                return Result.Cancelled;
            }        
        return Result.Succeeded;
    }
}
}
