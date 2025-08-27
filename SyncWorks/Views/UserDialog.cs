namespace SyncWorks.Views
{
    public class UserDialog
    {
        public string AskForSaveLocation(string suggestedFileName)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.FileName = suggestedFileName;
                saveDialog.Filter = "Arquivo CSV (*.csv)|*.csv|Arquivo de Texto (*.txt)|*.txt|Todos os arquivos (*.*)|*.*";
                saveDialog.FilterIndex = 1;
                saveDialog.RestoreDirectory = true;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveDialog.FileName;
                }
            }

            return null;
        }
        public void ShowSuccessMessage(string title, string message)
        {
            Autodesk.Revit.UI.TaskDialog.Show(title, message);
        }

        public void ShowErrorMessage(string title, string message)
        {
            Autodesk.Revit.UI.TaskDialog.Show(title, message);
        }
    }
}
