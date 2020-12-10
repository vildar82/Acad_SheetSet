namespace Acad_SheetSet.Batch
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Media;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using MicroMvvm;
    using Nodes;
    using Numeration;
    using Options;
    using Utils;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
    using Commands = Commands;

    public class BatchVM : ViewModelBase
    {
        private static BatchVM batchVm;
        private static NodeFile internalFile;
        private static Brush okColor = new SolidColorBrush(Colors.LightGreen);
        private static Brush errColor = new SolidColorBrush(Colors.Red);

        public BatchVM(NumerationVM model)
        {
            Model = model;
            Batch = new RelayCommand(BatchExec);
            CheckExistFile = new RelayCommand<NodeFile>(CheckExistFileExec);
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

        public ICommand Batch { get; set; }

        public bool IsBatchFiles { get; set; } = true;

        public bool IsBatchLayouts { get; set; } = true;

        public ICommand CheckExistFile { get; set; }

        private void CheckExistFileExec(NodeFile nodeFile)
        {
            nodeFile.IsExist = File.Exists(nodeFile.Name);
        }

        public void Update()
        {
            var oldNodes = Nodes.ToList();
            Nodes = new List<NodeFile>();
            if (Model.Select?.SheetSet == null)
            {
                return;
            }

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

        private void SetToBatch(List<NodeBase> nodes, List<NodeBase> oldNodes)
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
            if (!Nodes.Any())
            {
                return;
            }

            if (IsBatchFiles && batchVm.Options.FileExecute.IsNullOrEmpty())
            {
                AcadHelper.ShowMessage("Не задана команда обработки файлов.");
                return;
            }

            if (IsBatchLayouts && batchVm.Options.LayoutExecute.IsNullOrEmpty())
            {
                AcadHelper.ShowMessage("Не задана команда обработки листов.");
                return;
            }

            if (!IsBatchFiles && !IsBatchLayouts)
            {
                AcadHelper.ShowMessage("Отключена обработка файлов и листов.");
                return;
            }

            Execute(AcadHelper.Doc, nameof(Commands._InternalUse_SSBatchSession) + " ");
        }

        private static void BatchLayout(Document doc, NodeLayout nodeLayout)
        {
            Application.SetSystemVariable("CLAYOUT", nodeLayout.Name);
            Command(doc, batchVm.Options.LayoutExecute);
        }

        private static void Execute(Document doc, string execute)
        {
            doc.SendStringToExecute(execute, true, false, false);
        }

        private static void Command(Document doc, string command)
        {
            doc.Editor.Command(command);
        }

        public static void InternalBatchModal(Document doc)
        {
            if (internalFile == null)
            {
                return;
            }

            try
            {
                if (batchVm.IsBatchFiles && internalFile.ToBatch)
                {
                    if (!doc.Database.TileMode)
                    {
                        doc.Database.TileMode = true;
                    }

                    Command(doc, batchVm.Options.FileExecute);
                    internalFile.Color = okColor;
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
                Execute(doc, nameof(Commands._InternalUse_SSBatchSession) + " ");
            }
        }

        public static void InternalBatchSession(Document doc)
        {
            if (internalFile == null)
            {
                internalFile = batchVm?.Nodes?.FirstOrDefault();
                if (internalFile == null)
                {
                    return;
                }
            }
            else
            {
                if (internalFile.NeedCloseFile)
                {
                    var adoc = AcadHelper.Doc;
                    try
                    {
                        adoc.CloseAndDiscard();
                    }
                    catch
                    {
                        // ignore
                    }
                }

                internalFile = internalFile.NextFile;
                if (internalFile == null)
                {
                    return;
                }

                internalFile.NeedCloseFile = false;
            }

            internalFile.BatchResult = null;
            internalFile.Color = null;

            if (NeedBatchFile(internalFile))
            {
                doc = AcadHelper.GetOpenedDocument(internalFile.Name);
                if (doc == null)
                {
                    internalFile.NeedCloseFile = true;
                    doc = Application.DocumentManager.Open(internalFile.Name);
                    Application.DocumentManager.MdiActiveDocument = doc;
                }
            }

            Execute(doc, nameof(Commands._InternalUse_SSBatchModal) + " ");
        }

        private static bool NeedBatchFile(NodeFile nodeFile)
        {
            return batchVm.IsBatchFiles && nodeFile.ToBatch ||
                   batchVm.IsBatchLayouts && nodeFile.Nodes.Any(n => n.ToBatch);
        }
    }
}
