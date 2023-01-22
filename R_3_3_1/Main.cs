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
            //try-catch отрабатывает механизм отмены программы пр нажатии  Esc
            try
            {
                // 2 строчки ниже это стандартная запись для выбора элементов кликом.
                selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Face, "Выберете элемент");
                var wallList = new List<Wall>();
                
                // Перебираем спиок выбранных элементов и если в этом списке встречается нужный нам тип элемента,
                // то эти элементы перекладываем в новый список
                foreach (var selectedElement in selectedElementRefList)
                {
                    Element element = doc.GetElement(selectedElement);
                    if (element is Wall)
                    {
                        Wall oWall = (Wall)element;
                        wallList.Add(oWall);
                    }
                }

                //Создаем новый список volumeSelectedWallList
                //куда заносим значения параметра HOST_VOLUME_COMPUTED для элеметов(oWall) списка wallList.
                //Выполняем это перебирая список циклом foreach
                List<double> volumeSelectedWallList = new List<double>();
                foreach (Wall oWall in wallList)
                {
                    //Создаем переменную параметра которая назначается для каждого элемента списка.
                    //Переменной параметра выбирается параметр в Revit отвечающий за вычисления объема HOST_VOLUME_COMPUTED.
                    Parameter volumeParametr = oWall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);                    
                   
                    //Создаем переменную в которую записываем значение параметра для каждого элемента списка
                    double volumeSelectedWall;                    
                    
                    //проверяем переменную на соответствие с типом данных (double), чтобы иметь возможность произвести вычисление
                    if (volumeParametr.StorageType == StorageType.Double)
                    {
                        volumeSelectedWall= volumeParametr.AsDouble();
                        // Переводим единицы измерения в кубические метры
                        volumeSelectedWall = UnitUtils.ConvertFromInternalUnits(volumeSelectedWall, /*UnitTypeId.CubicMeters*/DisplayUnitType.DUT_CUBIC_METERS);
                        // Добавляем вычисленные результаты в созданный выше списокб
                        // на выходе имеем список значений объема выбранных элементов (стен) в кубических метрах.
                        volumeSelectedWallList.Add(volumeSelectedWall);                        
                    }
                    
                }

                // Создаем переменную для в которую рассчитаем значение суммы всех элементов получившегося списка.
                double sumVolume = volumeSelectedWallList.ToArray().Sum();
                //Выводим результат в диалоговое окно.
                TaskDialog.Show("Суммарный объем", $"{sumVolume}");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            { }

            // If отрабатывает выключение программы в случае когда ничего не выбрано. Это продолжение кода try-catch
            if (selectedElementRefList == null)
            {
                return Result.Cancelled;
            }
            return Result.Succeeded;
        }
    }
}
    