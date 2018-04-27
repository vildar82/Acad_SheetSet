using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using AcadLib;
using Acad_SheetSet.Batch.Nodes;
using Acad_SheetSet.Numeration;
using Acad_SheetSet.Options;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using AXDBLib;
using JetBrains.Annotations;
using NetLib.Monad;
using NetLib.WPF;
using ReactiveUI;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Acad_SheetSet.Batch
{
    public class BatchVM : BaseModel
    {
        private static BatchVM batchVm;
        private static NodeFile internalFile;
        private static Brush okColor = new SolidColorBrush(Colors.LightGreen);
        private static Brush errColor = new SolidColorBrush(Colors.Red);

        public BatchVM([NotNull] NumerationVM model) : base (model)
        {
            Model = model;
            Batch = CreateCommand(BatchExec);
            CheckExistFile = CreateCommand<NodeFile>(CheckExistFileExec);
            if (model.Options.Options.Batch == null)
            {
                model.Options.Options.Batch = new BatchOptions();
            }
            Options = model.Options.Options.Batch;
            batchVm = this;
        }

        public NumerationVM Model { get; }
        public bool ExpandTreeView { get; set; } = true;

        public List<NodeFile> Nodes { get; set; } = new List<NodeFile>();

        public BatchOptions Options { get; set; }
        public ReactiveCommand Batch { get; set; }

        public bool IsBatchFiles { get; set; } = true;
        public bool IsBatchLayouts { get; set; } = true;

        public ReactiveCommand CheckExistFile { get; set; }

        private void CheckExistFileExec([NotNull] NodeFile nodeFile)
        {
            nodeFile.IsExist = File.Exists(nodeFile.Name);
        }

        public void Update()
        {
            var oldNodes = Nodes.ToList();
            Nodes = new List<NodeFile>();
            if (Model.Select?.SheetSet == null) return;
            var nodes = new List<NodeFile>();
            NodeFile parentFile = null;
            var sheets = Model.Select.SheetSet.Nodes.SelectMany(s => s.GetSheets()).ToList();
            foreach (var grouping in sheets.GroupBy(g=>g.File))
            {
                var nodeFile = new NodeFile
                {
                    Name = grouping.Key,
                    Nodes = grouping.Select(s=> new NodeLayout
                    {
                        Name = s.Layout
                    }).ToList(),
                    IsExist = File.Exists(grouping.Key)
                };
                nodes.Add(nodeFile);
                if (parentFile != null)
                {
                    parentFile.NextFile = nodeFile;
                }
                parentFile = nodeFile;
            }
            SetToBatch(nodes.ToList<NodeBase>(), oldNodes.ToList<NodeBase>());
            Nodes = nodes;
        }

        private void SetToBatch([NotNull] List<NodeBase> nodes, List<NodeBase> oldNodes)
        {
            foreach (var node in nodes)
            {
                var oldNode = oldNodes.FirstOrDefault(f => f.Name == node.Name);
                if (oldNode != null)
                {
                    node.ToBatch = oldNode.ToBatch;
                    if (node is NodeFile nodeFile && oldNode is NodeFile oldNodeFile)
                    {
                        SetToBatch(nodeFile.Nodes.ToList<NodeBase>(),oldNodeFile.Nodes.ToList<NodeBase>());
                    }
                }
            }
        }

        private void BatchExec()
        {
            if (!Nodes.Any()) return;
            if (!IsBatchFiles && !IsBatchLayouts)
            {
                ShowMessage("Отключена обработка файлов и листов.");
                return;
            }
            var nodeFile = Nodes.First();
            nodeFile.BatchResult = null;
            nodeFile.Color = null;
            BatchFile(nodeFile);
        }

        private void BatchFile([CanBeNull] NodeFile nodeFile)
        {
            if (nodeFile == null)
            {
                internalFile = null;
                return;
            }
            internalFile = nodeFile;
            nodeFile.NeedCloseFile = false;
            var doc = AcadHelper.GetOpenedDocument(nodeFile.Name);
            if (doc == null)
            {
                nodeFile.NeedCloseFile = true;
                doc = Application.DocumentManager.Open(nodeFile.Name);
                Application.DocumentManager.MdiActiveDocument = doc;
            }
            Execute(doc, nameof(Commands._InternalUse_SheetSetBatch) + " ");
        }

        private static void BatchLayout([NotNull] Document doc, [NotNull] NodeLayout nodeLayout)
        {
            Application.SetSystemVariable("CLAYOUT", nodeLayout.Name);
            Command(doc, batchVm.Options.FileExecute);
        }

        private static void Execute([NotNull] Document doc, [NotNull] string execute)
        {
            doc.SendStringToExecute(execute, true, false, true);
        }

        private static void Command([NotNull] Document doc, [NotNull] string command)
        {
            doc.Editor.Command(command);
        }

        public static void InternalBatch([NotNull] Document doc)
        {
            if (internalFile == null) return;
            try
            {
                if (batchVm.IsBatchFiles)
                {
                    if (!doc.Database.TileMode)
                        doc.Database.TileMode = true;
                    Command(doc, batchVm.Options.FileExecute);
                }
                if (batchVm.IsBatchLayouts)
                {
                    foreach (var nodeLayout in internalFile.Nodes.Where(w => w.ToBatch))
                    {
                        try
                        {
                            nodeLayout.BatchResult = null;
                            BatchLayout(doc, nodeLayout);
                            nodeLayout.Color = okColor;
                        }
                        catch (Exception ex)
                        {
                            nodeLayout.Color = errColor;
                            nodeLayout.BatchResult = ex.Message;
                        }
                    }
                }
                doc.Database.SaveAs(internalFile.Name, DwgVersion.Current);
            }
            catch (Exception ex)
            {
                internalFile.Color = errColor;
                internalFile.BatchResult = ex.Message;
            }
            finally
            {
                if (internalFile.NeedCloseFile)
                {
                    doc.Try(d=>d.CloseAndDiscard());
                }
                batchVm.BatchFile(internalFile.NextFile);
            }
        }
    }
}
