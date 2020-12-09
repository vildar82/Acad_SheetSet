namespace Acad_SheetSet.Props
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using CsvHelper;
    using MicroMvvm;
    using Numeration;
    using Utils;

    public class PropsVM : ModelBase
    {
        private string _csvFile;

        public PropsVM(NumerationVM model)
        {
            Model = model;
            Create = new RelayCommand(CreateExec);
        }

        public NumerationVM Model { get; }

        public string CsvFile
        {
            get => _csvFile;
            set
            {
                _csvFile = value;
                UpdateCsv();
                RaisePropertyChanged();
            }
        }

        public List<SSProp> SSProps { get; set; }

        public List<SSProp> CsvProps { get; set; }

        public ICommand Create { get; set; }

        private void CreateExec()
        {
            try
            {
                // Удаление старых свойств подшивки и создание новых
                var ss = Model.Select.SheetSet;
                ss.CreateProps(CsvProps);
                SSProps = Model.Select.SheetSet.Props;
            }
            catch (Exception ex)
            {
                ex.ShowMessage();
            }
        }

        private void UpdateCsv()
        {
            try
            {
                CsvProps = new List<SSProp>();
                if (CsvFile.IsNullOrEmpty())
                {
                    return;
                }

                using var textReader = File.OpenText(CsvFile);
                var csv = new CsvReader(textReader, CultureInfo.CurrentCulture);
                csv.Configuration.HasHeaderRecord = true;
                CsvProps = csv.GetRecords<SSProp>().ToList();
            }
            catch (Exception ex)
            {
                ex.ShowMessage();
            }
        }
    }
}
