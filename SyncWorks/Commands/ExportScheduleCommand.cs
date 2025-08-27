using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SyncWorks.Methods;
using SyncWorks.Views;

namespace SyncWorks.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExportScheduleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selector = new ScheduleSelector(uidoc);
            var extractor = new ScheduleExtractor();
            var exporter = new FileExporter();
            var dialogs = new UserDialog();

            var selectedSchedules = selector.GetPreSelectedSchedules();

            if (selectedSchedules.Count == 0)
            {
                dialogs.ShowErrorMessage("Nenhuma Seleção", "Por favor, selecione uma ou mais tabelas no Navegador de Projeto ANTES de executar o comando.");
                return Result.Cancelled;
            }

            try
            {
                foreach (var schedule in selectedSchedules)
                {
                    using (Transaction t = new Transaction(doc, "Exportando dados da tabela"))
                    {
                        t.Start();

                        ScheduleDefinition definition = schedule.Definition;
                        bool wasItemized = definition.IsItemized;

                        if (!wasItemized)
                        {
                            definition.IsItemized = true;
                            doc.Regenerate();
                        }

                        var extractedData = extractor.Extract(schedule);
                        // Desfazemos a transação para não alterar o projeto do usuário.
                        t.RollBack();

                        if (extractedData == null) continue;

                        string savePath = dialogs.AskForSaveLocation($"{extractedData.ScheduleName}.csv");
                        if (string.IsNullOrEmpty(savePath)) continue;

                        exporter.ExportToCsv(extractedData, savePath);
                        dialogs.ShowSuccessMessage("Sucesso", $"A tabela '{schedule.Name}' foi exportada com sucesso para:\n{savePath}");
                    }
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Ocorreu um erro inesperado: {ex.Message}";
                dialogs.ShowErrorMessage("Erro", message);
                return Result.Failed;
            }
        }
    }
}
