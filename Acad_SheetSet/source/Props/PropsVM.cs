namespace Acad_SheetSet.Props
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive;
    using CsvHelper;
    using NetLib;
    using NetLib.WPF;
    using Numeration;
    using ReactiveUI;

    public class PropsVM : BaseModel
    {
        public PropsVM(NumerationVM model)
            : base (model)
        {
            Model = model;
            Create = CreateCommand(CreateExec);
            this.WhenAnyValue(v => v.CsvFile).Subscribe(s => UpdateCsv());
        }

        public NumerationVM Model { get; }

        public string CsvFile { get; set; }

        public List<SSProp> SSProps { get; set; }

        public List<SSProp> CsvProps { get; set; }

        public ReactiveCommand<Unit, Unit> Create { get; set; }

        private void CreateExec()
        {
            // Удаление старых свойств подшивки и создание новых
            var ss = Model.Select.SheetSet;
            ss.CreateProps(CsvProps);
            SSProps = Model.Select.SheetSet.Props;
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

                using (var textReader = File.OpenText(CsvFile))
                {
                    var csv = new CsvReader(textReader);
                    csv.Configuration.HasHeaderRecord = true;
                    CsvProps = csv.GetRecords<SSProp>().ToList();
                }
            }
            catch (Exception ex)
            {
                AcadLib.Logger.Log.Error(ex, $"Acad_SheetSet UpdateCsv - {CsvFile}");
                ShowMessage(ex.Message);
            }
        }
    }
}
