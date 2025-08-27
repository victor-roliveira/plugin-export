using Autodesk.Revit.DB;
using SyncWorks.Models;

namespace SyncWorks.Methods
{
    public class ScheduleExtractor
    {
        public ExtractedScheduleData Extract(ViewSchedule schedule)
        {
            if (schedule == null) return null;

            Document doc = schedule.Document;
            var extractedData = new ExtractedScheduleData { ScheduleName = schedule.Name };

            var visibleFields = schedule.Definition.GetFieldOrder()
                .Select(fieldId => schedule.Definition.GetField(fieldId))
                .Where(field => !field.IsHidden)
                .ToList();

            var headerRow = new List<string>();
            foreach (var field in visibleFields)
            {
                headerRow.Add(field.ColumnHeading);
            }
            extractedData.Headers.Add(headerRow);

            var collector = new FilteredElementCollector(doc, schedule.Id);
            var scheduleElements = collector.ToElements();
            foreach (Element elem in scheduleElements)
            {
                var rowData = new List<string>();
                foreach (ScheduleField field in visibleFields)
                {
                    string cellValue = GetElementValueForField(elem, field, doc);
                    rowData.Add(cellValue);
                }
                extractedData.Rows.Add(rowData);
            }

            if (schedule.Definition.ShowGrandTotal)
            {
                TableData tableData = schedule.GetTableData();
                TableSectionData bodySection = tableData.GetSectionData(SectionType.Body);
                int lastRowIndex = bodySection.NumberOfRows - 1;

                if (lastRowIndex >= 0)
                {
                    var totalRowData = new List<string>();
                    // Para a linha de total, vamos confiar no que o Revit mostra
                    for (int j = 0; j < visibleFields.Count; j++)
                    {
                        string cellText = bodySection.GetCellText(lastRowIndex, j);
                        // Limpamos a string para garantir que VAZIO seja consistente
                        if (string.IsNullOrWhiteSpace(cellText)) cellText = "VAZIO";
                        else cellText = cellText.Replace("\r\n", " ").Replace(";", ",");
                        totalRowData.Add(cellText);
                    }

                    if (totalRowData.Any(s => s != "VAZIO"))
                    {
                        extractedData.Rows.Add(totalRowData);
                    }
                }
            }

            return extractedData;
        }

        private string GetElementValueForField(Element element, ScheduleField field, Document doc)
        {
            // CORREÇÃO FINAL E ESPECÍFICA PARA A COLUNA DE QUANTIDADE:
            // Verificamos se o campo é do tipo Contagem OU se o nome da coluna customizado pelo usuário é "QUANT."
            if (field.FieldType == ScheduleFieldType.Count || field.ColumnHeading.ToUpper().StartsWith("QUANT"))
            {
                return "1"; // Em uma lista itemizada (que é o que o ERP precisa), a quantidade de cada item é sempre 1.
            }

            string value = "VAZIO";
            Parameter param = null;
            ElementId paramId = field.ParameterId;

            if (paramId == null || paramId == ElementId.InvalidElementId)
            {
                return value; // Retorna "VAZIO" para outros campos sem parâmetro
            }

            if (paramId.IntegerValue == (int)BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM)
            {
                if (doc.GetElement(element.GetTypeId()) is ElementType elementType) return $"{elementType.FamilyName} : {elementType.Name}";
            }

            param = GetParameter(element, paramId, doc);
            if (param == null || !param.HasValue || (param.StorageType == StorageType.String && string.IsNullOrEmpty(param.AsString())))
            {
                ElementType elementType = doc.GetElement(element.GetTypeId()) as ElementType;
                if (elementType != null) param = GetParameter(elementType, paramId, doc);
            }

            if (param != null)
            {
                value = param.AsString();
                if (string.IsNullOrEmpty(value)) value = param.AsValueString();
            }

            if (string.IsNullOrWhiteSpace(value)) value = "VAZIO";
            return value.Replace("\r\n", " ").Replace(";", ",");
        }

        private Parameter GetParameter(Element element, ElementId paramId, Document doc)
        {
            if (paramId.Value < 0) return element.get_Parameter((BuiltInParameter)paramId.Value);
            if (doc.GetElement(paramId) is ParameterElement paramElem) return element.get_Parameter(paramElem.GetDefinition());
            return null;
        }
    }
}

