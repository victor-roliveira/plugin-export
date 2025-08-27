using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace SyncWorks.Methods
{
    public class ScheduleSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem) => elem is ViewSchedule;
        public bool AllowReference(Reference reference, XYZ position) => false;
    }

    public class ScheduleSelector
    {
        private readonly UIDocument _uiDoc;
        public ScheduleSelector(UIDocument uiDocument)
        {
            _uiDoc = uiDocument;
        }
        public IList<ViewSchedule> GetPreSelectedSchedules()

        {
            try
            {
                ICollection<ElementId> selectedIds = _uiDoc.Selection.GetElementIds();

                return selectedIds.Select(id => _uiDoc.Document.GetElement(id)).OfType<ViewSchedule>().ToList();
            }
            catch
            {
                Autodesk.Revit.UI.TaskDialog.Show("Erro", "Nenhum seleção foi feita.");
                return new List<ViewSchedule>();
            }
        }
    }
}
