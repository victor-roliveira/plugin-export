using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using SyncWorks.Commands;

namespace SyncWorks
{
    public class MenuRibbon : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "SyncWorks";
            try { application.CreateRibbonTab(tabName); } catch { }

            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Exportação de Dados");

            PushButtonData buttonExportData = new PushButtonData(
                "BtnExportDatas", "Exportar", Assembly.GetExecutingAssembly().Location, typeof(ExportScheduleCommand).FullName
            );

            LoadButtonImage(buttonExportData, "icon-export.png");

            buttonExportData.ToolTip = "Exporta dados das tabelas para um arquivo csv ou xls.";
            panel.AddItem(buttonExportData);

            return Result.Succeeded;
        }

        private void LoadButtonImage(PushButtonData buttonData, string imageName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                string resourceName = $"SyncWorks.images.{imageName}";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        buttonData.LargeImage = bitmap;
                    }
                    else
                    {
                        Autodesk.Revit.UI.TaskDialog.Show("Erro de Imagem", $"Recurso não encontrado: {resourceName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Erro de Imagem", $"Falha ao carregar imagem: {ex.Message}");
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}