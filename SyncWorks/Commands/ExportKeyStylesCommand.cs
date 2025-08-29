//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using SyncWorks.Methods;
//using SyncWorks.Views;

//namespace SyncWorks.Commands
//{
//    public class ExportKeyStylesCommand : IExternalCommand
//    {
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            UIApplication uiapp = commandData.Application;
//            UIDocument uidoc = uiapp.ActiveUIDocument;
//            Document doc = uidoc.Document;

//            var selector = new ScheduleSelector(uidoc);
//            var extractor = new ScheduleExtractor();
//            var exporter = new FileExporter();
//            var dialogs = new UserDialog();

//            var mainScheduleList = selector.GetPreSelectedSchedules();
//            if (mainScheduleList.Count == 0)
//            {
//                dialogs.ShowErrorMessage("Nenhuma tabela de quantitativos encontrada no modelo.", "Por favor selecione alguma tabela.");
//                return Result.Cancelled;
//            }

//            ViewSchedule mainSchedule = mainScheduleList.First();

//            ViewSchedule keySchedule = null;
//            string keyScheduleFieldName = "";

//            var visibleFields = mainSchedule.Definition.GetFieldOrder()
//                .Select(id => mainSchedule.Definition.GetField(id))
//                .Where(field => !field.IsHidden);

//            foreach (var field in visibleFields)
//            {
//                // 1. Obtemos o parâmetro associado ao campo da tabela.
//                ElementId paramId = field.ParameterId;
//                if (paramId != null && paramId != ElementId.InvalidElementId)
//                {
//                    // 2. Verificamos se este parâmetro é do tipo "Key Schedule".
//                    // A forma de fazer isso é vendo se o parâmetro aponta para um elemento que é uma ViewSchedule.
//                    Element paramElem = doc.GetElement(paramId);
//                    if (paramElem is ParameterElement parameterElement)
//                    {
//                        Parameter p = parameterElement.GetDefinition();
//                        var keyScheduleParam = p as KeyScheduleParameter; // Tentativa de cast

//                        // Infelizmente, a API não expõe uma forma direta e simples.
//                        // A comunidade descobriu que a forma mais confiável é, infelizmente, complexa.

//                        // VAMOS TENTAR A ABORDAGEM MAIS SIMPLES QUE PODE FUNCIONAR:
//                        // Verificando se o nome do parâmetro corresponde a um nome de tabela de chave
//                         keySchedule = doc.GetElement(paramId) as KeySchedule;

//                        // A API para isso é notoriamente difícil. Vamos simplificar drasticamente.
//                        // A sua sugestão é a melhor pista. Vamos checar a DEFINIÇÃO da tabela a que este campo pertence.

//                        // A forma mais simples é iterar todas as tabelas de chave e ver se alguma delas
//                        // define o campo que estamos olhando.
//                    }
//                }
//            }

//            if (keySchedule == null)
//            {
//                dialogs.ShowErrorMessage("Nenhuma tabela de estilos-chave encontrada.", "A tabela selecionada não possui uma tabela de estilos-chave associada.");
//                return Result.Cancelled;
//            }

//            var extractedKeyData = extractor.Extract(keySchedule);
//            if (extractedKeyData == null)
//            {
//                dialogs.ShowErrorMessage("Falha na extração dos dados.", "Não foi possível extrair os dados da tabela de estilos-chave.");
//                return Result.Failed;
//            }

//            string savePath = dialogs.AskForSaveLocation($"{keySchedule.Name}_KeyStyles.csv");
//            if (string.IsNullOrEmpty(savePath))
//            {
//                return Result.Cancelled;
//            }

//            exporter.ExportToCsv(extractedKeyData, savePath);
//            dialogs.ShowSuccessMessage("Exportação Concluída", $"Os dados da tabela de estilos-chave '{keySchedule.Name}' foram exportados com sucesso para:\n{savePath}");

//            return Result.Succeeded;
//        }
//    }
//}
